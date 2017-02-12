using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour
{
    public enum TransitionModes { Automatic, Manual };
    public TransitionModes transitionMode;
    public enum TransitionTypes { Normal, Accelerate};
    public TransitionTypes transitionType;

    public Transform[] wayPoints;
    public float transitionSpeed;
    public float idleDuration;

    private int nextWayPoint;
    private Transform currentWaypoint;
    public int[] AcceptedLeverIDs;

    private List<int> remainingLeverIDs = new List<int>();

    private bool isSatisfied;

    //   Event Listener
    void OnEnable()
    {
        Lever.OnLeverTurnLeft += MovePlatform;
        Lever.OnLeverTurnRight += MovePlatform;
    }

    void OnDisable()
    {
        Lever.OnLeverTurnLeft -= MovePlatform;
        Lever.OnLeverTurnRight -= MovePlatform;
    }

    
    // Use this for initialization
    void Start()
    {
        currentWaypoint = wayPoints[0];
        transform.position = currentWaypoint.position;
        if (transitionMode == TransitionModes.Automatic)
            StartCoroutine(PlatformTransition());
        remainingLeverIDs = AcceptedLeverIDs.ToList();
    }

    //  IEnumerator to translate between waypoints
    IEnumerator PlatformTransition()
    {
        while (true)
        {
            currentWaypoint = GetNextWaypoint();
            //  scaling function
            while (Vector2.Distance(transform.position, currentWaypoint.position) > 0.1f)
            {
                if (transitionType == TransitionTypes.Normal)
                    transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, Time.deltaTime * transitionSpeed);
                else if (transitionType == TransitionTypes.Accelerate)
                    transform.position = Vector3.Lerp(transform.position, currentWaypoint.position, Time.deltaTime * transitionSpeed);

                //Debug.Log(Mathf.Abs(platform.position.magnitude - currentWaypoint.position.magnitude));
                yield return null;
            }

            if (idleDuration > 0)
            {
                yield return new WaitForSeconds(idleDuration);
            }

            if (transitionMode == TransitionModes.Manual)
                yield break;
        }

    }
    public void MovePlatform(int leverID)
    {
        if (DoesAcceptLeverID(leverID))
        {
            transform.position = currentWaypoint.position;
            StartCoroutine("PlatformTransition");
            if (remainingLeverIDs.Contains(leverID))
                remainingLeverIDs.Remove(leverID);
            else remainingLeverIDs.Add(leverID);
        }

    }

    //  Returns the transform of the nextwaypoint
    private Transform GetNextWaypoint()
    {
        nextWayPoint = (nextWayPoint + 1) % wayPoints.Length;
        return wayPoints[nextWayPoint];
    }

    //  Detect when player is on the platform
    void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
        //Debug.Log("Player is on moving platform!");
    }
    void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
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
