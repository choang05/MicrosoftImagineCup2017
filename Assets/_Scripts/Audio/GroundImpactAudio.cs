using UnityEngine;
using System.Collections;

public class GroundImpactAudio : MonoBehaviour
{

    //variables
    private AudioSource gimpact;

    // Use this for initialization
    void Awake()
    {
        gimpact = GetComponent<AudioSource>();
    }

    // On collision with player play sound
    public void playerHit(ControllerColliderHit hit)
    {
        playerAudio.randomizePitch(gimpact);
        gimpact.volume *= playerAudio.randomVolume();
        if (!gimpact.isPlaying)
            gimpact.Play();
    }
}