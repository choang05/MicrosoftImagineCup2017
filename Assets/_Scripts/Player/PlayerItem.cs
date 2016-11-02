using UnityEngine;
using System.Collections;

public class PlayerItem : MonoBehaviour
{
    public Transform R_HandNode;

    [HideInInspector]
    public GameObject heldItem;
    public bool hasItem;

    //  References
    CharacterController2D charController;

    //  Animation
    private Animator animator;
    int pickUpTriggerHash = Animator.StringToHash("pickUpTrigger");

    void Awake()
    {
        charController = GetComponent<CharacterController2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact") && hasItem)
        {
            HoldableItem holdableItem = heldItem.GetComponent<HoldableItem>();

            heldItem = null;
            hasItem = false;

            //  Remove parent hand
            holdableItem.transform.SetParent(null);
            holdableItem.transform.position = new Vector3(holdableItem.transform.position.x, holdableItem.transform.position.y, transform.position.z);

            holdableItem.boxColl[0].enabled = true;
            holdableItem.boxColl[1].enabled = true;
            holdableItem.rigidBody.isKinematic = false;

            //Debug.Log("Dropped item");
        }
    }

    public void PickUpAnimationStart()
    {
        charController.canMove = false;

        animator.SetTrigger(pickUpTriggerHash);    
    }

    public void PickUpAnimationComplete()
    {
        charController.canMove = true;

        //  Set item parent to player hand
        hasItem = true;
        heldItem.transform.SetParent(R_HandNode);
        heldItem.transform.localPosition = Vector3.zero;
    }
}
