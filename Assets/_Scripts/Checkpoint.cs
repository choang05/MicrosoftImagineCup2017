using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    //  References
    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    //  Called when a collider enters another collider with isTrigger enabled
    void OnTriggerEnter(Collider other)
    {
        gameManager.LatestCheckpoint = transform;     
    }
}
