using UnityEngine;
using System.Collections;

public class EventFallingLeaves : MonoBehaviour
{
    public GameObject[] fallingLeaves;

    public bool isEnabled;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            for (int i = 0; i < fallingLeaves.Length; i++)
            {
                fallingLeaves[i].SetActive(isEnabled);
            }

            Destroy(gameObject);
        }
    }
}
