using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour {

    //variables
    private float velToVol = 0.2f;
    private AudioSource[] audioSrcs;                  //Audio components stored into array from the child objects of player

    // Use this for initialization
    void Awake () {
        audioSrcs = GetComponentsInChildren<AudioSource>();
    }

    //play audio source for footsteps when player is walking
    void grassFootstepAudio()
    {
        randomizePitch(audioSrcs[0]);
        randomizeVolume(audioSrcs[0], 0.95f, 1.05f);
        audioSrcs[0].Play();

    }

    //Play audio source for climbing ladder
    void climbingLadderAudio()
    {
        randomizePitch(audioSrcs[3]);
        randomizeVolume(audioSrcs[3], 0.15f, 0.25f);
        audioSrcs[3].Play();
    }

    // Called to randomize the pitch of certain audio sources so they don't get dull to hear
    public void randomizePitch(AudioSource audio)
    {
        audio.pitch = Random.Range(0.95f, 1.05f);
    }

    //randomize volume
    void randomizeVolume(AudioSource audio, float a, float b)
    {
        audio.volume = Random.Range(a, b);
    }
}
