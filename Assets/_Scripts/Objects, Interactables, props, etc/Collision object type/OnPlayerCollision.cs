using UnityEngine;
using System.Collections;

public class OnPlayerCollision : MonoBehaviour
{
    //  This is the parent method, use this to pass functions to the corresponding child
    public virtual void OnPlayerHit(ControllerColliderHit hit, Vector3 pushDirection)
    { }
}
