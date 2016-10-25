using UnityEngine;
using System.Collections;

public class playerAudio : MonoBehaviour {

    // variables
    public AudioClip footsteps;
    public AudioClip ladder;
    public AudioClip boxSlide;
    public AudioClip ropeClimb;
    public AudioClip ropeSwing;
    public AudioClip[] timeWarps;
    new GameObject camera;
    private AudioSource ambiance;
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
    // add audio methods to events
    void OnEnable()
    {
        CharacterController2D.OnPushing += sliding;
        CharacterController2D.OnPulling += sliding;
        WorldChanger.OnWorldChangeStart += timeWarpSound;
        WorldChanger.OnWorldChangeComplete += ambianceChange;
    }
    // remove audio methods from events when completed
    void OnDisable()
    {
        CharacterController2D.OnPushing -= sliding;
        CharacterController2D.OnPulling -= sliding;
        WorldChanger.OnWorldChangeStart -= timeWarpSound;
        WorldChanger.OnWorldChangeComplete -= ambianceChange;
    }
    // audio for push/pull box
    public void sliding()
    {
        randomizePitch(playerSound);
        playerSound.PlayOneShot(boxSlide, randomVolume());
    }
    // audio for animation event of rope climbing
    void playerRopeClimb()
    {
        randomizePitch(playerSound);
        playerSound.PlayOneShot(ropeClimb, randomVolume());
    }
    // experimental animation event method for rope swinging audio
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

    // method for time warp event audio
    void timeWarpSound(WorldChanger.WorldState ws)
    {
        randomizePitch(playerSound);
        int randomIndex = Random.Range(0, timeWarps.Length);
        playerSound.PlayOneShot(timeWarps[randomIndex], randomVolume());
    }

    // start new ambiance in current time
    void ambianceChange(WorldChanger.WorldState ws)
    {
        if (ws == WorldChanger.WorldState.Present)
        {
            cameraAudioStop("Past Camera");
            cameraAudioStop("Future Camera");
            cameraAudioStart("Present Camera");
        }
        else if (ws == WorldChanger.WorldState.Past)
        {
            cameraAudioStop("Present Camera");
            cameraAudioStop("Future Camera");
            cameraAudioStart("Past Camera");
        }
        else if (ws == WorldChanger.WorldState.Future)
        {
            cameraAudioStop("Past Camera");
            cameraAudioStop("Present Camera");
            cameraAudioStart("Future Camera");
        }
    }

    // find target camera and disable audio
    void cameraAudioStop(string cam)
    {
        camera = GameObject.Find(cam);
        ambiance = camera.GetComponent<AudioSource>();
        ambiance.Stop();
    }
    // find target camera and start audio
    void cameraAudioStart(string cam)
    {
        camera = GameObject.Find(cam);
        ambiance = camera.GetComponent<AudioSource>();
        ambiance.Play();
    }
}
