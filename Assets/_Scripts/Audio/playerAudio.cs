using UnityEngine;
using System.Collections;

public class playerAudio : MonoBehaviour {

    // variables
    public AudioClip footsteps;
    public AudioClip ladder;
    public AudioClip grassImpact;
    public AudioClip boxSlide;

    private AudioSource playerSound;
    private float velToVol = 0.2f;

    // Initialize audiosource component
	void Awake()
    {
        playerSound = GetComponent<AudioSource>();
    }

    // play footstep audio during animation events
    void grassFootstepAudio()
    {
        randomizePitch(playerSound);
        playerSound.PlayOneShot(footsteps, randomVolume());
    }

    // play ladder climbing audio during animation events
    void climbingLadderAudio()
    {
        randomizePitch(playerSound);
        playerSound.PlayOneShot(ladder, randomVolume());
    }

    // audiosource pitch randomizer
    public static void randomizePitch(AudioSource audio)
    {
        audio.pitch = Random.Range(0.95f, 1.05f);
    }

    // return random volume value in 5% range
    public static float randomVolume()
    {
        return Random.Range(0.95f, 1.05f);
    }

    void OnEnable()
    {
        CharacterController2D.OnPushing += sliding;
        CharacterController2D.OnPulling += sliding;
    }

    void OnDisable()
    {
        CharacterController2D.OnPushing -= sliding;
        CharacterController2D.OnPulling -= sliding;
    }

    public void sliding()
    {
        randomizePitch(playerSound);
        playerSound.PlayOneShot(boxSlide, randomVolume());
    }

    public void grassImpactsound()
    {
        randomizePitch(playerSound);
        playerSound.PlayOneShot(grassImpact, randomVolume());
    }
}
