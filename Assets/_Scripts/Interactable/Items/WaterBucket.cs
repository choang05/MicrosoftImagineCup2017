using UnityEngine;
using System.Collections;

public class WaterBucket : MonoBehaviour
{
    public GameObject unfilledSprite;
    public GameObject filledSprite;
    public bool hasWater;

    public AudioClip fillwithwater;
    public AudioClip pourwater;
    private AudioSource bucketSound;

    void Awake()
    {
        bucketSound = GetComponent<AudioSource>();
    }

    public void SetBucketWater(bool isFilled)
    {
        if (isFilled)
        {
            playerAudio.randomizePitch(bucketSound);
            bucketSound.PlayOneShot(fillwithwater, playerAudio.randomVolume());

            hasWater = true;

            unfilledSprite.SetActive(false);
            filledSprite.SetActive(true);
        }
        else
        {
            hasWater = false;

            unfilledSprite.SetActive(true);
            filledSprite.SetActive(false);

            playerAudio.randomizePitch(bucketSound);
            bucketSound.PlayOneShot(pourwater, playerAudio.randomVolume());
        }
    }
}
