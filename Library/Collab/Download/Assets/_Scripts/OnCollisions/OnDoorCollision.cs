using UnityEngine;
using System.Collections;

public class OnDoorCollision : OnPlayerCollision
{
    public float ForceMultiplier;

    private AudioSource audioSource;

    private void OnEnable()
    {
        PlayerCollisions.OnPlayerHit += OnPlayerHit;
    }

    private void OnDisable()
    {
        PlayerCollisions.OnPlayerHit -= OnPlayerHit;
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnPlayerHit(ControllerColliderHit hit)
    {
        if (hit.gameObject != gameObject)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, hit.moveDirection.y, 0);
        hit.rigidbody.velocity = pushDir * ForceMultiplier;

        // door creak audio
        if (!audioSource.isPlaying)
        {
            playerAudio.randomizePitch(audioSource);
            //audioSource.volume *= playerAudio.randomVolume();
            audioSource.Play();
        }
    }
}
