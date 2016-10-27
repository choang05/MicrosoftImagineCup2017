using UnityEngine;
using System.Collections;

public class EventBreakBridge : MonoBehaviour
{
    public HingeJoint firstHinge;
    public HingeJoint secondHinge;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            StartCoroutine(CoBreakBridge());
        }
    }

    private IEnumerator CoBreakBridge()
    {
        firstHinge.breakForce = 0;

        yield return new WaitForSeconds(.25f);

        secondHinge.breakForce = 0;

        Destroy(gameObject);
    }
}
