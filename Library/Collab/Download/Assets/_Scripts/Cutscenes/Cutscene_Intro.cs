using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;

public class Cutscene_Intro : MonoBehaviour
{
    public DOTweenPath cameraTarget_DOTweenPath;
    public DOTweenPath wispPresent_DOTweenPath;
    public DOTweenPath wispPast_DOTweenPath;
    public DOTweenPath wispFuture_DOTweenPath;

    //  UI
    public Image Image_TeamVogel;
    public Text Text_Presents;
    public Text Text_GameName;
    public RectTransform Letterbox_Bottom;
    public RectTransform Letterbox_Top;

    private ProCamera2DTransitionsFX proCamera2DTransitionFx;

    private AudioSource audioSource;

    AsyncOperation async;

    void OnEnable()
    {
        SceneManager.sceneLoaded += SetUpScene;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= SetUpScene;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        proCamera2DTransitionFx = FindObjectOfType<ProCamera2DTransitionsFX>();
    }

    // Use this for initialization
    void Start()
    {
        Image_TeamVogel.CrossFadeAlpha(0, 0, false);
        Text_Presents.CrossFadeAlpha(0, 0, false);
        Text_GameName.CrossFadeAlpha(0, 0, false);

        async = SceneManager.LoadSceneAsync(1);
        async.allowSceneActivation = false;
    }

    //  Sets up the scene on scene loaded
    private void SetUpScene(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(CoStartCinematic());
    }

    IEnumerator CoStartCinematic()
    {
        proCamera2DTransitionFx.TransitionEnter();

        yield return new WaitForSeconds(proCamera2DTransitionFx.DurationEnter);

        Letterbox_Bottom.DOSizeDelta(new Vector2(Letterbox_Bottom.rect.width, 50), 1);
        Letterbox_Top.DOSizeDelta(new Vector2(Letterbox_Top.rect.width, 50), 1);

        wispPresent_DOTweenPath.DOPlay();

        yield return new WaitForSeconds(2.5f);
        wispFuture_DOTweenPath.DOPlay();

        yield return new WaitForSeconds(2.5f);

        cameraTarget_DOTweenPath.DOPlay();

        yield return new WaitForSeconds(2f);

        Image_TeamVogel.CrossFadeAlpha(1, 1, false);
        audioSource.PlayOneShot(audioSource.clip);

        yield return new WaitForSeconds(3);

        wispPast_DOTweenPath.DOPlay();

        yield return new WaitForSeconds(1f);

        Image_TeamVogel.CrossFadeAlpha(0, 1, false);

        yield return new WaitForSeconds(4.5f);

        Text_Presents.CrossFadeAlpha(1, 1, false);
        audioSource.PlayOneShot(audioSource.clip);

        yield return new WaitForSeconds(4f);

        Text_Presents.CrossFadeAlpha(0, 1, false);

        yield return new WaitForSeconds(4.5f);

        Text_GameName.CrossFadeAlpha(1, 1, false);
        audioSource.PlayOneShot(audioSource.clip);

        yield return new WaitForSeconds(4f);

        Text_GameName.CrossFadeAlpha(0, 1, false);
    }

    public void OnPathComplete()
    {
        StartCoroutine(CoTransitionToMasterScene());
    }

    IEnumerator CoTransitionToMasterScene()
    {
        proCamera2DTransitionFx.TransitionExit();

        yield return new WaitForSeconds(proCamera2DTransitionFx.DurationExit);

        async.allowSceneActivation = true;
    }
}
