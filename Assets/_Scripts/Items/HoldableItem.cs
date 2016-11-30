using UnityEngine;
using System.Collections;

public class HoldableItem : MonoBehaviour
{
    [HideInInspector] public BoxCollider[] boxColl;
    [HideInInspector] public Rigidbody rigidBody;

    void Awake()
    {
        boxColl = GetComponents<BoxCollider>();
        rigidBody = GetComponent<Rigidbody>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Tags.Player) && Input.GetButtonDown("Interact"))
        {
            PlayerItem playerItem = other.GetComponent<PlayerItem>();

            if (!playerItem.hasItem)
            {
                playerItem.heldItem = gameObject;

                //  Animation
                playerItem.PickUpAnimationStart();

                boxColl[0].enabled = false;
                boxColl[1].enabled = false;
                rigidBody.isKinematic = true;
            }
        }
    }
}
