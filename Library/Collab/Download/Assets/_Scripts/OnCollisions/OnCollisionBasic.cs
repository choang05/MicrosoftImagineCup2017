using UnityEngine;
using System.Collections;

public class OnCollisionBasic : OnPlayerCollision
{
    public float ForceMultiplier;

    private void OnEnable()
    {
        PlayerCollisions.OnPlayerHit += OnPlayerHit;
    }

    private void OnDisable()
    {
        PlayerCollisions.OnPlayerHit -= OnPlayerHit;
    }

    void OnPlayerHit(ControllerColliderHit hit)
    {
        if (hit.gameObject != gameObject)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, hit.moveDirection.y, 0);
        hit.rigidbody.velocity = pushDir * ForceMultiplier;

        //Debug.Log("Player Hit: " + gameObject.name);
    }
}
