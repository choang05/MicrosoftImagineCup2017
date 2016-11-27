using UnityEngine;
using System.Collections;

public class BucketSound : MonoBehaviour {

    public AudioClip scoop;
    public AudioClip clothingMovement;
    public AudioClip pouring;
    private AudioSource bucketSound;

	// Use this for initialization
	void Awake () {
        bucketSound = GetComponent<AudioSource>();
	}
	
    public void playWaterScoop()
    {
        bucketSound.PlayOneShot(scoop);
    }

    public void playClothMovement()
    {
        bucketSound.PlayOneShot(clothingMovement);
    }

    public void playPouringSound()
    {
        bucketSound.PlayOneShot(pouring);
    }
	
}
