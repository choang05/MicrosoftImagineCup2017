using UnityEngine;
using System.Collections;

public class ImpactAudio : MonoBehaviour
{

    //variables
    private float velToVol = 0.2f;
    private AudioSource impact;

    // Use this for initialization
    void Awake()
    {
        impact = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        CharacterController2D.OnCollisionHit += playerHit;
    }

    void OnDisable()
    {
        CharacterController2D.OnCollisionHit -= playerHit;
    }

    //on collision play attached audiosource and calculate volume of impact
    void OnCollisionEnter(Collision hit)
    {
        playerAudio.randomizePitch(impact);
        float hitVol = hit.impulse.magnitude * velToVol;
        impact.volume = hitVol;
        impact.Play();
    }

    void playerHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Box"))
        {
            playerAudio.randomizePitch(impact);
            float hitVol = hit.moveLength;
            impact.volume = hitVol;
            impact.Play();
        }
    }
}