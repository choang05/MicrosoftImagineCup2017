using UnityEngine;
using System.Collections;

public class BoxSliding : MonoBehaviour {

    private AudioSource boxAudioSrc;
    public AudioClip boxSlide;

	// Use this for initialization
	void Awake () {
        boxAudioSrc = GetComponent<AudioSource>();
	}

    void OnEnable()
    {
        PlayerController.OnPushing += sliding;
        PlayerController.OnPulling += sliding;
    }

    void OnDisable()
    {
        PlayerController.OnPushing -= sliding;
        PlayerController.OnPulling -= sliding;
    }

    // audio for push/pull box
    public void sliding(PushPullObject pushPullObject)
    {
        playerAudio.randomizePitch(boxAudioSrc);
        boxAudioSrc.PlayOneShot(boxSlide, boxAudioSrc.volume * playerAudio.randomVolume());
    }
}
