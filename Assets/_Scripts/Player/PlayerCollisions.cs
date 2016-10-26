using UnityEngine;
using System.Collections;

public class PlayerCollisions : MonoBehaviour
{
    public float doorForceMultiplier;
    public float bridgeForceMultiplier;

    //  Events
    public delegate void PlayerCollisionEvent(ControllerColliderHit hit);
    public static event PlayerCollisionEvent OnCollisionHit;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //  Evaluate interaction things because CharacterController is in special status state that does not allow itself to collide agaisnt awake physics
        Rigidbody hitRigidbody = hit.collider.GetComponent<Rigidbody>();
        if (hitRigidbody != null)
        {
            //  if the colliding rigidbody is a kinematic, do nothing.
            if (hitRigidbody.isKinematic)
                return;

            // Calculate push direction from move direction,
            Vector3 pushDir = new Vector3(hit.moveDirection.x, hit.moveDirection.y, 0);

            //  Evaluate what was hit and apply corresponding push force to it
            if(hit.collider.CompareTag(Tags.Door))
            {
                hitRigidbody.velocity = pushDir * doorForceMultiplier;
                //Debug.Log("Hit door!");
            }
            else if (hit.collider.CompareTag(Tags.Bridge))
            {
                hitRigidbody.velocity = pushDir * bridgeForceMultiplier;
                //Debug.Log("Hit bridge!");
            }
        }

        //  Event
        if (OnCollisionHit != null)
            OnCollisionHit(hit);
    }
}
