using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldIndicator : MonoBehaviour
{
    public enum IndicatorType { Past, Present, Future };
    public IndicatorType indicatorType;

    private RectTransform rTransform;
    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        rTransform = GetComponent<RectTransform>();
        rTransform.localScale = new Vector3(0, 0, 0);
    }

    void Start()
    {
        UpdateIndicatorScale(WorldChanger.WorldState.Present);
    }

    public void OnEnable()
    {
        Wisp.OnWispAdd += ActivateIndicator;
        PauseMenu.OnPauseMenuActivated += HideIndicator;
        PauseMenu.OnPauseMenuDeactivated += ShowIndicator;
        WorldChanger.OnWorldChangeComplete += UpdateIndicatorScale;
    }
    public void OnDisable()
    {
        Wisp.OnWispAdd -= ActivateIndicator;
        PauseMenu.OnPauseMenuActivated -= HideIndicator;
        PauseMenu.OnPauseMenuDeactivated -= ShowIndicator;
        WorldChanger.OnWorldChangeComplete -= UpdateIndicatorScale;
    }

    public IEnumerator GrowIndicator(float growSize, float speed)
    {
        for (float f = rTransform.localScale.x; f <= growSize; f += speed)
        {
            rTransform.localScale = new Vector3(f, f, 1);
            yield return null;
        }
    }
    public IEnumerator ShrinkIndicator(float shrinkSize, float speed)
    {
        for (float f = rTransform.localScale.x; f >= shrinkSize; f -= speed)
        {
            rTransform.localScale = new Vector3(f, f, 1);
            yield return null;
        }
    }
    public void HideIndicator()
    {
        StartCoroutine(ShrinkIndicator(0.0f, 0.2f));
    }

    public void ShowIndicator()
    {
        if (rTransform.localScale.Equals(new Vector3(0.0f, 0.0f, 0.0f)))
        {
            if (indicatorType == IndicatorType.Present && gameManager.hasPresentWisp)
            {
                StartCoroutine(GrowIndicator(1.0f, 0.2f));
            }
            else if (indicatorType == IndicatorType.Future && gameManager.hasFutureWisp)
            {
                StartCoroutine(GrowIndicator(1.0f, 0.2f));
            }
            else if (indicatorType == IndicatorType.Past && gameManager.hasPastWisp)
            {
                StartCoroutine(GrowIndicator(1.0f, 0.2f));
            }
        }
    }

    //  Enlarge the indicator relative to the world state when world change completes
    private void UpdateIndicatorScale(WorldChanger.WorldState worldState)
    {
        float growSize = 1.5f;

        // Shrink this indicators scale to normal size
        StartCoroutine(ShrinkIndicator(1.0f, 0.2f));

        //  Enlarge the new world indicator
        if (indicatorType == IndicatorType.Present && gameManager.hasPresentWisp && worldState == WorldChanger.WorldState.Present)
        {
            StartCoroutine(GrowIndicator(growSize, 0.2f));
        }
        else if (indicatorType == IndicatorType.Future && gameManager.hasFutureWisp && worldState == WorldChanger.WorldState.Future)
        {
            StartCoroutine(GrowIndicator(growSize, 0.2f));
        }
        else if (indicatorType == IndicatorType.Past && gameManager.hasPastWisp && worldState == WorldChanger.WorldState.Past)
        {
            StartCoroutine(GrowIndicator(growSize, 0.2f));
        }
    }

    //  Activate the indicator when a wisp has been added
    private void ActivateIndicator(Wisp.WispType wispType)
    {
        if (wispType == Wisp.WispType.Present && indicatorType == IndicatorType.Present)
        {
            StartCoroutine(GrowIndicator(1.0f, 0.2f));
        }
        else if (wispType == Wisp.WispType.Past && indicatorType == IndicatorType.Past)
        {
            StartCoroutine(GrowIndicator(1.0f, 0.2f));
        }
        else if (wispType == Wisp.WispType.Future && indicatorType == IndicatorType.Future)
        {
            StartCoroutine(GrowIndicator(1.0f, 0.2f));
        }
    }
}
