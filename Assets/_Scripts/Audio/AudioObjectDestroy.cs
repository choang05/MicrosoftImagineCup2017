using UnityEngine;
using System.Collections;

public class AudioObjectDestroy : MonoBehaviour {

    public GameObject ObjectToDestroy;
    public int Delay;

    public void destroyAudioObject()
    {
        Destroy(ObjectToDestroy, Delay);
    }

}
