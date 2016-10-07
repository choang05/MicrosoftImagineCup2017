using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour {

    //Private variables
    private AudioSource grassStepSource;                            // The audio source for footsteps
    private AudioSource playerGroundImpactSource;                   // audio source for impact with ground for player


    // Use this for initialization
    void Start () {
        grassStepSource = GetComponentInChildren<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //play audio source for footsteps when player is walking
    void grassFootstepAudio()
    {
        randomizePitch(grassStepSource);
        randomizeVolume(grassStepSource);
        grassStepSource.Play();

    }

    // Called to randomize the pitch of certain audio sources so they don't get dull to hear
    void randomizePitch(AudioSource audio)
    {
        audio.pitch = Random.Range(0.95f, 1.05f);
    }

    //randomize volume
    void randomizeVolume(AudioSource audio)
    {
        audio.volume = Random.Range(0.95f, 1.05f);
    }
}
