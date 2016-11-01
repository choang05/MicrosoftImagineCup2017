using UnityEngine;
using System.Collections;

public class PlayerCollisions : MonoBehaviour
{
    //  Events
    public delegate void PlayerCollisionEvent(ControllerColliderHit hit);
    public static event PlayerCollisionEvent OnCollisionHit;
    public delegate void PlayerFootCollisionEvent(RaycastHit hit);
    public static event PlayerFootCollisionEvent OnFootstep;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // ground Audio
        GroundImpactAudio gimpactaudio = hit.collider.GetComponent<GroundImpactAudio>();
        if (gimpactaudio != null)
            if ((hit.controller.velocity.magnitude * 0.2f) > 1f)
                gimpactaudio.playerHit(hit);

        // other audio
        ImpactAudio impactaudio = hit.collider.GetComponent<ImpactAudio>();
        if (impactaudio != null)
            if ((hit.controller.velocity.magnitude * 0.2f) > 1f)
                impactaudio.playerHit(hit);

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
            if (playerHitObject != null)
                playerHitObject.OnPlayerHit(hit, pushDir);
        }

        //  Event
        if (OnCollisionHit != null)
            OnCollisionHit(hit);
    }

    public void ProcessFootstep()
    {
        Ray ray = new Ray(Vector3.zero, Vector3.down);
        RaycastHit hit;

        ray.origin = transform.position;

        //  Debug ray                                                                                                        
        //if (Application.isEditor) Debug.DrawRay(ray.origin, ray.direction, Color.blue, 0.01f);

        if (Physics.Raycast(ray, out hit))
        {
            //  Broadcast event
            if (OnFootstep != null)
                OnFootstep(hit);

            //Debug.Log("Stepped on " + hit.collider.name);
        }

    }
}
