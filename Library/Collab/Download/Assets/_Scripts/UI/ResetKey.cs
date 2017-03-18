using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetKey : MonoBehaviour
{
    public Image ProgressBar;
    public float fillRate;
    public float defillRate;

    private bool isUsable = true;
    private bool isActive = false;

    private GameManager gameManager;


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        if (GameManager.CurrentCheckpointID < 2)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            isActive = true;
        }
    }

    private void OnEnable()
    {
        Checkpoint.OnCheckpoint += onCheckpoint;
    }

    private void OnDisable()
    {
        Checkpoint.OnCheckpoint -= onCheckpoint;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetButton("Reset") && isUsable && isActive)
        {
            ProgressBar.fillAmount += fillRate;

            if (ProgressBar.fillAmount >= 1)
            {
                isUsable = false;

                gameManager.RespawnPlayer();
            }
        }
        else if (isUsable && isActive)
        {
            ProgressBar.fillAmount -= defillRate;
        }
	}

    private void onCheckpoint(int checkpointID)
    {
        if (checkpointID >= 2)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            isActive = true;
            Checkpoint.OnCheckpoint -= onCheckpoint;
        }
    }
}
