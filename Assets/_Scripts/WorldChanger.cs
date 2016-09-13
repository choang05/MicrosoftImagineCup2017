using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using CameraTransitions;

public class WorldChanger : MonoBehaviour
{
    [Space(10)]
    public GameObject PresentObjects;
    public GameObject PastObjects;
    public GameObject FutureObjects;

    [Space(10)]
    public Camera PresentCamera;
    public Camera PastCamera;
    public Camera FutureCamera;

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

        PastCamera.gameObject.SetActive(false);
        FutureCamera.gameObject.SetActive(false);
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
    
    public void SwitchWorld(int worldID)
    {
        //  Get the current world and camera
        GameObject UnloadingWorld;
        Camera currentCamera;
        if (currentWorldState == WorldState.Present)
        {
            UnloadingWorld = PresentObjects;
            currentCamera = PresentCamera;
        }
        else if (currentWorldState == WorldState.Past)
        {
            UnloadingWorld = PastObjects;
            currentCamera = PastCamera;
        }
        else 
        {
            UnloadingWorld = FutureObjects;
            currentCamera = FutureCamera;
        }

        //  Determine which world ID to switch to and check if world is already active.
        if (worldID == 1 && currentWorldState != WorldState.Present)
        {
            //  Update world state
            currentWorldState = WorldState.Present;
            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, PresentCamera, transitionDuration, new object[] { transitionEdgeSmoothness, true });
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            PresentCamera.transform.position = new Vector3(currentCamera.transform.position.x, currentCamera.transform.position.y, -10);
            //PresentCamera. 
            //Debug.Log("Switched to Present");
        }
        else if (worldID == 2 && currentWorldState != WorldState.Past)
        {
            //  Update world state
            currentWorldState = WorldState.Past;
            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, PastCamera, transitionDuration, new object[] { transitionEdgeSmoothness, true });
            transform.position = new Vector3(transform.position.x, transform.position.y, 25);
            PastCamera.transform.position = new Vector3(currentCamera.transform.position.x, currentCamera.transform.position.y, 10);
            //Debug.Log("Switched to Past");
        }
        else if (worldID == 3 && currentWorldState != WorldState.Future)
        {
            //  Update world state
            currentWorldState = WorldState.Future;
            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, FutureCamera, transitionDuration, new object[] { transitionEdgeSmoothness, true });
            transform.position = new Vector3(transform.position.x, transform.position.y, 50);
            FutureCamera.transform.position = new Vector3(currentCamera.transform.position.x, currentCamera.transform.position.y, 35);
            //Debug.Log("Switched to Future");
        }
    }
}
