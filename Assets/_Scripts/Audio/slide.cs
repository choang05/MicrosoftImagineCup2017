using UnityEngine;
using System.Collections;

public class slide : MonoBehaviour {

    public AudioClip boxSlide;

    private AudioSource playerSound;

    void Awake()
    {
        playerSound = GetComponent<AudioSource>();
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
        playerAudio.randomizePitch(playerSound);
        playerSound.PlayOneShot(boxSlide, playerAudio.randomVolume());
    }
}
