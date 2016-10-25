using UnityEngine;

public class ObstacleImpact : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag(Tags.Player))
        {
            //  Cache the player's character controller
            PlayerDeath playerDeath = other.collider.GetComponent<PlayerDeath>();

            //  Get total force. (impulse / time)
            Vector3 collisionForce = other.impulse / Time.fixedDeltaTime;

            playerDeath.ProcessImpact(collisionForce);
        }
    }
}
