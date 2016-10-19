using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour {

    //variables
    private float velToVol = 0.2f;
    public AudioClip[] playerClips;                  //Audio components stored into array from the child objects of player
    private AudioSource playerSound;

    // Use this for initialization
    void Awake () {
        playerSound = GetComponent<AudioSource>();
    }

    //play audio source for footsteps when player is walking
    void grassFootstepAudio()
    {
        randomizePitch(playerSound);
        playerSound.PlayOneShot(playerClips[0], randomVolume());
    }

    //Play audio source for climbing ladder
    void climbingLadderAudio()
    {
        randomizePitch(playerSound);
        playerSound.PlayOneShot(playerClips[1], randomVolume());
    }

    // Called to randomize the pitch of certain audio sources so they don't get dull to hear
    public void randomizePitch(AudioSource audio)
    {
        audio.pitch = Random.Range(0.95f, 1.05f);
    }

    //OneShot random Volume value
    public float randomVolume()
    {
        return Random.Range(0.95f, 1.05f);
    }

    //randomize which audio clip plays during time warps
    public AudioClip randomTimeWarp(AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        return clips[randomIndex];
    }
}
