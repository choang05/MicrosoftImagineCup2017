using UnityEngine;
using System.Collections;

public class EventPlantAcorn : MonoBehaviour
{
    Acorn acorn;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Tags.Player) && Input.GetButtonDown("Interact"))
        {
            PlayerItem playerItem = other.GetComponent<PlayerItem>();
            if (playerItem.hasItem)
            {
                acorn = playerItem.heldItem.GetComponent<Acorn>();
                if (acorn != null)
                {
                    StartCoroutine(CoPlantAcorn(playerItem));
                }
            }
        }
    }

    IEnumerator CoPlantAcorn(PlayerItem other)
    {
        other.DropItemAnimationStart();

        yield return new WaitForSeconds(2);

        GetComponent<EventWaterSoil>().isAcornPlanted = true;

        acorn.DestroyCollisions();

        Destroy(acorn.gameObject);
        Destroy(this);
    }
}
