using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;
using CameraTransitions;

public class WorldChanger : MonoBehaviour
{
    [Space(10)]
    public ProCamera2D PresentProCamera2D;
    public ProCamera2D PastProCamera2D;
    public ProCamera2D FutureProCamera2D;

    public WorldState currentWorldState;
    public enum WorldState { Present, Past, Future };

    [Space(10)]
    public bool isPresentAvaliable;
    public bool isPastAvaliable;
    public bool isFutureAvaliable;
    [HideInInspector] public bool canSwitchPresent = true;
    [HideInInspector] public bool canSwitchPast = true;
    [HideInInspector] public bool canSwitchFuture = true;
    public float transitionDuration;
    [Range(0, 1)] public float transitionEdgeSmoothness;

    public CameraTransition cameraTransition;
    private CharacterController charController;

    public delegate void WorldChangeEvent(WorldState worldState);
    public static event WorldChangeEvent OnWorldChanged;

    void Awake()
    {
        //  Find and assign references
        cameraTransition = FindObjectOfType<CameraTransition>();
    }

    void Start()
    {
        //  Initial setups
        currentWorldState = WorldState.Present;
    }

	// Update is called once per frame
	void Update ()
    {
        //  Determine which world player can be teleported too if there is open space
        CheckWorldCollisions();

        //  Ensure the player stays on the correct Z plane at all times
        switch (currentWorldState)
        {
            case WorldState.Present:
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                break;
            case WorldState.Past:
                transform.position = new Vector3(transform.position.x, transform.position.y, 25);
                break;
            case WorldState.Future:
                transform.position = new Vector3(transform.position.x, transform.position.y, 50);
                break;
        }

        //  If player is allowed to switch & a transition is currently not running...
        if (!cameraTransition.IsRunning)
        {
            //  Evaluate input from player. 1-3 selects which world to transition to
            if (Input.GetKeyUp(KeyCode.Alpha1) && currentWorldState != WorldState.Present && isPresentAvaliable && canSwitchPresent)
            {
                //  Broadcast event delegate
                if (OnWorldChanged != null)
                    OnWorldChanged(WorldState.Present);

                SwitchWorld(1); //  Present
            }
            else if (Input.GetKeyUp(KeyCode.Alpha2) && currentWorldState != WorldState.Past && isPastAvaliable && canSwitchPast)
            {
                //  Broadcast event delegate
                if (OnWorldChanged != null)
                    OnWorldChanged(WorldState.Past);

                SwitchWorld(2); //  Past
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3) && currentWorldState != WorldState.Future && isFutureAvaliable && canSwitchFuture) 
            {
                //  Broadcast event delegate
                if (OnWorldChanged != null)
                    OnWorldChanged(WorldState.Future);

                SwitchWorld(3); //  Future
            }
        }
    }

    #region SwitchWorld(): Switch world given an ID
    public void SwitchWorld(int worldID)
    {    
        //  Get the current world camera
        Camera currentCamera = GetCurrentWorldCamera();

        //  Disable audio listener
        currentCamera.GetComponent<AudioListener>().enabled = false;
          
        //  Determine which world ID to switch to and check if world is already active.
        if (worldID == 1)
        {
            //  Update world state
            currentWorldState = WorldState.Present;

            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, PresentProCamera2D.GameCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness });

            //  Set new Z position for player
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
        else if (worldID == 2)
        {
            //  Update world state
            currentWorldState = WorldState.Past;
            
            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, PastProCamera2D.GameCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness });

            //  Set new Z position for player
            transform.position = new Vector3(transform.position.x, transform.position.y, 25);
        }
        else if (worldID == 3)
        {
            //  Update world state
            currentWorldState = WorldState.Future;
            
            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, FutureProCamera2D.GameCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness });

            //  Set new Z position for player
            transform.position = new Vector3(transform.position.x, transform.position.y, 50);
        }

        //  Enable audio listener for new world
        GetCurrentWorldCamera().GetComponent<AudioListener>().enabled = true;
    }
    #endregion

    #region GetCurrentWorldCamera(): Get current world camera
    private Camera GetCurrentWorldCamera()
    {
        //  Get the current world and camera
        if (currentWorldState == WorldState.Present)
        {
            //UnloadingWorld = PresentObjects;
            return PresentProCamera2D.GameCamera;
        }
        else if (currentWorldState == WorldState.Past)
        {
            //UnloadingWorld = PastObjects;
            return PastProCamera2D.GameCamera;
        }
        else
        {
            //UnloadingWorld = FutureObjects;
            return FutureProCamera2D.GameCamera;
        }
    }
    #endregion

    #region CheckWorldCollisions(): Determine which world player can be teleported too if there is open space
    private void CheckWorldCollisions()
    {
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
        RaycastHit hit;
        Vector3 rayDir;

        //  If player is in Present, check collisions for Past and Future
        if (currentWorldState != WorldState.Present)
        {
            //  cast present ray & evaluate
            rayDir = new Vector3(playerPos.x, playerPos.y, -5);
            if (Physics.Raycast(rayDir, Vector3.forward * 10, out hit, 15))
            {
                canSwitchPresent = false;
                if (Application.isEditor) Debug.DrawRay(rayDir, Vector3.forward * 10, Color.red, 0.1f);
            }
            else
            {
                canSwitchPresent = true;
                if (Application.isEditor) Debug.DrawRay(rayDir, Vector3.forward * 10, Color.green, 0.1f);
            }
        }
        //  If player is in Past, check collisions for Present and Future
        if (currentWorldState != WorldState.Past)
        {
            //  cast past ray & evaluate
            rayDir = new Vector3(playerPos.x, playerPos.y, 20);
            if (Physics.Raycast(rayDir, Vector3.forward * 10, out hit, 15))
            {
                canSwitchPast = false;
                if (Application.isEditor) Debug.DrawRay(rayDir, Vector3.forward * 10, Color.red, 0.1f);
            }
            else
            {
                canSwitchPast = true;
                if (Application.isEditor) Debug.DrawRay(rayDir, Vector3.forward * 10, Color.green, 0.1f);
            }
        }
        //  If player is in Future, check collisions for Present and Past
        if (currentWorldState != WorldState.Future)
        {
            //  cast future ray & evaluate
            rayDir = new Vector3(playerPos.x, playerPos.y, 45);
            if (Physics.Raycast(rayDir, Vector3.forward * 10, out hit, 15))
            {
                canSwitchFuture = false;
                if (Application.isEditor) Debug.DrawRay(rayDir, Vector3.forward * 10, Color.red, 0.1f);
            }
            else
            {
                canSwitchFuture = true;
                if (Application.isEditor) Debug.DrawRay(rayDir, Vector3.forward * 10, Color.green, 0.1f);
            }
        }
    }
    #endregion
}
