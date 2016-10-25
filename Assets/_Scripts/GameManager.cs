using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Com.LuisPedroFonseca.ProCamera2D;

public class GameManager : MonoBehaviour
{
    //  Static instance of object to dnot destroy on load
    private static GameManager control;

    //  User Parameters variables
    public GameObject playerObject;
    public Transform LatestCheckpoint;

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
        SetUpPlayer();
    }

    public void Respawn()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);

        //  Respawn player at GameManager's respawn node
        SetUpPlayer();
    }

    private void SetUpPlayer()
    {
        GameObject player = Instantiate(playerObject, LatestCheckpoint.position, Quaternion.identity) as GameObject;

        //  Add to camera
        ProCamera2D.Instance.AddCameraTarget(player.transform);

        //  Assign the cape helper
        CapePhysicsHelper capeHelper = FindObjectOfType<CapePhysicsHelper>();
        capeHelper.capeControlNode = GameObject.FindGameObjectWithTag(Tags.bone_Cape_CTRL).transform;
        capeHelper.GetComponent<DistanceJoint2D>().connectedBody = GameObject.FindGameObjectWithTag(Tags.bone_Cape).GetComponent<Rigidbody2D>();
    }
}
