using UnityEngine;
using System.Collections;

public class EventRopeTrapSpring : MonoBehaviour
{
    public HingeJoint mainRope;
    public Rope bottomRope;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            StartCoroutine(CoSpringTrap());
        }
    }

    IEnumerator CoSpringTrap()
    {

        mainRope.connectedAnchor = new Vector3(mainRope.connectedAnchor.x, 23, mainRope.connectedAnchor.z);

        yield return null;

        bottomRope.HangFirstSegment = false;
        bottomRope.HangLastSegment = false;

        //yield return new WaitForSeconds(2);

        //Destroy(this);
    }
}
