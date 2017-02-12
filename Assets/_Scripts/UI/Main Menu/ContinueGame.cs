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

    void Start()
    {
        //  Determine at start if a save file exist, if so, the continue button should be enabled
        if (!File.Exists(Application.persistentDataPath + "/playerSave1.dat"))
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
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
}
