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

    // Audio
    public GameObject clothe;
    public GameObject scoop;
    public GameObject pour;

    void Awake()
    {
        charController = GetComponent<CharacterController2D>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        //bSound = bucket.GetComponent<BucketSound>();
        //dstroy = bucket.GetComponent<AudioObjectDestroy>();
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
        charController.isControllable = false;
       
        //play audio for bucket pickup
        if (heldItem.CompareTag("Bucket") || heldItem.CompareTag("Basket"))
        {
            GameObject bucketSound = Instantiate(clothe) as GameObject;
        }
        animator.SetTrigger(pickUpTriggerHash);    
    }

    public void PickUpAnimationComplete()
    {
        charController.isControllable = true;

        //  Set item parent to player hand
        hasItem = true;
        heldItem.transform.SetParent(R_HandNode);
        heldItem.transform.localPosition = Vector3.zero;
    }

    public void DropItemAnimationStart()
    {
        charController.isControllable = false;

        Ray ray = new Ray(Vector3.zero, Vector3.down);
        RaycastHit hit;

        ray.origin = transform.position;

        Physics.Raycast(ray, out hit);
        ObjectMaterial obMat = hit.collider.GetComponent<ObjectMaterial>();

        if (obMat != null && obMat.Material == ObjectMaterial.MaterialType.water)
        {
            GameObject waterscoop = Instantiate(scoop) as GameObject;
        }   
        else if (obMat != null && obMat.Material == ObjectMaterial.MaterialType.grass)
        {
            GameObject waterpour = Instantiate(pour) as GameObject;
        }

        animator.SetTrigger(dropItemTriggerHash);
    }

    public void DropItemAnimationComplete()
    {
        charController.isControllable = true;

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
