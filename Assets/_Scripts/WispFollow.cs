using UnityEngine;
using System.Collections;

public class WispFollow : MonoBehaviour
{
    //  User-Assigned
    public Transform wisp1;
    public float smoothTime = 0.3F;

    //  Private
    private SphereCollider sphereColl;
    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        sphereColl = GetComponent<SphereCollider>();
    }

	// Use this for initialization
	void Start ()
    {
        //StartCoroutine(CoFollow());    
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 targetPosition = (Random.insideUnitSphere + sphereColl.transform.position) * sphereColl.radius;
        while (Vector3.Distance(wisp1.transform.position, targetPosition) >= 0.5f)
        {
            //Debug.Log(targetPosition);
            //Vector3 targetPosition = target.TransformPoint(new Vector3(0, 5, -10));
            wisp1.transform.position = Vector3.SmoothDamp(wisp1.transform.position, targetPosition, ref velocity, smoothTime);
        }

    }

    /*IEnumerator CoWispFollow()
    {
    } */
}
