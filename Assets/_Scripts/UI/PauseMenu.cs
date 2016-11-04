using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMainMenu;
    public AudioMixerSnapshot paused;
    public AudioMixerSnapshot unpaused;

    private bool isPaused;

    private SettingsManager settingsManger;

    void AWake()
    {
        settingsManger = FindObjectOfType<SettingsManager>();
    }

    void Start()
    {
        PauseMainMenu.SetActive(false);
    }

    void Update()
    {
        // if Esc is pressed and scene is master scene... 
        if (Input.GetKeyUp(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex == 1)
        {

            //  if the game is not paused... then pause it
            if (!isPaused)
            {
                PauseGame();
            }
            //  Else unpause the game
            else
            {
                ResumeGame();
            }
        }
    }

    void Lowpass()
    {
        if (Time.timeScale == 0)
            paused.TransitionTo(.01f);
        else
            unpaused.TransitionTo(.01f);
    }

    #region Properties for fields
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

    public void PauseGame()
    {
        PauseMainMenu.SetActive(true);
        IsPaused = true;

        //  Freeze the time so nothing moves
        Time.timeScale = 0;
        Lowpass();
    }

    public void ResumeGame()
    {
        PauseMainMenu.SetActive(false);
        IsPaused = false;

        //  Revert game timescale back to normal
        Time.timeScale = 1;
        Lowpass();
    }
    #endregion
}
