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

    public static WorldState CurrentWorldState;
    public enum WorldState { Present, Past, Future };

    [Space(10)]
    public bool isPresentWorldAvaliable;
    public bool isPastWorldAvaliable;
    public bool isFutureWorldAvaliable;
    [HideInInspector] public bool canSwitchPresent = true;
    [HideInInspector] public bool canSwitchPast = true;
    [HideInInspector] public bool canSwitchFuture = true;

    [Space(10)]
    public float transitionDuration;
    [Range(0, 1)] public float transitionEdgeSmoothness;
    private LayerMask originalLayer;

    public CameraTransition cameraTransition;
    [HideInInspector] public PlayerController playerController;
    private bool isCurrentlyTransitioning = false;
    
    //  Events
    public delegate void WorldChangeEvent(WorldState worldState);
    public static event WorldChangeEvent OnWorldChangeStart;
    public static event WorldChangeEvent OnWorldChangeComplete;

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += OnPlayerSpawned;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= OnPlayerSpawned;
    }

    void Awake()
    {
        //  Find and assign references
        cameraTransition = FindObjectOfType<CameraTransition>();
    }

    void Start()
    {
        //  Initial setups
        CurrentWorldState = WorldState.Present;
        originalLayer = gameObject.layer;
    }

	// Update is called once per frame
	void Update ()
    {
        //  If character hasn't been spawned yet, do nothing
        if (playerController == null)
            return;
        
        //  Determine which world player can be teleported too if there is open space
        CheckWorldCollisions();

        //  Ensure the player stays on the correct Z plane at all times
        switch (CurrentWorldState)
        {
            case WorldState.Present:
                playerController.transform.position = new Vector3(playerController.transform.position.x, playerController.transform.position.y, 0);
                break;
            case WorldState.Past:
                playerController.transform.position = new Vector3(playerController.transform.position.x, playerController.transform.position.y, 50);
                break;
            case WorldState.Future:
                playerController.transform.position = new Vector3(playerController.transform.position.x, playerController.transform.position.y, 100);
                break;
        }

        //  If player is allowed to switch & a transition is currently not running...
        if (!isCurrentlyTransitioning && PlayerDeath.isPlayerDead == false)
        { 
            //  If the player is in any of these states, do not allow switching
            if (playerController.currentState == PlayerController.PlayerState.ClimbingLadder
                || playerController.currentState == PlayerController.PlayerState.ClimbingRope
                || playerController.currentState == PlayerController.PlayerState.ClimbingLedge)
            {
                if (CurrentWorldState == WorldState.Present)
                {
                    canSwitchPast = false;
                    canSwitchFuture = false;
                }
                else if (CurrentWorldState == WorldState.Past)
                {
                    canSwitchPresent = false;
                    canSwitchFuture = false;
                }
                else if (CurrentWorldState == WorldState.Future)
                {
                    canSwitchPresent = false;
                    canSwitchPast = false;
                }
            }

            //  Evaluate input from player. 1-3 selects which world to transition to
            if (Input.GetButtonDown("WarpPresent") && CurrentWorldState != WorldState.Present && isPresentWorldAvaliable && canSwitchPresent)
            {
                //  Broadcast event delegate
                if (OnWorldChangeStart != null)
                    OnWorldChangeStart(WorldState.Present);

                SwitchWorld(1); //  Present
            }
            else if (Input.GetButtonDown("WarpPast") && CurrentWorldState != WorldState.Past && isPastWorldAvaliable && canSwitchPast)
            {
                //  Broadcast event delegate
                if (OnWorldChangeStart != null)
                    OnWorldChangeStart(WorldState.Past);

                SwitchWorld(2); //  Past
            }
            else if (Input.GetButtonDown("WarpFuture") && CurrentWorldState != WorldState.Future && isFutureWorldAvaliable && canSwitchFuture) 
            {
                //  Broadcast event delegate
                if (OnWorldChangeStart != null)
                    OnWorldChangeStart(WorldState.Future);

                SwitchWorld(3); //  Future
            }
        }
    }

    #region SwitchWorld(): Switch world given an ID
    public void SwitchWorld(int worldID)
    {    
        //  Get the current world camera
        Camera currentCamera = GetCurrentWorldCamera();

        isCurrentlyTransitioning = true;

        //  Update the layer
        Layers.ChangeLayers(playerController.gameObject, Layers.ViewAlways);

        //  Cache the player's position in normalized screen space coordinates.
        Vector2 transitionCenter = currentCamera.WorldToViewportPoint(playerController.transform.position);

        //  Determine which world ID to switch to and check if world is already active.
        if (worldID == 1)
        {
            //  Update world state
            CurrentWorldState = WorldState.Present;

            //  Update the parallax cameras
            UpdateParallaxes(CurrentWorldState);

            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, PresentCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness, transitionCenter });
        }
        else if (worldID == 2)
        {
            //  Update world state
            CurrentWorldState = WorldState.Past;

            //  Update the parallax cameras
            UpdateParallaxes(CurrentWorldState);

            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, PastCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness, transitionCenter });
        }
        else if (worldID == 3)
        {
            //  Update world state
            CurrentWorldState = WorldState.Future;

            //  Update the parallax cameras
            UpdateParallaxes(CurrentWorldState);

            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, FutureCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness, transitionCenter });
        }
    }
    #endregion

    #region GetCurrentWorldCamera(): Get current world camera
    private Camera GetCurrentWorldCamera()
    {
        //  Get the current world and camera
        if (CurrentWorldState == WorldState.Present)
        {
            return PresentCamera;
        }
        else if (CurrentWorldState == WorldState.Past)
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
        float distanceFromCenter = playerController.charController.height / 11.5f;
        //float distanceFromCenter = playerController.capCollider.height / 11.5f;

        Vector3 topCenterOfCapsule = playerController.transform.position + playerController.charController.center + Vector3.up * distanceFromCenter;
        Vector3 bottomCenterOfCapsule = playerController.transform.position + playerController.charController.center - Vector3.up * distanceFromCenter;
        //Vector3 topCenterOfCapsule = playerController.transform.position + playerController.capCollider.center + Vector3.up * distanceFromCenter;
        //Vector3 bottomCenterOfCapsule = playerController.transform.position + playerController.capCollider.center - Vector3.up * distanceFromCenter;

        float castRadius = playerController.charController.radius * 0.95f;
        //float castRadius = playerController.capCollider.radius * 0.95f;

        //  If player is not in the Present, check collisions for the Present
        if (CurrentWorldState != WorldState.Present)
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
        if (CurrentWorldState != WorldState.Past)
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
        if (CurrentWorldState != WorldState.Future)
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
            OnWorldChangeComplete(CurrentWorldState);

        isCurrentlyTransitioning = false;

        //  Update the layer
        Layers.ChangeLayers(playerController.gameObject, originalLayer);
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

    private void OnPlayerSpawned()
    {
        //  Set up world changer
        playerController = FindObjectOfType<PlayerController>();
    }
}
