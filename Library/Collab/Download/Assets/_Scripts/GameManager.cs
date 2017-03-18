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
    //  Static instance of object to do not destroy on load
    private static GameManager control;

    //  User Parameters variables
    //public ProCamera2D MainCamera;
    public GameObject playerPrefab;
    public static int CurrentCheckpointID;

    //  NEEDS RESETS before loading new scene
    public bool hasPresentWisp = false;
    public bool hasPastWisp = false;
    public bool hasFutureWisp = false;

    //  Private
    [HideInInspector] public Checkpoint[] Checkpoints;
    public static string SaveLoadFileName = "LocalSave" + ".dat";
    private AccountManager accountManager;
    private AsyncOperation async;

    //  Events
    public delegate void GameManagerEvent();
    public static event GameManagerEvent OnSave;
    public static event GameManagerEvent OnPlayerSpawned;

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

        accountManager = FindObjectOfType<AccountManager>();
    }

    //  Function to unload a level segment given its checkpoint ID
    public void UnloadLevelSegment(int checkpointIndex)
    {
        Checkpoints[checkpointIndex].SegmentGO.SetActive(false);
        //if (Application.isEditor) Debug.Log("Checkpoint " + Checkpoints[checkpointIndex].checkpointID + " unloaded." + " Index: " + checkpointIndex);
    }

    //  Function to load in a level segment given its checkpoint ID
    public void LoadLevelSegment(int checkpointIndex)
    {
        Checkpoints[checkpointIndex].SegmentGO.SetActive(true);
        //if (Application.isEditor) Debug.Log("Checkpoint " + Checkpoints[checkpointIndex].checkpointID + " loaded." + " Index: " + checkpointIndex);
    }

    //  Public function to start the coroutine
    public void RespawnPlayer()
    {   StartCoroutine(CoRespawn());    }

    //  Coroutine that handles respawning in timely manner
    private IEnumerator CoRespawn()
    {
        //  Async scene loading so loading time is lower.
        async = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        WorldChanger worldChanger = FindObjectOfType<WorldChanger>();

        //  Stop player controlling
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.isControllable = false;
        playerController.animator.SetFloat(PlayerController.xVelocityHash, 0);
        playerController.velocity.x = 0;

        TransitionCameraExit();

        yield return new WaitForSeconds(worldChanger.MainCamera.GetComponent<ProCamera2DTransitionsFX>().DurationExit);

        //  Load in scene when its allowed
        async.allowSceneActivation = true;
        //SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    //  Sets up the scene on master scene loaded
    private void SetUpScene(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("Scene loaded: current checkpoint: " + CurrentCheckpointID);
        //  If the scene loaded was not the master scene, then do nothing.
        if (scene.buildIndex != 1)
            return;

        //  Reset avliable timelines
        hasPresentWisp = false;
        hasPastWisp = false;
        hasFutureWisp = false;

        //  Fetch & Cache all the checkpoints, then sort by their checkpoint IDs
        Checkpoints = Resources.FindObjectsOfTypeAll(typeof(Checkpoint)) as Checkpoint[];
        SortCheckpointsByQuickSort(Checkpoints, 0, Checkpoints.Length-1);
        //for (int i = 0; i < Checkpoints.Length; i++)
        //    Debug.Log(Checkpoints[i].checkpointID);

        //  Turn off all level segments as a clean slate
        for (int i = 0; i < Checkpoints.Length; i++)
            Checkpoints[i].SegmentGO.SetActive(false);

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
                    Checkpoints[i - 1].SegmentGO.SetActive(true);

                //  Load in the level of the checkpoint ID
                Checkpoints[i].SegmentGO.SetActive(true);

                //  Load the level segment after the checkpoint ID
                if (i + 1 < Checkpoints.Length)
                    Checkpoints[i + 1].SegmentGO.SetActive(true);

                break;
            }
        }

        //  Instantiate the player at the checkpoint location
        GameObject player = Instantiate(playerPrefab, currentCheckpointPosition, Quaternion.identity) as GameObject;

        //  Event
        if (OnPlayerSpawned != null)
            OnPlayerSpawned();

        //  Set up camera
        ProCamera2D proCamera2D = ProCamera2D.Instance;
        proCamera2D.AddCameraTarget(player.transform);
        //  camera railing
        ProCamera2DRails proCamera2DRails = proCamera2D.GetComponent<ProCamera2DRails>();
        if (proCamera2DRails != null)
        {
            proCamera2DRails.AddRailsTarget(player.transform);
            proCamera2D.CameraTargets[0].TargetInfluenceH = 0.5f;
            proCamera2D.CameraTargets[0].TargetInfluenceV = 0.5f;
        }
        //  move instant so theres no late follow upon loading.
        proCamera2D.MoveCameraInstantlyToPosition(new Vector2(player.transform.position.x, player.transform.position.y));

        //  Transition Enter
        TransitionCameraEnter();
    }

    //  Do effect when player enters scene
    public void TransitionCameraEnter()
    {
        FindObjectOfType<ProCamera2DTransitionsFX>().TransitionEnter();
    }

    //  Do effect when player exits the scene
    public void TransitionCameraExit()
    {
        FindObjectOfType<ProCamera2DTransitionsFX>().TransitionExit();
    }

    #region GetCurrentCheckpointIndex()
    public int GetCurrentCheckpointIndex(int checkpointID)
    {
        for (int i = 0; i < Checkpoints.Length; i++)
        {
            if (Checkpoints[i].checkpointID == checkpointID)
            {
                return i;
            }
        }

        return -1;
    }
    #endregion

    #region Saving & loading
    public void SavePlayerData()
    {
        //  Evaluate whether to save locally or online if logged in
        if (AccountManager.IsLoggedIn)
        {
            AccountManager.CurrentUser.checkpointID = CurrentCheckpointID;
            accountManager.UpdateDatabaseUser();
        }
        else
        {
            if (DoesLocalSaveExist())
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(Application.persistentDataPath + "/" + SaveLoadFileName);
                //FileStream file = File.Open(Application.persistentDataPath + "/" + SaveLoadFileName, FileMode.Open);
                //PlayerData data = (PlayerData)bf.Deserialize(file);
                PlayerData data = new PlayerData();

                data.lastCheckpointID = CurrentCheckpointID;
                //Debug.Log("checkpoint file after save: " + data.lastCheckpointID);

                bf.Serialize(file, data);
                file.Close();
            }
            else
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(Application.persistentDataPath + "/" + SaveLoadFileName);
                PlayerData data = new PlayerData();

                data.lastCheckpointID = CurrentCheckpointID;

                bf.Serialize(file, data);
                file.Close();
            }
        }
    }

    public void LoadPlayerData()
    {
        //  Evaluate whether to save locally or online if logged in
        if (AccountManager.IsLoggedIn)
        {
            CurrentCheckpointID = AccountManager.CurrentUser.checkpointID;
        }
        else
        {
            if (DoesLocalSaveExist())
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/" + SaveLoadFileName, FileMode.Open);

                PlayerData data = (PlayerData)bf.Deserialize(file);
                file.Close();
                //Debug.Log("checkpoint before load: " + CurrentCheckpointID);
                //Debug.Log("file checkpoint before load: " + data.lastCheckpointID);
                CurrentCheckpointID = data.lastCheckpointID;
                //Debug.Log("checkpoint after load: " + CurrentCheckpointID);
                //Debug.Log("file checkpoint after load: " + data.lastCheckpointID);


                //Debug.Log(data.lastCheckpointID);
            }
            else
            {
                CurrentCheckpointID = 0;
            }
        }
    }
    #endregion

    public static bool DoesLocalSaveExist()
    {
        return File.Exists(Application.persistentDataPath + "/" + SaveLoadFileName);
    }

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
