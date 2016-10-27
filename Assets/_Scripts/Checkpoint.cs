using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    public int AreaID;
    
    //  References
    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.Checkpoints.Add(this);
    }

    //  Called when a collider enters another collider with isTrigger enabled
    void OnTriggerEnter(Collider other)
    {
        gameManager.CurrentAreaID = AreaID;     
    }
}
