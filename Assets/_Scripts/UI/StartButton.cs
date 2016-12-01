using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Com.LuisPedroFonseca.ProCamera2D;

public class StartButton : MonoBehaviour
{
    GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void LoadSceneByIndex(int index)
    {
        gameManager.CurrentCheckpointID = 0;

        //  Perform the transition coroutine to the master scene
        StartCoroutine(CoTransitionToMasterScene(index));
    }

    IEnumerator CoTransitionToMasterScene(int sceneIndex)
    {
        Time.timeScale = 1;
        
        //  Perform the exit transition
        ProCamera2D.Instance.GetComponent<ProCamera2DTransitionsFX>().TransitionExit();

        //  Delay until exit transition is complete
        float delay = ProCamera2D.Instance.GetComponent<ProCamera2DTransitionsFX>().DurationExit;
        yield return new WaitForSeconds(delay);

        //  Load the Master Scene
        SceneManager.LoadScene(sceneIndex);
    }
}