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
        SceneManager.sceneLoaded += SetUpPlayer;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= SetUpPlayer;
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
        
    public void Respawn()
    {
        Checkpoints.Clear();

        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private void SetUpPlayer(Scene scene, LoadSceneMode mode)
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
        camera.AddCameraTarget(player.transform);
        camera.GetComponent<ProCamera2DRails>().AddRailsTarget(player.transform);
        camera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, camera.transform.position.z);

        //  Set up world changer
        FindObjectOfType<WorldChanger>().charController = player.GetComponent<CharacterController2D>();

        //  Set up the cape helper
        CapePhysicsHelper capeHelper = FindObjectOfType<CapePhysicsHelper>();
        capeHelper.transform.position = player.transform.position;
        capeHelper.capeControlNode = GameObject.FindGameObjectWithTag(Tags.bone_Cape_CTRL).transform;
        capeHelper.GetComponent<DistanceJoint2D>().connectedBody = GameObject.FindGameObjectWithTag(Tags.bone_Cape).GetComponent<Rigidbody2D>();
    }
}
