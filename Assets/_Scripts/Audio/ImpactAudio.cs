using UnityEngine;
using System.Collections;

public class impactAudio : MonoBehaviour {

    //variables
    private float velToVol = 0.2f;
    private AudioSource impact;
    public PlayerAudio pa;

    // Use this for initialization
    void Start () {
        impact = GetComponent<AudioSource>();
	}

    void OnCollisionEnter(Collision hit)
    {
        pa.randomizePitch(impact);
        float hitVol = hit.impulse.magnitude * velToVol;
        impact.volume = hitVol;
        impact.Play();
    }
}
