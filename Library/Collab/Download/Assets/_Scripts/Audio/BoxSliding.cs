using UnityEngine;
using System.Collections;

public class BoxSliding : MonoBehaviour
{

    private AudioSource audioSource;
    public AudioClip boxSlide;

	// Use this for initialization
	void Awake ()
    {
        audioSource = GetComponent<AudioSource>();
	}

    void OnEnable()
    {
        PlayerController.OnPushing += OnSliding;
        PlayerController.OnPulling += OnSliding;
    }

    void OnDisable()
    {
        PlayerController.OnPushing -= OnSliding;
        PlayerController.OnPulling -= OnSliding;
    }

    // audio for push/pull box
    void OnSliding(PushPullObject pushPullObject)
    {
        //playerAudio.randomizePitch(boxAudioSrc);
        audioSource.pitch = Random.Range(.8f, 1);
        audioSource.PlayOneShot(boxSlide);
    }
}
