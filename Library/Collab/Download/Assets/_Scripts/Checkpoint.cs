using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    public GameObject SegmentGO;
    public int checkpointID;
    public bool willSave = true;

    public delegate void CheckpointEvent(int checkpointID);
    public static event CheckpointEvent OnCheckpoint;

    //  References
    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        //gameManager.Checkpoints.Add(this);

        //  Error if SegmentGO does not exist
        if (SegmentGO == null)  Debug.LogError("SegmentGO is null!");
    }

    //  Called when a collider enters another collider with isTrigger enabled
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
  //          if (checkpointID != GameManager.CurrentCheckpointID)
  //          {
                //  Set new checkpoint to gamemanager
                GameManager.CurrentCheckpointID = checkpointID;

                int currentCheckpointIndex = gameManager.GetCurrentCheckpointIndex(checkpointID);

                //  Disable the level before the previous level
                if (currentCheckpointIndex - 2 >= 0)
                    gameManager.UnloadLevelSegment(currentCheckpointIndex - 2);

                //  Enable the next level if its not the last one
                if (currentCheckpointIndex < gameManager.Checkpoints.Length)
                {
                    //Debug.Log(currentCheckpointIndex + "/" + gameManager.Checkpoints.Length);
                    gameManager.LoadLevelSegment(currentCheckpointIndex + 1);
                }

                if (OnCheckpoint != null)
                    OnCheckpoint(checkpointID);

                //  Save data
                if (willSave)
                {
                    gameManager.SavePlayerData();
                    if(Application.isEditor) Debug.Log("Game saved at checkpoint: " + GameManager.CurrentCheckpointID);
                }

//            }

            //  Destroy the checkpoint.
            //Destroy(gameObject);
            GetComponent<BoxCollider>().enabled = false;
            Destroy(this);
        }
    }
}
