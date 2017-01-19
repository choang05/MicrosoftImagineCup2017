using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public int LeverID;
    public bool isOneShot;

    public LeverState currentLeverState;
    public enum LeverState
    {
        Left,
        Right
    }

    public delegate void LeverTurnHandler(int leverID);
    public static event LeverTurnHandler OnLeverTurnRight;
    public static event LeverTurnHandler OnLeverTurnLeft;

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetButtonDown("Interact") && other.CompareTag(Tags.Player))
        {
            // Lever animation
            if (currentLeverState == LeverState.Left && isOneShot)
            {
                if (OnLeverTurnRight != null)
                    OnLeverTurnRight(LeverID);

                Debug.Log("Lever " + LeverID + " turned Right and broke!");

                Destroy(this);
            }
            else if (currentLeverState == LeverState.Left)
            {
                if (OnLeverTurnRight != null)
                    OnLeverTurnRight(LeverID);

                currentLeverState = LeverState.Right;

                Debug.Log("Lever " + LeverID + " turned Right!");
            }
            else
            {
                if (OnLeverTurnLeft != null)
                    OnLeverTurnLeft(LeverID);

                currentLeverState = LeverState.Left;

                Debug.Log("Lever " + LeverID + " turned Left!");
            }
        }
    }



}
