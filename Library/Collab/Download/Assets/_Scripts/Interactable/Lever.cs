using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public int LeverID;
    public bool isSingleUse;

    public LeverState currentLeverState;
    public enum LeverState
    {
        Left,
        Right
    }

    public delegate void LeverTurnHandler(int leverID);
    public static event LeverTurnHandler OnLeverTurnRight;
    public static event LeverTurnHandler OnLeverTurnLeft;

    //  Animation
    private Animator animator;
    int turnLeverLeftTriggerHash = Animator.StringToHash("turnLeverLeftTrigger");
    int turnLeverRightTriggerHash = Animator.StringToHash("turnLeverRightTrigger");

    //  Audio
    public AudioClip leverTurnAudio;
    private AudioSource audioSrc;

    private bool isUsable = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PushPullable))
            isUsable = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetButtonUp("Interact") && other.CompareTag(Tags.Player) && isUsable)
        {
            //  Ensure player is in a none state before allowing to use lever
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController.currentState != PlayerController.PlayerState.None)
                return;

            //  Send message to player
            PlayerInteractor playerInteractor = other.GetComponent<PlayerInteractor>();

            playerInteractor.OnTurnLeverStart(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.PushPullable))
            isUsable = true;
    }

    public void TurnLever()
    {
        if (currentLeverState == LeverState.Left && isSingleUse)
        {
            if (OnLeverTurnRight != null)
                OnLeverTurnRight(LeverID);

            //Debug.Log("Lever " + LeverID + " turned Right and broke!");

            //  Animation
            animator.SetTrigger(turnLeverRightTriggerHash);

            Destroy(this);
        }
        else if (currentLeverState == LeverState.Left)
        {
            if (OnLeverTurnRight != null)
                OnLeverTurnRight(LeverID);

            currentLeverState = LeverState.Right;

            //  Animation
            animator.SetTrigger(turnLeverRightTriggerHash);

            //Debug.Log("Lever " + LeverID + " turned Right!");
        }
        else
        {
            if (OnLeverTurnLeft != null)
                OnLeverTurnLeft(LeverID);

            currentLeverState = LeverState.Left;

            //  Animation
            animator.SetTrigger(turnLeverLeftTriggerHash);
            //Debug.Log("Lever " + LeverID + " turned Left!");
        }

        //  Audio
        audioSrc.pitch = Random.Range(0.8f, 1.2f);
        audioSrc.PlayOneShot(leverTurnAudio);
    }

    public bool IsUsable
    {
        get
        {
            return isUsable;
        }
        set
        {
            isUsable = value;
        }
    }
}
