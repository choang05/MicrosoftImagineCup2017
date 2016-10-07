using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour {

    //Public variables
    public AudioSource[] audioSrcs;                  //Audio components stored into array from the child objects of player
   

    // Use this for initialization
    void Awake () {
        audioSrcs = GetComponentsInChildren<AudioSource>();
    }

    //play audio source for footsteps when player is walking
    void grassFootstepAudio()
    {
        randomizePitch(audioSrcs[1]);
        randomizeVolume(audioSrcs[1], 0.95f, 1.05f);
        audioSrcs[1].Play();

    }

    //Play audio source for climbing ladder
    void climbingLadderAudio()
    {
        randomizePitch(audioSrcs[2]);
        randomizeVolume(audioSrcs[2], 0.15f, 0.25f);
        audioSrcs[2].Play();
    }

    // Called to randomize the pitch of certain audio sources so they don't get dull to hear
    void randomizePitch(AudioSource audio)
    {
        audio.pitch = Random.Range(0.95f, 1.05f);
    }

    //randomize volume
    void randomizeVolume(AudioSource audio, float a, float b)
    {
        audio.volume = Random.Range(a, b);
    }
}
