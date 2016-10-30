using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID;
    
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
        if (other.CompareTag(Tags.Player) && gameManager.CurrentCheckpointID != checkpointID)
        {
            gameManager.CurrentCheckpointID = checkpointID;

            //  Save data
            gameManager.SaveData();

            if(Application.isEditor) Debug.Log("Game saved at checkpoint: " + gameManager.CurrentCheckpointID); 
        }
    }
}
