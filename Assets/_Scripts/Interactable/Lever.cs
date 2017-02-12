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

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetButtonDown("Interact") && other.CompareTag(Tags.Player))
        {
            //  Send message to player
            PlayerInteractor playerInteractor = other.GetComponent<PlayerInteractor>();

            if (playerInteractor.CanUseLever)
            {
                playerInteractor.OnTurnLeverStart(this);
            }
        }
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
    }
}
