using UnityEngine;
using System.Collections;

public class EventFillBucketWithWater : MonoBehaviour
{
    //  references
    public WaterBucket waterBucket;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Tags.Player) && Input.GetButtonDown("Interact"))
        {
            PlayerItem playerItem = other.GetComponent<PlayerItem>();
            if (playerItem.hasItem)
            {
                waterBucket = playerItem.heldItem.GetComponent<WaterBucket>();
                if(waterBucket != null)
                {
                    StartCoroutine(CoFillBucket(playerItem));
                }
            }
        }
    }

    IEnumerator CoFillBucket(PlayerItem other)
    {
        Animator animator = other.GetComponent<Animator>();
        animator.SetTrigger(Animator.StringToHash("dropItemTrigger"));
        
        yield return new WaitForSeconds(1);

        waterBucket.SetBucketWater(true);

        Destroy(this);
    }
}
