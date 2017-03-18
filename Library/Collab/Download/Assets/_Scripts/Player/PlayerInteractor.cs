using UnityEngine;
using System.Collections;

public class PlayerInteractor : MonoBehaviour
{
    //  User Parameters variables
    //public bool CanUseLever = true;

    //  Private variables
    private bool flipBeforeLever;

    //  References variables
    private CharacterController charController;
    private PlayerController playerController;
    private Puppet2D_GlobalControl puppet2DGlobalControl;

    private Lever leverInUse;

    //  Animation variables
    [HideInInspector] public Animator animator;
    int turnLeverTriggerHash = Animator.StringToHash("turnLeverTrigger");

    //  Events
    public delegate void PlayerActionEvent();
    public static event PlayerActionEvent OnJumpLaunch;
    public static event PlayerActionEvent OnLanding;
    public delegate void PlayerActionEvent_PushPull(PushPullObject pushPullObject);
    public static event PlayerActionEvent_PushPull OnPushPullStart;
    public static event PlayerActionEvent_PushPull OnPushing;
    public static event PlayerActionEvent_PushPull OnPulling;
    public static event PlayerActionEvent_PushPull OnPushPullEnd;

    void Awake ()
    {
        //  Find and assign references
        charController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        puppet2DGlobalControl = GetComponentInChildren<Puppet2D_GlobalControl>();
	}

    #region Update(): check and evaluate input and states every frame
    void Update()
    {
        /*if (playerController.currentState != PlayerController.PlayerState.None)
            CanUseLever = false;
        else
            CanUseLever = true;*/
    }
    #endregion

    public void OnTurnLeverStart(Lever lever)
    {
        playerController.velocity = Vector3.zero;

        leverInUse = lever;

        leverInUse.IsUsable = false;

        //CanUseLever = false;

        //  Align position with lever
        transform.position = new Vector3(lever.transform.position.x, transform.position.y, transform.position.z);

        //  Disable player controller so player cannot move
        playerController.isControllable = false;

        flipBeforeLever = puppet2DGlobalControl.flip;

        //  Animation
        if (lever.currentLeverState == Lever.LeverState.Left)
        {
            puppet2DGlobalControl.flip = true;
            animator.SetTrigger(turnLeverTriggerHash);
        }
        else
        {
            puppet2DGlobalControl.flip = false;
            animator.SetTrigger(turnLeverTriggerHash);
        }
    }

    //  Animation
    public void StartLeverAnimation()
    {
        if (leverInUse)
            leverInUse.TurnLever();
    }

    public void OnTurnLeverEnd()
    {
        //  Align position with lever
        //transform.position = new Vector3(lever.transform.position.x, transform.position.y, transform.position.z);

        leverInUse.IsUsable = true;

        leverInUse = null;

        puppet2DGlobalControl.flip = flipBeforeLever;

        //  Renable player controller so player cannot move
        playerController.isControllable = true;
    }
}

