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
    CharacterController charController;
    PlayerController playerController;

    //  Animation
    private Animator animator;
    int pickUpTriggerHash = Animator.StringToHash("pickUpTrigger");
    int dropItemTriggerHash = Animator.StringToHash("dropItemTrigger");

    // Audio
    //GameObject bucket;
    //BucketSound bSound;

    void Awake()
    {
        charController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        //bucket = GameObject.Find("WaterBucket");
        //bSound = bucket.GetComponent<BucketSound>();
    }

    void Update()
    {
        if (canDropItem && Input.GetButtonDown("Interact") && hasItem && charController.isGrounded)
        {
            DropItemAnimationStart();    
        }
    }

    public void PickUpAnimationStart()
    {
        playerController.isControllable = false;
        playerController.velocity = Vector3.zero;
       
        //play audio for bucket pickup
        /*if (heldItem.CompareTag("Bucket") || heldItem.CompareTag("Basket"))
        {
            //bSound.playClothMovement();
        }*/
        animator.SetTrigger(pickUpTriggerHash);    
    }

    public void PickUpAnimationComplete()
    {
        playerController.isControllable = true;

        //  Set item parent to player hand
        hasItem = true;
        heldItem.transform.SetParent(R_HandNode);
        heldItem.transform.localPosition = Vector3.zero;
    }

    public void DropItemAnimationStart()
    {
        playerController.isControllable = false;
        playerController.velocity = Vector3.zero;

        //Ray ray = new Ray(Vector3.zero, Vector3.down);
        //RaycastHit hit;

        //ray.origin = transform.position;

        //Physics.Raycast(ray, out hit);
        //ObjectMaterial obMat = hit.collider.GetComponent<ObjectMaterial>();

       // if (obMat != null && obMat.Material == ObjectMaterial.MaterialType.water)
       //     bSound.playWaterScoop();
       // else if (obMat != null && obMat.Material == ObjectMaterial.MaterialType.soil)
       //    bSound.playPouringSound();

        animator.SetTrigger(dropItemTriggerHash);
    }

    public void DropItemAnimationComplete()
    {
        playerController.isControllable = true;

        DropItem();
    }

    private void DropItem()
    {
        if (!heldItem)
            return;

        HoldableItem holdableItem = heldItem.GetComponent<HoldableItem>();
        if (!holdableItem)
            return;

        heldItem = null;
        hasItem = false;

        //  Remove parent hand
        holdableItem.transform.SetParent(null);
        
        if (playerController.facingDirection == PlayerController.FacingDirection.Left)
            holdableItem.transform.position = new Vector3(holdableItem.transform.position.x - 1, holdableItem.transform.position.y, transform.position.z);
        else
            holdableItem.transform.position = new Vector3(holdableItem.transform.position.x + 1, holdableItem.transform.position.y, transform.position.z);

        holdableItem.boxColl[0].enabled = true;
        holdableItem.boxColl[1].enabled = true;
        holdableItem.rigidBody.isKinematic = false;

        //Debug.Log("Dropped item");
    }
}
