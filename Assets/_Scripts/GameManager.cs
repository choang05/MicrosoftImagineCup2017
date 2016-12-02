using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Com.LuisPedroFonseca.ProCamera2D;

[Serializable]
class PlayerData
{
    public int lastCheckpointID;
}

public class GameManager : MonoBehaviour
{
    //  Static instance of object to dnot destroy on load
    private static GameManager control;

    //  User Parameters variables
    public GameObject playerPrefab;
    public int CurrentCheckpointID;
    public bool hasPresentWisp = false;
    public bool hasPastWisp = false;
    public bool hasFutureWisp = false;

    //  Private
    [HideInInspector] public Checkpoint[] Checkpoints;

    void OnEnable()
    {
        SceneManager.sceneLoaded += SetUpScene;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= SetUpScene;
    }

    void Awake()
    {
        #region Dont Destroy On Load
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
            Destroy(gameObject);
        #endregion
    }

    //  Function to unload a level segment given its checkpoint ID
    public void UnloadLevelSegment(int checkpointID)
    {
        for (int i = 0; i < Checkpoints.Length; i++)
            if (Checkpoints[i].checkpointID == checkpointID)
            {
                Checkpoints[i].LevelSegmentsGO.SetActive(false);
                break;
            }
    }

    //  Function to load in a level segment given its checkpoint ID
    public void LoadLevelSegment(int checkpointID)
    {
        for (int i = 0; i < Checkpoints.Length; i++)
            if (Checkpoints[i].checkpointID == checkpointID)
            {
                Checkpoints[i].LevelSegmentsGO.SetActive(true);
                break;
            }
    }

    //  Public function to start the coroutine
    public void StartCoRespawn()
    {   StartCoroutine(CoRespawn());    }

    //  Coroutine that handles respawning in timly manner
    private IEnumerator CoRespawn()
    {
        WorldChanger worldChanger = FindObjectOfType<WorldChanger>();
        worldChanger.TransitionCameraExit();

        yield return new WaitForSeconds(worldChanger.MainCamera.GetComponent<ProCamera2DTransitionsFX>().DurationExit);

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    //  Sets up the scene on master scene loaded
    private void SetUpScene(Scene scene, LoadSceneMode mode)
    {
        //  If the scene loaded was not the master scene, then do nothing.
        if (scene.buildIndex != 1)
            return;

        //  Fetch & Cache all the checkpoints, then sort by their checkpoint IDs
        Checkpoints = FindObjectsOfType<Checkpoint>();
        SortCheckpointsByQuickSort(Checkpoints, 0, Checkpoints.Length-1);

        //  Find the current areaID the player is in
        Vector3 currentCheckpointPosition = Vector3.zero;
        for (int i = 0; i < Checkpoints.Length; i++)
        {
            if (Checkpoints[i].checkpointID == CurrentCheckpointID)
            {                
                //  Set the player position of the checkpoint
                currentCheckpointPosition = Checkpoints[i].transform.position;

                //  Load in the level segment previous of the checkpointID
                if (i - 1 >= 0)
                    Checkpoints[i - 1].LevelSegmentsGO.SetActive(true);

                //  Load in the level of the checkpoint ID
                Checkpoints[i].LevelSegmentsGO.SetActive(true);

                //  Load the level segment after the checkpoint ID
                if (i + 1 < Checkpoints.Length)
                    Checkpoints[i + 1].LevelSegmentsGO.SetActive(true);

                break;
            }
        }


        //  Instaniate the player at the checkpoint location
        GameObject player = Instantiate(playerPrefab, currentCheckpointPosition, Quaternion.identity) as GameObject;

        //  Set up camera
        ProCamera2D camera = ProCamera2D.Instance;
        camera.MoveCameraInstantlyToPosition(new Vector2(player.transform.position.x, player.transform.position.y));
        camera.AddCameraTarget(player.transform);
        camera.GetComponent<ProCamera2DRails>().AddRailsTarget(player.transform);

        //  Set up world changer
        WorldChanger worldChanger = FindObjectOfType<WorldChanger>();
        worldChanger.charController2D = player.GetComponent<CharacterController2D>();
        worldChanger.TransitionCameraEnter();

        //  Set up the parallax
        /*FreeParallax[] parallaxes = FindObjectsOfType<FreeParallax>();
        for (int i = 0; i < parallaxes.Length; i++)
        {
            parallaxes[i].transform.position = new Vector3(player.transform.position.x, parallaxes[i].transform.position.y, parallaxes[i].transform.position.z);
            //parallaxes[i].transform.GetChild(0).position = new Vector3(0, parallaxes[i].transform.GetChild(0).position.y, parallaxes[i].transform.GetChild(0).position.z);
            //parallaxes[i].Elements[0].GameObjects[0].transform.position = new Vector3(0, parallaxes[i].Elements[0].GameObjects[0].transform.position.y, parallaxes[i].Elements[0].GameObjects[0].transform.position.z);
            for (int j = 0; j < parallaxes[i].transform.childCount - 3; j++)
            {
                //parallaxes[i].transform.GetChild(j).position = new Vector3(0, parallaxes[i].transform.GetChild(j).position.y, parallaxes[i].transform.GetChild(j).position.z);
            }
        }*/

        //  Set up the cape helper
        CapePhysicsHelper capeHelper = FindObjectOfType<CapePhysicsHelper>();
        capeHelper.transform.position = player.transform.position;
        capeHelper.capeControlNode = GameObject.FindGameObjectWithTag(Tags.bone_Cape_CTRL).transform;
        capeHelper.GetComponent<DistanceJoint2D>().connectedBody = GameObject.FindGameObjectWithTag(Tags.bone_Cape).GetComponent<Rigidbody2D>();
    }

    #region Saving & loading
    public void SavePlayerData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(Application.persistentDataPath + "/playerSave1.dat"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/playerSave1.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);

            data.lastCheckpointID = CurrentCheckpointID;

            bf.Serialize(file, data);
            file.Close();
        }
        else
        {
            FileStream file = File.Create(Application.persistentDataPath + "/playerSave1.dat");
            PlayerData data = new PlayerData();

            data.lastCheckpointID = CurrentCheckpointID;

            bf.Serialize(file, data);
            file.Close();
        }
    }

    public void LoadPlayerData()
    {
        if (File.Exists(Application.persistentDataPath + "/playerSave1.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerSave1.dat", FileMode.Open);

            PlayerData data = (PlayerData)bf.Deserialize(file);

            CurrentCheckpointID = data.lastCheckpointID;
            Debug.Log(data.lastCheckpointID);
        }
        else
        {
            CurrentCheckpointID = 0;
        }
    }
    #endregion

    #region SortCheckpointsByQuickSort()
    private void SortCheckpointsByQuickSort(Checkpoint[] checkpoints, int left, int right)
    {
        int i = left, j = right;
        Checkpoint temp;
        Checkpoint pivot = checkpoints[(left + right) / 2];

        //	Partioning
        while (i <= j)
        {
            while (checkpoints[i].checkpointID < pivot.checkpointID)
                i++;
            while (checkpoints[j].checkpointID > pivot.checkpointID)
                j--;

            if (i <= j)
            {
                temp = checkpoints[i];
                checkpoints[i] = checkpoints[j];
                checkpoints[j] = temp;
                i++;
                j--;
            }
        };

        //	Recursion
        if (left < j)
            SortCheckpointsByQuickSort(checkpoints, left, j);
        if (i < right)
            SortCheckpointsByQuickSort(checkpoints, i, right);
    }
    #endregion
}
