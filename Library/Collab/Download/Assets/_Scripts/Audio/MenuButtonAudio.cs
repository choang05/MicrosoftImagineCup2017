using UnityEngine;
using System.Collections;

public class MenuButtonAudio : MonoBehaviour {

    public AudioClip hover;
    private AudioSource buttonSound;

	// Use this for initialization
	void Awake () {
        buttonSound = GetComponent<AudioSource>();
	}

    public void hoverSound()
    {
        playerAudio.randomizePitch(buttonSound);
        buttonSound.PlayOneShot(hover);
    }
	
}
