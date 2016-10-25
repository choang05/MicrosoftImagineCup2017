using UnityEngine;
using System.Collections;

public class ImpactAudio : MonoBehaviour
{

    //variables
    public AudioClip hitSound;

    public float velToVol = 0.2f;
    private AudioSource impact;

    // Use this for initialization
    void Awake()
    {
        impact = GetComponent<AudioSource>();
    }

    //on collision play attached audiosource and calculate volume of impact
    void OnCollisionEnter(Collision hit)
    {
        playerAudio.randomizePitch(impact);
        float hitVol = hit.impulse.magnitude * velToVol;
        impact.PlayOneShot(hitSound, hitVol);
    }

    // On collision with player play sound
    public void playerHit(ControllerColliderHit hit)
    {
        playerAudio.randomizePitch(impact);
        float hitVol = hit.controller.velocity.magnitude * velToVol;
        if (!impact.isPlaying)
            impact.PlayOneShot(hitSound, hitVol);
    }
}