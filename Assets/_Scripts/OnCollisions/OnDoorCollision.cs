using UnityEngine;
using System.Collections;

public class OnDoorCollision : OnPlayerCollision
{
    public float ForceMultiplier;
    private AudioSource doorSound;

    void Awake()
    {
        doorSound = GetComponent<AudioSource>();
    }

    public override void OnPlayerHit(ControllerColliderHit hit, Vector3 pushDir)
    {
        hit.rigidbody.velocity = pushDir * ForceMultiplier;

        // door creak audio
        if (!doorSound.isPlaying)
        {
            playerAudio.randomizePitch(doorSound);
            doorSound.volume *= playerAudio.randomVolume();
            doorSound.Play();
        }
    }
}
