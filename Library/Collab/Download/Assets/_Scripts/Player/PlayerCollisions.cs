using UnityEngine;
using System.Collections;

public class PlayerCollisions : MonoBehaviour
{
    public GameObject FootstepFX;
    public GameObject LandedFX;

    //  True if the player is on the ground(not platform)
    [HideInInspector] public bool isTouchingGround;

    //  References
    private CharacterController charController;

    //  Events
    public delegate void PlayerCollisionEvent(ControllerColliderHit hit);
    public static event PlayerCollisionEvent OnPlayerHit;
    public delegate void PlayerFootCollisionEvent(RaycastHit hit);
    public static event PlayerFootCollisionEvent OnFootstep;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //  If landed
        if ((charController.collisionFlags & CollisionFlags.Below) != 0)
        {
            if (hit.controller.velocity.magnitude > 6f)
            {
                //Debug.Log(hit.controller.velocity.magnitude + " Landed!");
                //  FX
                Instantiate(LandedFX, hit.point, Quaternion.identity);
            }
        }
        
        //  Evaluate what if the object hit is the ground (lowest platform/terrain)
        if (hit.collider.CompareTag(Tags.Ground))
            isTouchingGround = true;
        else
            isTouchingGround = false;

        // ground Audio
        GroundImpactAudio groundImpactAudio = hit.collider.GetComponent<GroundImpactAudio>();
        if (groundImpactAudio != null)
        {
            if ((hit.controller.velocity.magnitude * 0.2f) > 1f)
            {
                //Debug.Log(hit.controller.velocity.magnitude);
                groundImpactAudio.playerHit(hit);
            }
        }

        // other audio
        /*ImpactAudio impactAudio = hit.collider.GetComponent<ImpactAudio>();
        if (impactAudio != null)
            if ((hit.controller.velocity.magnitude * 0.2f) > 1f)
                impactAudio.playerHit(hit);*/

        //  Evaluate interaction things because CharacterController is in special status state that does not allow itself to collide against awake physics
        /*Rigidbody hitRigidbody = hit.collider.GetComponent<Rigidbody>();
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
        }*/

        //  Event
        if (OnPlayerHit != null)
            OnPlayerHit(hit);
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

            //  FX
            Instantiate(FootstepFX, hit.point, Quaternion.identity);

            //Debug.Log("Stepped on " + hit.collider.name);
        }

    }
}
