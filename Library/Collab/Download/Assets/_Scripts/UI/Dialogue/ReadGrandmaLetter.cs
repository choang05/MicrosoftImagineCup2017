using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadGrandmaLetter : MonoBehaviour
{
    public float fadeDuration;
    public Canvas promptUICanvas;
    public Canvas letterUICanvas;

    private Graphic[] promptGraphics;
    private Graphic[] letterGraphics;
    private bool isLetterUIActive = false;
    private bool isLetterUsable = true;

    private UnityStandardAssets.ImageEffects.BlurOptimized blurComponent;

    private AudioSource audioSource;
    public AudioClip letterSound;

    private void Awake()
    {
        promptGraphics = promptUICanvas.GetComponentsInChildren<Graphic>();
        letterGraphics = letterUICanvas.GetComponentsInChildren<Graphic>();
        blurComponent = Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized>();
        audioSource = GetComponentInParent<AudioSource>();
    }

    void Start()
    {
        for (int i = 0; i < promptGraphics.Length; i++)
            promptGraphics[i].CrossFadeAlpha(0, 0, false);

        letterUICanvas.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tags.Player))
            return;

        ToggleInteractUI(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Tags.Player))
            return;

        ToggleInteractUI(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(Tags.Player))
            return;

        if (Input.GetButtonDown("Interact"))
        {
            if (isLetterUIActive && isLetterUsable)
            {
                StartCoroutine(CoToggleLetterUI(false));

                //  Toggle camera blur
                blurComponent.enabled = false;

                audioSource.pitch = Random.Range(.7f, 1);
                audioSource.PlayOneShot(letterSound);
            }
            else if (!isLetterUIActive && isLetterUsable)
            {
                StartCoroutine(CoToggleLetterUI(true));

                //  Toggle camera blur
                blurComponent.enabled = true;

                audioSource.pitch = Random.Range(.7f, 1);
                audioSource.PlayOneShot(letterSound);
            }
        }
    }

    private void ToggleInteractUI(bool toggle)
    {
        if (toggle)
        {
            for (int i = 0; i < promptGraphics.Length; i++)
                promptGraphics[i].CrossFadeAlpha(1, fadeDuration, false);
        }
        else
        {
            for (int i = 0; i < promptGraphics.Length; i++)
                promptGraphics[i].CrossFadeAlpha(0, fadeDuration, false);
        }
    }

    IEnumerator CoToggleLetterUI(bool toggle)
    {
        if (toggle)
        {
            isLetterUsable = false;

            letterUICanvas.gameObject.SetActive(true);
            for (int i = 0; i < letterGraphics.Length; i++)
                letterGraphics[i].CrossFadeAlpha(0, 0, false);

            DialogueManager.BroadcastOnConversationStart();

            isLetterUIActive = true;
            for (int i = 0; i < letterGraphics.Length; i++)
                letterGraphics[i].CrossFadeAlpha(1, fadeDuration, false);

            yield return new WaitForSeconds(fadeDuration);

            isLetterUsable = true;
        }
        else
        {
            isLetterUIActive = false;
            isLetterUsable = false;

            for (int i = 0; i < letterGraphics.Length; i++)
                letterGraphics[i].CrossFadeAlpha(0, fadeDuration, false);

            yield return new WaitForSeconds(fadeDuration);

            DialogueManager.BroadcastOnConversationEnd();

            letterUICanvas.gameObject.SetActive(false);

            isLetterUsable = true;
        }
    }
}
