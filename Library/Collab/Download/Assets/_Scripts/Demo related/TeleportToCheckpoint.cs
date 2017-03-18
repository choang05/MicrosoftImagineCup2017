using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Com.LuisPedroFonseca.ProCamera2D;

public class TeleportToCheckpoint : MonoBehaviour
{
    public int checkpointToSwitchTo;
    public float fadeDuration;
    public Canvas[] canvases;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        //  fade out all canvases so they are not visable
        for (int i = 0; i < canvases.Length; i++)
        {
            Graphic[] graphics = canvases[i].GetComponentsInChildren<Graphic>();

            for (int j = 0; j < graphics.Length; j++)
            {
                graphics[j].CrossFadeAlpha(0, 0, false);
                //graphics[j].DOFade(0, fadeDuration);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            FadeIn(0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            FadeOut(0);
        }
    }

    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            if (Input.GetButtonDown("Interact"))
            {
                ChangeScene(checkpointToSwitchTo);
            }
        }
    }

    private void FadeOut(int canvasIndex)
    {
        Graphic[] graphics = canvases[canvasIndex].GetComponentsInChildren<Graphic>();

        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].CrossFadeAlpha(0, fadeDuration, false);
        }
    }

    private void FadeIn(int canvasIndex)
    {
        Graphic[] graphics = canvases[canvasIndex].GetComponentsInChildren<Graphic>();

        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].CrossFadeAlpha(1, fadeDuration, false);
            //graphics[i].DOFade(255, fadeDuration);
        }
    }

    public void ChangeScene(int checkpointID)
    {
        GameManager.CurrentCheckpointID = checkpointID;
        StartCoroutine(CoReloadScene());
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
