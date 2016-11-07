using UnityEngine;
using System.Collections;

public class PlayerItem : MonoBehaviour
{
    public Transform R_HandNode;

    [HideInInspector]
    public GameObject heldItem;
    public bool canDropItem = true;
    public bool hasItem;

    //  References
    CharacterController2D charController;
    CharacterController controller;

    //  Animation
    private Animator animator;
    int pickUpTriggerHash = Animator.StringToHash("pickUpTrigger");
    int dropItemTriggerHash = Animator.StringToHash("dropItemTrigger");

    void Awake()
    {
        charController = GetComponent<CharacterController2D>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (canDropItem && Input.GetButtonDown("Interact") && hasItem && controller.isGrounded)
        {
            DropItemAnimationStart();    
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

    public void DropItemAnimationStart()
    {
        charController.canMove = false;

        animator.SetTrigger(dropItemTriggerHash);
    }

    public void DropItemAnimationComplete()
    {
        charController.canMove = true;

        DropItem();
    }

    private void DropItem()
    {
        HoldableItem holdableItem = heldItem.GetComponent<HoldableItem>();

        heldItem = null;
        hasItem = false;

        //  Remove parent hand
        holdableItem.transform.SetParent(null);
        
        if (charController.facingDirection == CharacterController2D.FacingDirection.Left)
            holdableItem.transform.position = new Vector3(holdableItem.transform.position.x - 1, holdableItem.transform.position.y, transform.position.z);
        else
            holdableItem.transform.position = new Vector3(holdableItem.transform.position.x + 1, holdableItem.transform.position.y, transform.position.z);

        holdableItem.boxColl[0].enabled = true;
        holdableItem.boxColl[1].enabled = true;
        holdableItem.rigidBody.isKinematic = false;

        //Debug.Log("Dropped item");
    }
}
