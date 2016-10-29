using UnityEngine;
using System.Collections;

public class ImpactAudio : MonoBehaviour
{

    //variables
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
        impact.volume *= playerAudio.randomVolume();
        impact.Play();
    }

    // On collision with player play sound
    public void playerHit(ControllerColliderHit hit)
    {
        playerAudio.randomizePitch(impact);
        impact.volume *= playerAudio.randomVolume();
        if (!impact.isPlaying)
            impact.Play();
    }
}