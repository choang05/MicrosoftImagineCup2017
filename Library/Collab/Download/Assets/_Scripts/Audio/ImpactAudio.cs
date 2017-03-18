using UnityEngine;
using System.Collections;

public class ImpactAudio : MonoBehaviour
{
    //variables
    private AudioSource audioSource;

    private void OnEnable()
    {
        PlayerCollisions.OnPlayerHit += OnPlayerHit;
    }

    private void OnDisable()
    {
        PlayerCollisions.OnPlayerHit -= OnPlayerHit;
    }

    // Use this for initialization
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    //on collision play attached audiosource and calculate volume of impact
    void OnCollisionEnter(Collision hit)
    {
        //Debug.Log(hit.impulse.sqrMagnitude);
        if (hit.impulse.sqrMagnitude > 25000)
        {
            //playerAudio.randomizePitch(impact);
            audioSource.pitch = Random.Range(.8f, 1.2f);
            //audioSource.volume *= playerAudio.randomVolume();
            audioSource.Play();
        }
    }

    // On collision with player play sound
    void OnPlayerHit(ControllerColliderHit hit)
    {
        if (hit.gameObject != gameObject)
            return;

        playerAudio.randomizePitch(audioSource);
        //audioSource.volume *= playerAudio.randomVolume();
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}