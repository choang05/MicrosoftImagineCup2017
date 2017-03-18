using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI_BoxTransfer : MonoBehaviour
{
    public Transform parentTransform;
    public Transform followTarget;
    public Vector2 offset;
    public int acceptedPushPullObjectID;
    public float fadeDuration;
    public Canvas[] canvases;

    private TutorialState tutorialState;
    enum TutorialState
    {
        Step1,
        Step2
    };

    void OnEnable()
    {
        PlayerController.OnPushPullStart += OnPushPullStart;
        PlayerController.OnPushPullEnd += OnPushPullEnd;
    }

    void OnDisable()
    {
        PlayerController.OnPushPullStart -= OnPushPullStart;
        PlayerController.OnPushPullEnd -= OnPushPullEnd;
    }

    void Start()
    {
        tutorialState = TutorialState.Step1;

        //  fade out all canvases so they are not visable
        for (int i = 0; i < canvases.Length; i++)
        {
            Graphic[] graphics = canvases[i].GetComponentsInChildren<Graphic>();

            for (int j = 0; j < graphics.Length; j++)
                graphics[j].CrossFadeAlpha(0, 0, false);
        }
    }

    private void LateUpdate()
    {
        /*if (isActive)
        {
            canvas.transform.DOScale(1.1f, 1);
            canvas.transform.DOScale(1f, 1);
        }*/
        parentTransform.position = new Vector3(followTarget.position.x + offset.x, followTarget.position.y + offset.y, parentTransform.position.z);
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player) && tutorialState == TutorialState.Step1)
        {
            FadeIn(0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Player) && tutorialState == TutorialState.Step1)
        {
            FadeOut(0);
        }
    }

    private void OnPushPullStart(PushPullObject pushPullObject)
    {
        if (acceptedPushPullObjectID != pushPullObject.PushPullObjectID)
            return;

        FadeOut(0);
        FadeIn(1);

        tutorialState = TutorialState.Step2;
    }

    private void OnPushPullEnd(PushPullObject pushPullObject)
    {
        if (acceptedPushPullObjectID != pushPullObject.PushPullObjectID)
            return;

        FadeOut(0);
        FadeOut(1);

        tutorialState = TutorialState.Step1;
    }

    private void FadeOut(int canvasIndex)
    {
        Graphic[] graphics = canvases[canvasIndex].GetComponentsInChildren<Graphic>();

        for (int i = 0; i < graphics.Length; i++)
            graphics[i].CrossFadeAlpha(0, fadeDuration, false);
    }

    private void FadeIn(int canvasIndex)
    {
        Graphic[] graphics = canvases[canvasIndex].GetComponentsInChildren<Graphic>();

        for (int i = 0; i < graphics.Length; i++)
            graphics[i].CrossFadeAlpha(1, fadeDuration, false);
    }
}

