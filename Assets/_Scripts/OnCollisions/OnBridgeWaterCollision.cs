using UnityEngine;
using System.Collections;

public class OnBridgeWaterCollision : MonoBehaviour
{
    public GameObject SplashFX;

    //  Called when a collider enters another collider with isTrigger enabled
    void OnTriggerEnter(Collider other)
    {
        //  Spawn splash FX
        Instantiate(SplashFX, other.transform.position, Quaternion.identity);

        //  If triggered by a player...
        if (other.CompareTag(Tags.Player))
        {
            other.GetComponent<PlayerDeath>().DieByWater();
        }

    }
}
