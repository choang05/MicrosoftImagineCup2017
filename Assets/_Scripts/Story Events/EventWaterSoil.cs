using UnityEngine;
using System.Collections;

public class EventWaterSoil : MonoBehaviour
{
    public GameObject PresentTree;
    public GameObject FutureTree;

    WaterBucket waterBucket;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Tags.Player) && Input.GetButtonDown("Interact"))
        {
            PlayerItem playerItem = other.GetComponent<PlayerItem>();
            if (playerItem.hasItem)
            {
                waterBucket = playerItem.heldItem.GetComponent<WaterBucket>();
                if (waterBucket != null && waterBucket.hasWater)
                {
                    StartCoroutine(CoWaterSoil(playerItem));
                }
            }
        }
    }

    IEnumerator CoWaterSoil(PlayerItem other)
    {
        other.DropItemAnimationStart();

        yield return new WaitForSeconds(1);

        PresentTree.SetActive(true);
        FutureTree.SetActive(true);

        Destroy(waterBucket.gameObject);

        Destroy(this);
    }
}
