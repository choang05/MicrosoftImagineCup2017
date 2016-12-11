using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;
using CameraTransitions;

public class WorldChanger : MonoBehaviour
{
    [Space(10)]
    public ProCamera2D MainCamera;
    public Camera PresentCamera;
    public Camera PastCamera;
    public Camera FutureCamera;

    public WorldState currentWorldState;
    public enum WorldState { Present, Past, Future };

    [Space(10)]
    public bool isPresentWorldAvaliable;
    public bool isPastWorldAvaliable;
    public bool isFutureWorldAvaliable;
    [HideInInspector] public bool canSwitchPresent = true;
    [HideInInspector] public bool canSwitchPast = true;
    [HideInInspector] public bool canSwitchFuture = true;
    public float transitionDuration;
    [Range(0, 1)] public float transitionEdgeSmoothness;
    private LayerMask originalLayer;

    public CameraTransition cameraTransition;
    [HideInInspector] public CharacterController2D charController2D;
    private bool isCurrentlyTransitioning = false;
    
    //  Events
    public delegate void WorldChangeEvent(WorldState worldState);
    public static event WorldChangeEvent OnWorldChangeStart;
    public static event WorldChangeEvent OnWorldChangeComplete;

    void Awake()
    {
        //  Find and assign references
        cameraTransition = FindObjectOfType<CameraTransition>();
    }

    void Start()
    {
        //  Initial setups
        currentWorldState = WorldState.Present;
        originalLayer = gameObject.layer;
    }

	// Update is called once per frame
	void Update ()
    {
        //  If character hasn't been spawned yet, do nothing
        if (charController2D == null)
            return;
        
        //  Determine which world player can be teleported too if there is open space
        CheckWorldCollisions();

        //  Ensure the player stays on the correct Z plane at all times
        switch (currentWorldState)
        {
            case WorldState.Present:
                charController2D.transform.position = new Vector3(charController2D.transform.position.x, charController2D.transform.position.y, 0);
                break;
            case WorldState.Past:
                charController2D.transform.position = new Vector3(charController2D.transform.position.x, charController2D.transform.position.y, 50);
                break;
            case WorldState.Future:
                charController2D.transform.position = new Vector3(charController2D.transform.position.x, charController2D.transform.position.y, 100);
                break;
        }

        //  If player is allowed to switch & a transition is currently not running...
        if (!isCurrentlyTransitioning)
        {
            //  If the player is in any of these states, do not allow switching
            if (charController2D.currentState == CharacterController2D.PlayerState.ClimbingLadder
                || charController2D.currentState == CharacterController2D.PlayerState.ClimbingRope
                || charController2D.currentState == CharacterController2D.PlayerState.ClimbingLedge)
            {
                if (currentWorldState == WorldState.Present)
                {
                    canSwitchPast = false;
                    canSwitchFuture = false;
                }
                else if (currentWorldState == WorldState.Past)
                {
                    canSwitchPresent = false;
                    canSwitchFuture = false;
                }
                else if (currentWorldState == WorldState.Future)
                {
                    canSwitchPresent = false;
                    canSwitchPast = false;
                }
            }

            //  Evaluate input from player. 1-3 selects which world to transition to
            if (Input.GetKeyUp(KeyCode.Alpha2) && currentWorldState != WorldState.Present && isPresentWorldAvaliable && canSwitchPresent)
            {
                //  Broadcast event delegate
                if (OnWorldChangeStart != null)
                    OnWorldChangeStart(WorldState.Present);

                SwitchWorld(1); //  Present
            }
            else if (Input.GetKeyUp(KeyCode.Alpha1) && currentWorldState != WorldState.Past && isPastWorldAvaliable && canSwitchPast)
            {
                //  Broadcast event delegate
                if (OnWorldChangeStart != null)
                    OnWorldChangeStart(WorldState.Past);

                SwitchWorld(2); //  Past
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3) && currentWorldState != WorldState.Future && isFutureWorldAvaliable && canSwitchFuture) 
            {
                //  Broadcast event delegate
                if (OnWorldChangeStart != null)
                    OnWorldChangeStart(WorldState.Future);

                SwitchWorld(3); //  Future
            }
        }
    }

    //  Do effect when player enters scene
    public void TransitionCameraEnter()
    {
        MainCamera.GetComponent<ProCamera2DTransitionsFX>().TransitionEnter();
    }

    //  Do effect when player exits the scene
    public void TransitionCameraExit()
    {
        MainCamera.GetComponent<ProCamera2DTransitionsFX>().TransitionExit();
    }

    #region SwitchWorld(): Switch world given an ID
    public void SwitchWorld(int worldID)
    {    
        //  Get the current world camera
        Camera currentCamera = GetCurrentWorldCamera();

        isCurrentlyTransitioning = true;

        //  Update the layer
        Layers.ChangeLayers(charController2D.gameObject, Layers.ViewAlways);

        //  Cache the player's position in normalized screen space coordinates.
        Vector2 transitionCenter = currentCamera.WorldToViewportPoint(charController2D.transform.position);

        //  Determine which world ID to switch to and check if world is already active.
        if (worldID == 1)
        {
            //  Update world state
            currentWorldState = WorldState.Present;

            //  Update the parallax cameras
            UpdateParallaxes(currentWorldState);

            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, PresentCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness, transitionCenter });
        }
        else if (worldID == 2)
        {
            //  Update world state
            currentWorldState = WorldState.Past;

            //  Update the parallax cameras
            UpdateParallaxes(currentWorldState);

            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, PastCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness, transitionCenter });
        }
        else if (worldID == 3)
        {
            //  Update world state
            currentWorldState = WorldState.Future;

            //  Update the parallax cameras
            UpdateParallaxes(currentWorldState);

            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, FutureCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness, transitionCenter });
        }
    }
    #endregion

    #region GetCurrentWorldCamera(): Get current world camera
    private Camera GetCurrentWorldCamera()
    {
        //  Get the current world and camera
        if (currentWorldState == WorldState.Present)
        {
            return PresentCamera;
        }
        else if (currentWorldState == WorldState.Past)
        {
            return PastCamera;
        }
        else
        {
            return FutureCamera;
        }
    }
    #endregion

    #region CheckWorldCollisions(): Determine which world player can be teleported too if there is open space
    private void CheckWorldCollisions()
    {
        //float distanceFromCenter = charController2D.charController.height / 2 - charController2D.charController.radius;
        float distanceFromCenter = charController2D.charController.height / 10.5f;

        Vector3 topCenterOfCapsule = charController2D.transform.position + charController2D.charController.center + Vector3.up * distanceFromCenter;
        Vector3 bottomCenterOfCapsule = charController2D.transform.position + charController2D.charController.center - Vector3.up * distanceFromCenter;

        float castRadius = charController2D.charController.radius * 0.95f;

        //  If player is not in the Present, check collisions for the Present
        if (currentWorldState != WorldState.Present)
        {
            //  cast present capsule collider & evaluate
            topCenterOfCapsule = new Vector3(topCenterOfCapsule.x, topCenterOfCapsule.y, 0);
            bottomCenterOfCapsule = new Vector3(bottomCenterOfCapsule.x, bottomCenterOfCapsule.y, 0);
            if (Physics.CheckCapsule(topCenterOfCapsule, bottomCenterOfCapsule, castRadius))
            {
                canSwitchPresent = false;
            }
            else
            {
                canSwitchPresent = true;
            }
        }
        //  If player is not in the past, check collisions for the past
        if (currentWorldState != WorldState.Past)
        {
            //  cast past capsule collider & evaluate
            topCenterOfCapsule = new Vector3(topCenterOfCapsule.x, topCenterOfCapsule.y, 50);
            bottomCenterOfCapsule = new Vector3(bottomCenterOfCapsule.x, bottomCenterOfCapsule.y, 50);
            if (Physics.CheckCapsule(topCenterOfCapsule, bottomCenterOfCapsule, castRadius))
            {
                canSwitchPast = false;
            }
            else
            {
                canSwitchPast = true;
            }
        }
        //  If player is not in the Future, check collisions for the Future
        if (currentWorldState != WorldState.Future)
        {
            //  cast future capsule collider & evaluate
            topCenterOfCapsule = new Vector3(topCenterOfCapsule.x, topCenterOfCapsule.y, 100);
            bottomCenterOfCapsule = new Vector3(bottomCenterOfCapsule.x, bottomCenterOfCapsule.y, 100);
            if (Physics.CheckCapsule(topCenterOfCapsule, bottomCenterOfCapsule, castRadius))
            {
                canSwitchFuture = false;
            }
            else
            {
                canSwitchFuture = true;
            }
        }
    }
    #endregion

    public void BroadcastTransitionCompleteEvent()
    {
        if (OnWorldChangeComplete != null)
            OnWorldChangeComplete(currentWorldState);

        isCurrentlyTransitioning = false;

        //  Update the layer
        Layers.ChangeLayers(charController2D.gameObject, originalLayer);
    }

    #region UpdateParallaxes()
    private void UpdateParallaxes(WorldState currentWorldState)
    {
        EZParallax[] EZparallaxes = FindObjectsOfType<EZParallax>();

        switch (currentWorldState)
        {
            case WorldState.Present:
                for (int i = 0; i < EZparallaxes.Length; i++)
                    EZparallaxes[i].m_mainCamera = PresentCamera.gameObject;
                break;
            case WorldState.Past:
                for (int i = 0; i < EZparallaxes.Length; i++)
                    EZparallaxes[i].m_mainCamera = PastCamera.gameObject;
                break;
            case WorldState.Future:
                for (int i = 0; i < EZparallaxes.Length; i++)
                    EZparallaxes[i].m_mainCamera = FutureCamera.gameObject;
                break;
        }
    }
    #endregion
}
