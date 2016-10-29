using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Com.LuisPedroFonseca.ProCamera2D;

public class GameManager : MonoBehaviour
{
    //  Static instance of object to dnot destroy on load
    private static GameManager control;

    //  User Parameters variables
    public GameObject playerPrefab;
    public List<Checkpoint> Checkpoints = new List<Checkpoint>();
    public int CurrentAreaID;

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

    //  Public function to start the coroutine
    public void StartCoRespawn()
    {   StartCoroutine(CoRespawn());    }

    //  Coroutine that handles respawning in timly manner
    private IEnumerator CoRespawn()
    {
        WorldChanger worldChanger = FindObjectOfType<WorldChanger>();
        worldChanger.TransitionCameraExit();

        yield return new WaitForSeconds(worldChanger.MainCamera.GetComponent<ProCamera2DTransitionsFX>().DurationExit);

        Checkpoints.Clear();
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    //  Sets up the scene on master scene loaded
    private void SetUpScene(Scene scene, LoadSceneMode mode)
    {
        //  If the scene loaded was not the master scene, then do nothing.
        if (scene.buildIndex != 0)
            return;

        //  Find the current areaID the player is in
        Vector3 currentCheckpointPosition = Vector3.zero;
        for (int i = 0; i < Checkpoints.Count; i++)
            if (Checkpoints[i].AreaID == CurrentAreaID)
                currentCheckpointPosition = Checkpoints[i].transform.position;    

        //  Instaniate the player at the checkpoint location
        GameObject player = Instantiate(playerPrefab, currentCheckpointPosition, Quaternion.identity) as GameObject;

        //  Set up camera
        ProCamera2D camera = ProCamera2D.Instance;
        camera.MoveCameraInstantlyToPosition(new Vector2(player.transform.position.x, player.transform.position.y));
        camera.AddCameraTarget(player.transform);
        camera.GetComponent<ProCamera2DRails>().AddRailsTarget(player.transform);

        //  Set up world changer
        WorldChanger worldChanger = FindObjectOfType<WorldChanger>();
        worldChanger.charController = player.GetComponent<CharacterController2D>();
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
}
