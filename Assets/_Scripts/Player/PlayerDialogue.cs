using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDialogue : MonoBehaviour
{
    public bool isDialogueEnabled = true;
    public float fadeDuration;
    public Text barkText;
    public float textTypeDelay;
    public float barkDuration;

    private PlayerController playerController;
    private Graphic[] barkGraphics;

    //  Audio
    private AudioSource audioSource;
    public AudioClip barkStartSFX;
    public AudioClip barkEndSFX;
    //public AudioClip textTypingAudio;

    //  Animation
    private Animator animator;
    int enableBarkTriggerHash = Animator.StringToHash("enableBarkTrigger");
    int disableBarkTriggerHash = Animator.StringToHash("disableBarkTrigger");

    private void OnEnable()
    {
        DialogueManager.OnConversationStart += OnConversationStart;
        DialogueManager.OnConversationEnd += OnConversationEnd;
    }

    private void OnDisable()
    {
        DialogueManager.OnConversationStart -= OnConversationStart;
        DialogueManager.OnConversationEnd -= OnConversationEnd;
    }

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        barkGraphics = GetComponentsInChildren<Graphic>();
        audioSource = GetComponentInParent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start ()
    {
        for (int i = 0; i < barkGraphics.Length; i++)
            barkGraphics[i].CrossFadeAlpha(0, 0, false);
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0,0,0);
    }

    private void OnConversationStart()
    {
        //  Disable player control
        playerController.isControllable = false;
        playerController.velocity = Vector2.zero;
        playerController.animator.SetFloat(PlayerController.xVelocityHash, 0);
    }

    private void OnConversationEnd()
    {
        //  Enable player control
        playerController.isControllable = true;
    }

    public void Bark(string barkMessage)
    {
        StopAllCoroutines();
        StartCoroutine(CoBark(barkMessage));
    }

    IEnumerator CoBark(string barkMessage)
    {
        //  Animation
        animator.SetTrigger(enableBarkTriggerHash);

        //  Audio
        if (barkStartSFX)
        {
            audioSource.pitch = Random.Range(.7f, 1);
            audioSource.PlayOneShot(barkStartSFX);
        }

        for (int i = 0; i < barkGraphics.Length; i++)
            barkGraphics[i].CrossFadeAlpha(1, fadeDuration, false);

        StartCoroutine(CoTypeText(barkMessage));

        yield return new WaitForSeconds(barkDuration + (textTypeDelay * barkMessage.Length));

        for (int i = 0; i < barkGraphics.Length; i++)
            barkGraphics[i].CrossFadeAlpha(0, fadeDuration, false);

        //  Audio
        if (barkEndSFX)
        {
            audioSource.pitch = Random.Range(.7f, 1);
            audioSource.PlayOneShot(barkEndSFX);
        }

        //  Animation
        animator.SetTrigger(disableBarkTriggerHash);
    }

    IEnumerator CoTypeText(string message)
    {
        barkText.text = "";

        foreach (char letter in message.ToCharArray())
        {
            barkText.text += letter;

            /*if (textTypingAudio)
            {
                audioSource.pitch = Random.Range(.7f, 1);
                audioSource.PlayOneShot(textTypingAudio);
            }*/

            yield return new WaitForSeconds(textTypeDelay);
        }
    }
}
