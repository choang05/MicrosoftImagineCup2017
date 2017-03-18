using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialUI_Simple : MonoBehaviour
{
    public float fadeDuration;
    public Canvas canvas;

    private Graphic[] graphics;

    private bool isActive;

    void Start()
    {
        graphics = canvas.GetComponentsInChildren<Graphic>();

        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].CrossFadeAlpha(0, 0, false);
            //graphics[i].DOFade(0, fadeDuration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            for (int i = 0; i < graphics.Length; i++)
            {
                graphics[i].CrossFadeAlpha(1, fadeDuration, false);
                //graphics[i].DOFade(255, fadeDuration);
            }

            isActive = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            for (int i = 0; i < graphics.Length; i++)
            {
                graphics[i].CrossFadeAlpha(0, fadeDuration, false);
            }

            isActive = true;
        }
    }
}
