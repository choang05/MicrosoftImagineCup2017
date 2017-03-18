using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMainMenu;
    public AudioMixerSnapshot paused;
    public AudioMixerSnapshot unpaused;
    public delegate void PauseMenuHandler();
    public static event PauseMenuHandler OnPauseMenuActivated;
    public static event PauseMenuHandler OnPauseMenuDeactivated;

    private bool isPaused;

    private SettingsManager settingsManger;
    
    private LinkedList<GameObject> navigation;

    private UnityStandardAssets.ImageEffects.BlurOptimized blurComponent;

    void Awake()
    {
        settingsManger = FindObjectOfType<SettingsManager>();
        navigation = new LinkedList<GameObject>();
        blurComponent = Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized>();
    }

    void Start()
    {
        PauseMainMenu.SetActive(false);
        blurComponent.enabled = false;
    }

    void Update()
    {
        // if Esc is pressed and scene is master scene... 
        if (Input.GetKeyUp(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex == 1)
        {

            //  if the game is not paused... then pause it
            if (!isPaused)
            {
                navigation.AddLast(PauseMainMenu);
                PauseGame();
            }
            //  Else unpause the game
            else
            {
                if(navigation.Count != 1)
                {
                    navigation.Last.Value.SetActive(false);
                    navigation.Last.Previous.Value.SetActive(true);
                    navigation.RemoveLast();
                }
                else
                {
                    navigation.RemoveLast();
                    ResumeGame();
                }
            }
        }
    }

    /*void Lowpass()
    {
        if (Time.timeScale == 0)
            paused.TransitionTo(.01f);
        else
            unpaused.TransitionTo(.01f);
    }*/

    public void LoadMainMenuScene()
    {
        Time.timeScale = 1;

        //  Perform the transition coroutine to the master scene
        StartCoroutine(CoTransitionToMasterScene(0));
    }

    public void AddNavigation(GameObject panel)
    {
        navigation.AddLast(panel);
    }
    public void RemoveNavigation()
    {
        navigation.RemoveLast();
    }

    public void ReloadToLastCheckpoint()
    {
        Time.timeScale = 1;

        DisableAllButtons();

        //  Perform the transition coroutine to the master scene
        StartCoroutine(CoTransitionToMasterScene(1));
    }

    IEnumerator CoTransitionToMasterScene(int sceneIndex)
    {
        //  Perform the exit transition
        ProCamera2D.Instance.GetComponent<ProCamera2DTransitionsFX>().TransitionExit();

        //  Delay until exit transition is complete
        float delay = ProCamera2D.Instance.GetComponent<ProCamera2DTransitionsFX>().DurationExit;
        yield return new WaitForSeconds(delay);

        //  Load the Master Scene
        SceneManager.LoadScene(sceneIndex);
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
    #endregion

    public void SaveAudio()
    {
        settingsManger.SaveSettings();
    }

    public void PauseGame()
    {
        PauseMainMenu.SetActive(true);
        IsPaused = true;

        //  Blur the camera
        blurComponent.enabled = true;

        if (OnPauseMenuActivated != null)
            OnPauseMenuActivated();
        
        //  Freeze the time so nothing moves
        Time.timeScale = 0;
        //Lowpass();
    }

    public void ResumeGame()
    {
        PauseMainMenu.SetActive(false);
        IsPaused = false;

        //  UnBlur the camera
        blurComponent.enabled = false;

        if (OnPauseMenuDeactivated != null)
            OnPauseMenuDeactivated();

        //  Revert game timescale back to normal
        Time.timeScale = 1;
        //Lowpass();
    }

    void DisableAllButtons()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
    }

}
