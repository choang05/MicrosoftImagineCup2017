using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void LoadSceneByIndex(int index)
    {
        //  Chad - no reference error. Wrote alternative below
        //GameManager.manager.CurrentScene++;

        gameManager.CurrentCheckpointID = 0;

        SceneManager.LoadScene(index);
    }   
}