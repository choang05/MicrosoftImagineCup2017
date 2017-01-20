using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
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
        transform.position = currentWaypoint.position;

        StartCoroutine(PlatformTransition());
    }

    //  IEnumerator to translate between waypoints
    IEnumerator PlatformTransition()
    {
        while (true)
        {
            //  scaling function
            while (Vector2.Distance(transform.position, currentWaypoint.position) > 0.1f)
            {
                if (transitionType == TransitionTypes.None)
                    transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, Time.deltaTime * transitionSpeed);
                else if(transitionType == TransitionTypes.Smooth)
                    transform.position = Vector3.Lerp(transform.position, currentWaypoint.position, Time.deltaTime * transitionSpeed);

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
}
