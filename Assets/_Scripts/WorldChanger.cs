using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;
using CameraTransitions;

public class WorldChanger : MonoBehaviour
{
    [Space(10)]
    //public GameObject PresentObjects;
    //public GameObject PastObjects;
    //public GameObject FutureObjects;

    [Space(10)]
    public ProCamera2D PresentProCamera2D;
    public ProCamera2D PastProCamera2D;
    public ProCamera2D FutureProCamera2D;

    public WorldState currentWorldState;
    public enum WorldState { Present, Past, Future };

    [Space(10)]
    public bool canSwitchPresent = true;
    public bool canSwitchPast = true;
    public bool canSwitchFuture = true;
    public float transitionDuration;
    [Range(0, 1)] public float transitionEdgeSmoothness;

    private CameraTransition cameraTransition;

    void Awake()
    {
        //  Find and assign references
        cameraTransition = FindObjectOfType<CameraTransition>();
    }

    void Start()
    {
        //  Initial setups
        currentWorldState = WorldState.Present;

        PastProCamera2D.gameObject.SetActive(false);
        FutureProCamera2D.gameObject.SetActive(false);
    }

	// Update is called once per frame
	void Update ()
    {
        //  If player is allowed to switch & a transition is currently not running...
        if (!cameraTransition.IsRunning)
        {
            //  Evaluate input from player. 1-3 selects which world to transition to
            if (Input.GetKeyUp(KeyCode.Alpha1) && canSwitchPresent)
            {
                SwitchWorld(1); //  Present
            }
            else if (Input.GetKeyUp(KeyCode.Alpha2) && canSwitchPast)
            {
                SwitchWorld(2); //  Past
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3) && canSwitchFuture) 
            {
                SwitchWorld(3); //  Future
            }
        }
    }

    #region Switch World
    public void SwitchWorld(int worldID)
    {
        //  Get the current world camera
        Camera currentCamera = GetCurrentWorldCamera();

        //  Cache player's X,Y position
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);

        //  Determine which world ID to switch to and check if world is already active.
        if (worldID == 1 && currentWorldState != WorldState.Present)
        {
            //  Update world state
            currentWorldState = WorldState.Present;
            //  Perform transition
            //PresentProCamera2D.gameObject.SetActive(true);
            PresentProCamera2D.MoveCameraInstantlyToPosition(playerPos);
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, PresentProCamera2D.GameCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness });
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            //PresentCamera.transform.position = new Vector3(currentCamera.transform.position.x, currentCamera.transform.position.y, -10);
            //PresentCamera. 
            //Debug.Log("Switched to Present");
        }
        else if (worldID == 2 && currentWorldState != WorldState.Past)
        {
            //  Update world state
            currentWorldState = WorldState.Past;
            //  Perform transition
            //PastProCamera2D.gameObject.SetActive(true);
            PastProCamera2D.MoveCameraInstantlyToPosition(playerPos);
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, PastProCamera2D.GameCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness });
            transform.position = new Vector3(transform.position.x, transform.position.y, 25);
            //PastCamera.transform.position = new Vector3(currentCamera.transform.position.x, currentCamera.transform.position.y, 10);
            //Debug.Log("Switched to Past");
        }
        else if (worldID == 3 && currentWorldState != WorldState.Future)
        {
            //  Update world state
            currentWorldState = WorldState.Future;
            //  Perform transition
            //FutureProCamera2D.gameObject.SetActive(true);
            FutureProCamera2D.MoveCameraInstantlyToPosition(playerPos);
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, FutureProCamera2D.GameCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness });
            transform.position = new Vector3(transform.position.x, transform.position.y, 50);
            //FutureCamera.transform.position = new Vector3(currentCamera.transform.position.x, currentCamera.transform.position.y, 35);
            //Debug.Log("Switched to Future");
        }
    }
    #endregion

    #region Get current world camera
    private Camera GetCurrentWorldCamera()
    {
        //  Get the current world and camera
        //GameObject UnloadingWorld;
        Camera currentCamera;
        if (currentWorldState == WorldState.Present)
        {
            //UnloadingWorld = PresentObjects;
            currentCamera = PresentProCamera2D.GameCamera;
        }
        else if (currentWorldState == WorldState.Past)
        {
            //UnloadingWorld = PastObjects;
            currentCamera = PastProCamera2D.GameCamera;
        }
        else
        {
            //UnloadingWorld = FutureObjects;
            currentCamera = FutureProCamera2D.GameCamera;
        }
        return currentCamera;
    }
    #endregion
}
