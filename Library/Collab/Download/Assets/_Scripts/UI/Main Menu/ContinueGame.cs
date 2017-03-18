using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Com.LuisPedroFonseca.ProCamera2D;

public class ContinueGame : MonoBehaviour
{
    GameManager gameManager;

    private ProCamera2DTransitionsFX MainProCamera2DTransitionFX;

    void Awake()
    {
        MainProCamera2DTransitionFX = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ProCamera2DTransitionsFX>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        if (AccountManager.IsLoggedIn)
        {
            if (AccountManager.CurrentUser.checkpointID <= 0)
                gameObject.SetActive(false);
            else
                gameObject.SetActive(true);
        }
        else
        {
            if (GameManager.DoesLocalSaveExist())
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);
        }
    }

    public void LoadLastScene()
    {
        //  Load in the save file
        gameManager.LoadPlayerData();

        //  Perform the transition coroutine to the master scene
        StartCoroutine(CoTransitionToMasterScene(1));
    }

    IEnumerator CoTransitionToMasterScene(int sceneIndex)
    {
        //  Perform the exit transition
        MainProCamera2DTransitionFX.TransitionExit();

        //  Delay until exit transition is complete
        float delay = MainProCamera2DTransitionFX.DurationExit;
        yield return new WaitForSeconds(delay);

        //  Load the Master Scene
        SceneManager.LoadScene(sceneIndex);
    }

    /*private void OnLoggedIn()
    {
        Debug.Log("On Logged In");

        if (AccountManager.CurrentUser.checkpointID <= 0)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }*/

    /*private void OnPlayOffline()
    {
        Debug.Log("On Play Offline");
        //  Determine at start if a save file exist, if so, the continue button should be enabled
        if (GameManager.DoesLocalSaveExist())
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }*/
}
