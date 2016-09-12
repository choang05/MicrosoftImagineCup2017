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

    public WorldState CurrentWorldState;
    public enum WorldState { Present, Past, Future };

    [Space(10)]
    public bool canSwitch = true;
    public float transitionDuration = 1;

    private CameraTransition cameraTransition;

    void Awake()
    {
        //  Find and assign references
        cameraTransition = FindObjectOfType<CameraTransition>();
    }

    void Start()
    {
        //  Initial setups
        CurrentWorldState = WorldState.Present;

        //PastObjects.SetActive(false);
        //FutureObjects.SetActive(false);
    }

	// Update is called once per frame
	void Update ()
    {
        //  Evaluate input from player. 1-3 selects which world to transition to
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            SwitchWorld(1);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            SwitchWorld(2);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            SwitchWorld(3);
        }
    }
    
    public void SwitchWorld(int worldID)
    {
        //  Calculate which world and mask we need to unload
        GameObject UnloadingWorld;
        Camera currentWorld;
        if (CurrentWorldState == WorldState.Present)
        {
            UnloadingWorld = PresentObjects;
            currentWorld = PresentCamera;
        }
        else if (CurrentWorldState == WorldState.Past)
        {
            UnloadingWorld = PastObjects;
            currentWorld = PastCamera;
        }
        else 
        {
            UnloadingWorld = FutureObjects;
            currentWorld = FutureCamera;
        }

        //  If player is allowed to switch...
        if (canSwitch)
        {
            //  Determine which world ID to switch to and check if world is already active.
            if (worldID == 1 && CurrentWorldState != WorldState.Present)
            {
                CurrentWorldState = WorldState.Present;
                cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentWorld, PresentCamera, transitionDuration, new object[] { 0.05f, true });
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                PresentCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
                //PresentCamera. 
                //Debug.Log("Switched to Present");
            }
            else if (worldID == 2 && CurrentWorldState != WorldState.Past)
            {
                CurrentWorldState = WorldState.Past;
                cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentWorld, PastCamera, transitionDuration, new object[] { 0.05f, true });
                transform.position = new Vector3(transform.position.x, transform.position.y, 25);
                PastCamera.transform.position = new Vector3(transform.position.x, transform.position.y, 10);
                //Debug.Log("Switched to Past");
            }
            else if (worldID == 3 && CurrentWorldState != WorldState.Future)
            {
                CurrentWorldState = WorldState.Future;
                cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentWorld, FutureCamera, transitionDuration, new object[] { 0.05f, true });
                transform.position = new Vector3(transform.position.x, transform.position.y, 50);
                FutureCamera.transform.position = new Vector3(transform.position.x, transform.position.y, 35);
                //Debug.Log("Switched to Future");
            }
        }
    }
}
