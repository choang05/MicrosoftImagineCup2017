using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangesColorFromLever : MonoBehaviour
{
    //  User Defined Variables
    public int[] AcceptedLeverIDs;

    private List<int> remainingLeverIDs = new List<int>();
    private ColorAssigner colorAssigner;

    private bool isSatisfied;

    //   Event Listener
    void OnEnable()
    {
        Lever.OnLeverTurnLeft += OnLeverTurnLeft;
        Lever.OnLeverTurnRight += OnLeverTurnRight;
    }

    void OnDisable()
    {
        Lever.OnLeverTurnLeft -= OnLeverTurnLeft;
        Lever.OnLeverTurnRight -= OnLeverTurnRight;
    }

    // Use this for initialization
    void Start ()
    {
        colorAssigner = GetComponent<ColorAssigner>();

        remainingLeverIDs = AcceptedLeverIDs.ToList();
    }

    void OnLeverTurnLeft(int leverID)
    {
        if (!DoesAcceptLeverID(leverID))
            return;

        if (!remainingLeverIDs.Contains(leverID))
        {
            remainingLeverIDs.Add(leverID);

            if (isSatisfied == true)
            {
                Debug.Log("Bridge is lowered, but raised again");
            }

            isSatisfied = false;

            DisplayRemainingIDs();

        }

    }

    void OnLeverTurnRight(int leverID)
    {
        if (!DoesAcceptLeverID(leverID))
            return;

        if (remainingLeverIDs.Contains(leverID))
        {
            remainingLeverIDs.Remove(leverID);

            DisplayRemainingIDs();
        }

        if (remainingLeverIDs.Count <= 0)
        {
            isSatisfied = true;
            Debug.Log("Bridge dropped");
        }

    }

    bool DoesAcceptLeverID(int leverID)
    {
        if (AcceptedLeverIDs.Contains(leverID))
        {
            return true;
        }
        return false;
    }

    private void DisplayRemainingIDs()
    {
        for (int i = 0; i < remainingLeverIDs.Count; i++)
            Debug.Log(remainingLeverIDs[i]);
    }
}
