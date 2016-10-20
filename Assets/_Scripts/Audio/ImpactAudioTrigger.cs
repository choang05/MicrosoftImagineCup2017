using UnityEngine;
using System.Collections;

public class ImpactAudioTrigger : MonoBehaviour {

    //variables
    private float velToVol = 0.2f;
    private AudioSource impact;
    public PlayerAudio pa;

    // Use this for initialization
    void Start()
    {
        impact = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collision hit)
    {
        pa.randomizePitch(impact);
        float hitVol = hit.relativeVelocity.magnitude * velToVol;
        impact.volume = hitVol;
        impact.Play();
    }
}
