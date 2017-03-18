using UnityEngine;
using System.Collections;

public class OnBridgeWaterCollision : MonoBehaviour
{
    public GameObject SplashFX;
    private AudioSource waterSound;
    public AudioClip splashSound;

    void Awake()
    {
        waterSound = GetComponent<AudioSource>();
    }

    //  Called when a collider enters another collider with isTrigger enabled
    void OnTriggerEnter(Collider other)
    {
        //  Spawn splash FX
        Instantiate(SplashFX, other.transform.position, Quaternion.identity);

        // Play splash sound
        playerAudio.randomizePitch(waterSound);
        waterSound.PlayOneShot(splashSound);

        //  If triggered by a player...
        if (other.CompareTag(Tags.Player))
        {
            other.GetComponent<PlayerDeath>().DieByWater();
        }

    }
}
