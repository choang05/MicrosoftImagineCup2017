using UnityEngine;
using System.Collections;

public class ImpactAudio : MonoBehaviour
{

    //variables
    public AudioClip woodHit;

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
        impact.PlayOneShot(woodHit, hitVol);
    }

    // On collision with player play sound
    void playerHit(ControllerColliderHit hit)
    {
        if ((hit.collider.GetComponent<ObjectType_material>().material == ObjectType_material.MaterialType.wood) && ((hit.controller.velocity.magnitude*velToVol) > 1f))
        {
            playerAudio.randomizePitch(impact);
            float hitVol = hit.controller.velocity.magnitude * velToVol;
            if (!impact.isPlaying)
                impact.PlayOneShot(woodHit, hitVol);
        }
    }
}