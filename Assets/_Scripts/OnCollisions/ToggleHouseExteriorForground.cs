using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ToggleHouseExteriorForground : MonoBehaviour
{
    public GameObject foregroundGO;
    public float fadeTime;

    private SpriteRenderer[] sprites;
    private Renderer[] renderers;

	// Use this for initialization
	void Start ()
    {
        sprites = foregroundGO.GetComponentsInChildren<SpriteRenderer>();
        renderers = foregroundGO.GetComponentsInChildren<Renderer>();

        //  Initially hide all mats and sprites
        for (int i = 0; i < sprites.Length; i++)
            sprites[i].DOFade(0, 0);
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.DOFade(0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tags.Player))
            return;

        ToggleForeground(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Tags.Player))
            return;

        ToggleForeground(true);
    }

    public void ToggleForeground(bool isOn)
    {
        if (isOn)
        {
            for (int i = 0; i < sprites.Length; i++)
                sprites[i].DOFade(255, fadeTime);
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].material.DOFade(255, fadeTime);

            Debug.Log("Show foreground.");
        }
        else
        {
            for (int i = 0; i < sprites.Length; i++)
                sprites[i].DOFade(0, fadeTime);
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].material.DOFade(0, fadeTime);

            Debug.Log("Hide foreground.");
        }
    }
}
