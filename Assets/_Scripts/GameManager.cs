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
    public GameObject playerObject;
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

    void Start()
    {
        //SetUpPlayer(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }
        
    public void Respawn()
    {
        Checkpoints.Clear();

        Debug.Log(Checkpoints.Count);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private void SetUpPlayer(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(Checkpoints.Count);

        Vector3 currentCheckpointPosition = Vector3.zero;
        //  Find the current areaID the player is in
        for (int i = 0; i < Checkpoints.Count; i++)
        {
            if (Checkpoints[i].AreaID == CurrentAreaID)
            {
                currentCheckpointPosition = Checkpoints[i].transform.position;    
            }
        }

        //  Instaniate the player at the checkpoint location
        GameObject player = Instantiate(playerObject, currentCheckpointPosition, Quaternion.identity) as GameObject;

        //  Add to camera
        ProCamera2D.Instance.AddCameraTarget(player.transform);

        //  Set up the cape helper
        CapePhysicsHelper capeHelper = FindObjectOfType<CapePhysicsHelper>();
        capeHelper.transform.position = player.transform.position;
        capeHelper.capeControlNode = GameObject.FindGameObjectWithTag(Tags.bone_Cape_CTRL).transform;
        capeHelper.GetComponent<DistanceJoint2D>().connectedBody = GameObject.FindGameObjectWithTag(Tags.bone_Cape).GetComponent<Rigidbody2D>();
    }
}
