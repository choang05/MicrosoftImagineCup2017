using UnityEngine;
using System.Collections;

public class impactAudio : MonoBehaviour {

    //variables
    private float velToVol = 0.2f;
    public AudioSource impact;

    // Use this for initialization
    void Start () {
        impact = GetComponent<AudioSource>();
	}

    // Called to randomize the pitch of certain audio sources so they don't get dull to hear
    void randomizePitch(AudioSource audio)
    {
        audio.pitch = Random.Range(0.95f, 1.05f);
    }

    //play audio when player impacts the grass, i.e. jumping, falling etc
    void OnCollisionEnter(Collision hit)
    {
        randomizePitch(impact);
        float hitVol = hit.relativeVelocity.magnitude * velToVol;
        impact.volume = hitVol;
        impact.Play();
    }
}
