using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Com.LuisPedroFonseca.ProCamera2D;

public class ButtonNewGame : MonoBehaviour
{
    GameManager gameManager;

    private ProCamera2DTransitionsFX MainProCamera2DTransitionFX;

    void Awake()
    {
        MainProCamera2DTransitionFX = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ProCamera2DTransitionsFX>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void LoadSceneByIndex(int index)
    {
        GameManager.CurrentCheckpointID = 0;
        gameManager.hasPresentWisp = false;
        gameManager.hasPastWisp = false;
        gameManager.hasFutureWisp = false;
        //  Perform the transition coroutine to the master scene
        StartCoroutine(CoTransitionToMasterScene(index));
    }

    IEnumerator CoTransitionToMasterScene(int sceneIndex)
    {
        Time.timeScale = 1;

        //AsyncOperation async = SceneManager.LoadSceneAsync(1);

        //  Perform the exit transition
        MainProCamera2DTransitionFX.TransitionExit();

        //  Delay until exit transition is complete
        float delay = MainProCamera2DTransitionFX.DurationExit;
        yield return new WaitForSeconds(delay);

        //  Load the Master Scene
        SceneManager.LoadScene(sceneIndex);
    }
}