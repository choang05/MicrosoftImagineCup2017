using UnityEngine;
using System.Collections;

public class AudioObjectDestroy : MonoBehaviour
{

    public GameObject ObjectToDestroy;
    new AudioSource audio;

    void Awake()
    {
        audio = ObjectToDestroy.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!audio.isPlaying)
        {
            Destroy(ObjectToDestroy);
        }
    }

}
