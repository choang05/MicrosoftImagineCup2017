using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour {

    //variables
    private float velToVol = 0.2f;
    private AudioSource[] audioSrcs;                  //Audio components stored into array from the child objects of player
    private AudioSource grassStep;
    private AudioSource ladderSound;

    // Use this for initialization
    void Awake () {
        audioSrcs = GetComponentsInChildren<AudioSource>();
        grassStep = audioSrcs[0];
        ladderSound = audioSrcs[4];
    }

    //play audio source for footsteps when player is walking
    void grassFootstepAudio()
    {
        randomizePitch(grassStep);
        randomizeVolume(grassStep, 0.95f, 1.05f);
        grassStep.Play();

    }

    //Play audio source for climbing ladder
    void climbingLadderAudio()
    {
        randomizePitch(ladderSound);
        randomizeVolume(ladderSound, 0.15f, 0.25f);
        ladderSound.Play();
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

    //randomize which audio clip plays during time warps
    public AudioSource randomTimeWarp(AudioSource[] clips)
    {
        int randomIndex = Random.Range(6, clips.Length);
        randomizePitch(clips[randomIndex]);
        return clips[randomIndex];
    }
}
