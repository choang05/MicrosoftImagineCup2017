using UnityEngine;
using System.Collections;

public class playerAudio : MonoBehaviour {

    // variables
    public AudioClip footsteps;
    public AudioClip ladder;
    public AudioClip grassImpact;
    public AudioClip boxSlide;
    public AudioClip ropeClimb;
    public AudioClip ropeSwing;

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
        CharacterController2D.OnCollisionHit += playerHitGround;
    }

    void OnDisable()
    {
        CharacterController2D.OnPushing -= sliding;
        CharacterController2D.OnPulling -= sliding;
        CharacterController2D.OnCollisionHit -= playerHitGround;
    }

    public void sliding()
    {
        randomizePitch(playerSound);
        playerSound.PlayOneShot(boxSlide, randomVolume());
    }

    void playerHitGround(ControllerColliderHit hit)
    {
        if ((hit.collider.GetComponent<ObjectType_material>().material == ObjectType_material.MaterialType.grass) && ((hit.controller.velocity.magnitude * velToVol) > 1f))
        {
            randomizePitch(playerSound);
            float hitVol = hit.controller.velocity.magnitude * velToVol;
            if (!playerSound.isPlaying)
                playerSound.PlayOneShot(grassImpact, hitVol);
        }
    }

    void playerRopeClimb()
    {
        randomizePitch(playerSound);
        playerSound.PlayOneShot(ropeClimb, randomVolume());
    }

    void playerRopeSwing()
    {
        if (playerSound.isPlaying)
        {
            playerSound.Stop();
            randomizePitch(playerSound);
            playerSound.PlayOneShot(ropeSwing, randomVolume());
        }
        else
        {
            randomizePitch(playerSound);
            playerSound.PlayOneShot(ropeSwing, randomVolume());
        }
    }
}
