using UnityEngine;
using System.Collections;

public class PlayerCollisions : MonoBehaviour
{
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

            //  Send message to the hit object through the parent OnPlayerCollision script
            OnPlayerCollision playerHitObject = hit.collider.GetComponent<OnPlayerCollision>();
            playerHitObject.OnPlayerHit(hit, pushDir);
        }

        //  Event
        if (OnCollisionHit != null)
            OnCollisionHit(hit);
    }
}
