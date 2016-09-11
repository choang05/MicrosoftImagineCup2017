using UnityEngine;

public class ObstacleImpact : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag(Tags.Player))
        {
            CharacterController2D charController = other.collider.GetComponent<CharacterController2D>();

            //  Get total force. (impulse / time)
            Vector3 collisionForce = other.impulse / Time.fixedDeltaTime;
            //Debug.Log(collisionForce.magnitude);

            //  Evaluate force if its enough to kill the player
            if (collisionForce.magnitude >= charController.impactForceThreshold)
            {
                charController.Die();
            }
        }
    }
}
