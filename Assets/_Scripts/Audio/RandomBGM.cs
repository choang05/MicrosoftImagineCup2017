using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBGM : MonoBehaviour
{
    public bool DestroyOnComplete = true;

    private AudioSource audioSource;
    public AudioClip[] BGMs;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start ()
    {
        audioSource.clip = BGMs[Random.Range(0, BGMs.Length)];
        audioSource.Play();

        if (DestroyOnComplete)
        {
            Destroy(this);
        }
	}
}
