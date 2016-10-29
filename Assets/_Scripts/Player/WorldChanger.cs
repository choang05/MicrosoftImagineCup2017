﻿using UnityEngine;
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
    public bool isPresentAvaliable;
    public bool isPastAvaliable;
    public bool isFutureAvaliable;
    [HideInInspector] public bool canSwitchPresent = true;
    [HideInInspector] public bool canSwitchPast = true;
    [HideInInspector] public bool canSwitchFuture = true;
    public float transitionDuration;
    [Range(0, 1)] public float transitionEdgeSmoothness;
    private LayerMask originalLayer;

    public CameraTransition cameraTransition;
    [HideInInspector] public CharacterController2D charController;
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
        if (charController == null)
            return;
        
        //  Determine which world player can be teleported too if there is open space
        CheckWorldCollisions();

        //  Ensure the player stays on the correct Z plane at all times
        switch (currentWorldState)
        {
            case WorldState.Present:
                charController.transform.position = new Vector3(charController.transform.position.x, charController.transform.position.y, 0);
                break;
            case WorldState.Past:
                charController.transform.position = new Vector3(charController.transform.position.x, charController.transform.position.y, 25);
                break;
            case WorldState.Future:
                charController.transform.position = new Vector3(charController.transform.position.x, charController.transform.position.y, 50);
                break;
        }

        //  If player is allowed to switch & a transition is currently not running...
        if (!isCurrentlyTransitioning)
        {
            //  Evaluate input from player. 1-3 selects which world to transition to
            if (Input.GetKeyUp(KeyCode.Alpha1) && currentWorldState != WorldState.Present && isPresentAvaliable && canSwitchPresent)
            {
                //  Broadcast event delegate
                if (OnWorldChangeStart != null)
                    OnWorldChangeStart(WorldState.Present);

                SwitchWorld(1); //  Present
            }
            else if (Input.GetKeyUp(KeyCode.Alpha2) && currentWorldState != WorldState.Past && isPastAvaliable && canSwitchPast)
            {
                //  Broadcast event delegate
                if (OnWorldChangeStart != null)
                    OnWorldChangeStart(WorldState.Past);

                SwitchWorld(2); //  Past
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3) && currentWorldState != WorldState.Future && isFutureAvaliable && canSwitchFuture) 
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

        //  Disable audio listener
        currentCamera.GetComponent<AudioListener>().enabled = false;

        isCurrentlyTransitioning = true;

        //  Update the layer
        Layers.ChangeLayers(gameObject, Layers.ViewAlways);

        //  Cache the player's position in normalized screen space coordinates.
        Vector2 transitionCenter = currentCamera.WorldToViewportPoint(charController.transform.position);

        //  Determine which world ID to switch to and check if world is already active.
        if (worldID == 1)
        {
            //  Update world state
            currentWorldState = WorldState.Present;

            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, PresentCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness, transitionCenter });

            //  Set new Z position for player
            //charController.transform.position = new Vector3(charController.transform.position.x, transform.position.y, 0);
        }
        else if (worldID == 2)
        {
            //  Update world state
            currentWorldState = WorldState.Past;
            
            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, PastCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness, transitionCenter });

            //  Set new Z position for player
            //transform.position = new Vector3(transform.position.x, transform.position.y, 25);
        }
        else if (worldID == 3)
        {
            //  Update world state
            currentWorldState = WorldState.Future;
            
            //  Perform transition
            cameraTransition.DoTransition(CameraTransitionEffects.SmoothCircle, currentCamera, FutureCamera, transitionDuration, new object[] { false, transitionEdgeSmoothness, transitionCenter });

            //  Set new Z position for player
            //transform.position = new Vector3(transform.position.x, transform.position.y, 50);
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
        Vector2 playerPos = new Vector2(charController.transform.position.x, charController.transform.position.y);
        RaycastHit hit;
        Vector3 rayDir;

        //  If player is not in the Present, check collisions for the Present
        if (currentWorldState != WorldState.Present)
        {
            //  cast present ray & evaluate
            rayDir = new Vector3(playerPos.x, playerPos.y, -5);
            if (Physics.Raycast(rayDir, Vector3.forward * 10, out hit, 15))
            {
                canSwitchPresent = false;
                if (Application.isEditor) Debug.DrawRay(rayDir, Vector3.forward * 10, Color.red, 0.01f);
            }
            else
            {
                canSwitchPresent = true;
                if (Application.isEditor) Debug.DrawRay(rayDir, Vector3.forward * 10, Color.green, 0.01f);
            }
        }
        //  If player is not in the past, check collisions for the past
        if (currentWorldState != WorldState.Past)
        {
            //  cast past ray & evaluate
            rayDir = new Vector3(playerPos.x, playerPos.y, 20);
            if (Physics.Raycast(rayDir, Vector3.forward * 10, out hit, 15))
            {
                canSwitchPast = false;
                if (Application.isEditor) Debug.DrawRay(rayDir, Vector3.forward * 10, Color.red, 0.01f);
            }
            else
            {
                canSwitchPast = true;
                if (Application.isEditor) Debug.DrawRay(rayDir, Vector3.forward * 10, Color.green, 0.01f);
            }
        }
        //  If player is not in the Future, check collisions for the Future
        if (currentWorldState != WorldState.Future)
        {
            //  cast future ray & evaluate
            rayDir = new Vector3(playerPos.x, playerPos.y, 45);
            if (Physics.Raycast(rayDir, Vector3.forward * 10, out hit, 15))
            {
                canSwitchFuture = false;
                if (Application.isEditor) Debug.DrawRay(rayDir, Vector3.forward * 10, Color.red, 0.01f);
            }
            else
            {
                canSwitchFuture = true;
                if (Application.isEditor) Debug.DrawRay(rayDir, Vector3.forward * 10, Color.green, 0.01f);
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
        Layers.ChangeLayers(gameObject, originalLayer);
    }
}
