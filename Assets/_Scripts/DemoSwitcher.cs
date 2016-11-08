using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Com.LuisPedroFonseca.ProCamera2D;

public class DemoSwitcher : MonoBehaviour
{
    //  Static instance of object to dnot destroy on load
    private static DemoSwitcher control;

    GameManager gameManager;

    void Awake()
    {
        #region Dont Destroy On Load
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
            Destroy(gameObject);
        #endregion

        gameManager = FindObjectOfType<GameManager>();
    }

    public void ChangeScene(int checkpointID)
    {
        gameManager.CurrentCheckpointID = checkpointID;

        StartCoroutine(CoReloadScene());

        //SceneManager.LoadScene(sceneName);
    }

    IEnumerator CoReloadScene()
    {
        //  Perform the exit transition
        ProCamera2D.Instance.GetComponent<ProCamera2DTransitionsFX>().TransitionExit();

        //  Delay until exit transition is complete
        float delay = ProCamera2D.Instance.GetComponent<ProCamera2DTransitionsFX>().DurationExit;
        yield return new WaitForSeconds(delay);

        //  Load the Master Scene
        SceneManager.LoadScene(1);
    }
}
