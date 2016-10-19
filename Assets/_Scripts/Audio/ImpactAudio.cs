using UnityEngine;
using System.Collections;

public class impactAudio : MonoBehaviour {

    //variables
    private float velToVol = 0.2f;
    private AudioSource impact;

    public PlayerAudio pa;

    // Use this for initialization
    void Awake () {
        impact = GetComponent<AudioSource>();
	}
    
    //on collision play attached audiosource and calculate volume of impact
    void OnCollisionEnter(Collision hit)
    {
        pa.randomizePitch(impact);
        float hitVol = hit.impulse.magnitude * velToVol;
        impact.volume = hitVol;
        impact.Play();
    }
}
