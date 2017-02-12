using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialUILeverControl : MonoBehaviour
{

    public float fadeDuration;
    public Canvas canvas;

    void Start()
    {
        //  fade out all canvases so they are not visable
        Graphic[] graphics = canvas.GetComponentsInChildren<Graphic>();

        for (int j = 0; j < graphics.Length; j++)
        {
            graphics[j].CrossFadeAlpha(0, 0, false);
            //graphics[j].DOFade(0, fadeDuration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
                FadeIn();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Player))
            FadeOut();
    }

    private void FadeOut()
    {
        Graphic[] graphics = canvas.GetComponentsInChildren<Graphic>();

        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].CrossFadeAlpha(0, fadeDuration, false);
        }
    }

    private void FadeIn()
    {
        Graphic[] graphics = canvas.GetComponentsInChildren<Graphic>();

        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].CrossFadeAlpha(1, fadeDuration, false);
            //graphics[i].DOFade(255, fadeDuration);
        }
    }
    

}
