using UnityEngine;
using System.Collections;

public class playerAudio : MonoBehaviour
{

    // variables
    public AudioClip grassfootsteps;
    public AudioClip woodfootsteps;
    public AudioClip stonefootsteps;
    public AudioClip waterfootsteps;
    public AudioClip ladder;
    public AudioClip ropeClimb;
    public AudioClip ropeSwing;
    public AudioClip[] timeWarps;
    new GameObject camera;
    private AudioSource ambiance;
    private AudioSource playerSound;

    // Initialize audiosource component
	void Awake()
    {
        playerSound = GetComponent<AudioSource>();
    }

    // play footstep audio during animation events
    void FootstepAudio(RaycastHit hit)
    {
        ObjectMaterial obMat = hit.collider.GetComponent<ObjectMaterial>();
        if (obMat != null)
        {
            if (obMat.Material == ObjectMaterial.MaterialType.grass)
            {
                randomizePitch(playerSound);
                playerSound.PlayOneShot(grassfootsteps);
            }
            else if(obMat.Material == ObjectMaterial.MaterialType.wood)
            {
                randomizePitch(playerSound);
                playerSound.PlayOneShot(woodfootsteps);
            }
            else if (obMat.Material == ObjectMaterial.MaterialType.stone)
            {
                randomizePitch(playerSound);
                playerSound.PlayOneShot(stonefootsteps);
            }
            else if (obMat.Material == ObjectMaterial.MaterialType.water)
            {
                randomizePitch(playerSound);
                playerSound.PlayOneShot(waterfootsteps);
            }
        }
    }

    // play ladder climbing audio during animation events
    void climbingLadderAudio()
    {
        randomizePitch(playerSound);
        playerSound.PlayOneShot(ladder);
    }

    // audiosource pitch randomizer
    public static void randomizePitch(AudioSource audio)
    {
        audio.pitch = Random.Range(0.95f, 1.05f);
    }

    // return random volume value in 5% range
    /*public static float randomVolume()
    {
        return Random.Range(0.95f, 1.05f);
    }*/

    // add audio methods to events
    void OnEnable()
    {
        WorldChanger.OnWorldChangeStart += timeWarpSound;
        WorldChanger.OnWorldChangeComplete += ambianceChange;
        PlayerCollisions.OnFootstep += FootstepAudio;
    }
    // remove audio methods from events when completed
    void OnDisable()
    {
        WorldChanger.OnWorldChangeStart -= timeWarpSound;
        WorldChanger.OnWorldChangeComplete -= ambianceChange;
        PlayerCollisions.OnFootstep -= FootstepAudio;
    }
    
    // audio for animation event of rope climbing
    void playerRopeClimb()
    {
        randomizePitch(playerSound);
        playerSound.PlayOneShot(ropeClimb);
    }

    // experimental animation event method for rope swinging audio
    void playerRopeSwing()
    {
        if (playerSound.isPlaying)
        {
            playerSound.Stop();
            randomizePitch(playerSound);
            playerSound.PlayOneShot(ropeSwing);
        }
        else
        {
            randomizePitch(playerSound);
            playerSound.PlayOneShot(ropeSwing);
        }
    }

    // method for time warp event audio
    void timeWarpSound(WorldChanger.WorldState ws)
    {
        randomizePitch(playerSound);
        int randomIndex = Random.Range(0, timeWarps.Length);
        playerSound.PlayOneShot(timeWarps[randomIndex]);
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
    public void cameraAudioStop(string cam)
    {
        camera = GameObject.Find(cam);
        ambiance = camera.GetComponent<AudioSource>();
        ambiance.Stop();
    }
    // find target camera and start audio
    public void cameraAudioStart(string cam)
    {
        camera = GameObject.Find(cam);
        ambiance = camera.GetComponent<AudioSource>();
        ambiance.Play();
    }
}
