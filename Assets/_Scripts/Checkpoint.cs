using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    public GameObject LevelSegmentsGO;
    public int checkpointID;

    public delegate void CheckpointEvent(int checkpointID);
    public static event CheckpointEvent OnCheckpoint;

    //  References
    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        //gameManager.Checkpoints.Add(this);
    }

    //  Called when a collider enters another collider with isTrigger enabled
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            //  Set new checkpoint to gamemanager
            gameManager.CurrentCheckpointID = checkpointID;

            //  Disable the level before the previous level
            if (checkpointID - 2 >= 0)
                gameManager.UnloadLevelSegment(checkpointID - 2);

            //  Enable the next level if it exist
            if (checkpointID + 1 <= gameManager.Checkpoints.Length)
                gameManager.LoadLevelSegment(checkpointID + 1);

            if (OnCheckpoint != null)
                OnCheckpoint(checkpointID);

            //  Save data
            gameManager.SavePlayerData();

            if(Application.isEditor) Debug.Log("Game saved at checkpoint: " + gameManager.CurrentCheckpointID);

            //  Destroy the checkpoint, as we only need to save once.
            Destroy(gameObject);
        }
    }
}
