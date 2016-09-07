using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour
{
    public GameObject ObjectToDestroy;
    public int Delay;

    void Start()
    {
        Destroy(ObjectToDestroy, Delay);
    }
}
