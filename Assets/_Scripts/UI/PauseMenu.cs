using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMainMenu;

    private bool isPaused;
    private int currentScene;

    //  References
    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Use this for initialization
    void Start ()
    {
	
	}

    void Update()
    {
        // if Esc is pressed and scene is master scene... 
        if (Input.GetKeyUp(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex == 1)
        {
            //  if the game is not paused... then pause it
            if (!isPaused)
            {
                PauseMainMenu.SetActive(true);
                IsPaused = true;

                //  Freeze the time so nothing moves
                Time.timeScale = 0;
            }
            //  Else unpause the game
            else
            {
                PauseMainMenu.SetActive(false);
                IsPaused = false;

                //  Revert game timescale back to normal
                Time.timeScale = 1;
            }
        }
    }

    public bool IsPaused
    {
        get
        {
            return isPaused;
        }

        set
        {
            isPaused = value;
        }
    }

    public void ResumeGame()
    {
        IsPaused = false;
    }
}
