using UnityEngine;
using System.Collections;

public class KeyTest : MonoBehaviour
{
    public GameObject Door;
    public GameObject DoorUI;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Item))
        {
            Door.SetActive(false);
        }
        else
        {
            Door.SetActive(true);
        }
    }
}
