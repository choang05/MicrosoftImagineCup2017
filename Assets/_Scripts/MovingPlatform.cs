using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{

    public Transform platform;
    public Transform[] wayPoints;
    public float transitionSpeed;
    public float idleDuration;
    public TransitionTypes transitionType;
    public enum TransitionTypes { None, Smooth};

    private int nextWayPoint;
    private Transform currentWaypoint;

    void Start()
    {
        currentWaypoint = wayPoints[0];
        platform.position = currentWaypoint.position;

        StartCoroutine(PlatformTransition());
    }

    //  IEnumerator to translate between waypoints
    IEnumerator PlatformTransition()
    {
        while (true)
        {
            //  scaling function
            while (Mathf.Abs(platform.position.magnitude - currentWaypoint.position.magnitude) > 0.1f)
            {
                if (transitionType == TransitionTypes.None)
                    platform.position = Vector3.MoveTowards(platform.position, currentWaypoint.position, Time.deltaTime * transitionSpeed);
                else if(transitionType == TransitionTypes.Smooth)
                    platform.position = Vector3.Lerp(platform.position, currentWaypoint.position, Time.deltaTime * transitionSpeed);

                //Debug.Log(Mathf.Abs(platform.position.magnitude - currentWaypoint.position.magnitude));
                yield return null;
            }

            if (idleDuration > 0)
            {
                yield return new WaitForSeconds(idleDuration);
            }

            currentWaypoint = GetNextWaypoint();
        }
    }

    //  Returns the transform of the nextwaypoint
    private Transform GetNextWaypoint()
    {
        nextWayPoint = (nextWayPoint + 1) % wayPoints.Length;
        return wayPoints[nextWayPoint];
    }
}
