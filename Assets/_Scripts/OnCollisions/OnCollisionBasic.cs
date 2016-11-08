using UnityEngine;
using System.Collections;

public class OnCollisionBasic : OnPlayerCollision
{
    public float ForceMultiplier;

    public override void OnPlayerHit(ControllerColliderHit hit, Vector3 pushDir)
    {
        hit.rigidbody.AddForceAtPosition(pushDir * ForceMultiplier, hit.point);
    }
}
