using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    public GameObject[] LevelSegmentsGO;
    public int checkpointID;
    public bool isPastAvaliable;
    public bool isFutureAvaliable;
    
    //  References
    private GameManager gameManager;
    private WorldChanger worldChanger;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        worldChanger = FindObjectOfType<WorldChanger>();
        //gameManager.Checkpoints.Add(this);
    }

    //  Called when a collider enters another collider with isTrigger enabled
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player) && gameManager.CurrentCheckpointID != checkpointID)
        {
            //  Set new checkpoint to gamemanager
            gameManager.CurrentCheckpointID = checkpointID;

            //  Disable the level before the previous level
            if (checkpointID - 2 >= 0)
                gameManager.UnloadLevelSegment(checkpointID - 2);

            //  Enable the next level if it exist
            if (checkpointID + 1 <= gameManager.Checkpoints.Length)
                gameManager.LoadLevelSegment(checkpointID + 1);

            //  Set world changer availibilities
            worldChanger.isPastAvaliable = isPastAvaliable;
            worldChanger.isFutureAvaliable = isFutureAvaliable;

            //  Save data
            gameManager.SaveData();

            if(Application.isEditor) Debug.Log("Game saved at checkpoint: " + gameManager.CurrentCheckpointID); 
        }
    }
}
