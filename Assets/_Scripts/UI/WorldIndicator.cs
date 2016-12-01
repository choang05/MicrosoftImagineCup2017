using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldIndicator : MonoBehaviour
{
    public WorldChanger.WorldState indicatorType;

    private RectTransform rTransform;
    private GameManager gameManager;
    private UnityEngine.UI.Button button;
    private WorldChanger worldChanger;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        worldChanger = FindObjectOfType<WorldChanger>();
        rTransform = GetComponent<RectTransform>();
        button = GetComponent<UnityEngine.UI.Button>();
        rTransform.localScale = new Vector3(0, 0, 1);
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
        WorldChanger.OnWorldChangeStart += UpdateIndicatorScale;
    }
    public void OnDisable()
    {
        Wisp.OnWispAdd -= ActivateIndicator;
        PauseMenu.OnPauseMenuActivated -= HideIndicator;
        PauseMenu.OnPauseMenuDeactivated -= ShowIndicator;
        WorldChanger.OnWorldChangeStart -= UpdateIndicatorScale;
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

    public IEnumerator DisableIndicator()
    {
        for (float f = button.colors.disabledColor.r; f >= 0.39f; f -= 0.2f)
        {
            UnityEngine.UI.ColorBlock buttonColors = button.colors;
            buttonColors.disabledColor = new Color(f, f, f, 1.0f);
            button.colors = buttonColors;
            yield return null;
        }
    }

    public IEnumerator EnableIndicator()
    {
        for (float f = button.colors.disabledColor.r; f <= 1.0f; f += 0.2f)
        {
            UnityEngine.UI.ColorBlock buttonColors = button.colors;
            buttonColors.disabledColor = new Color(f, f, f, 1.0f);
            button.colors = buttonColors;
            yield return null;
        }
    }

    public void HideIndicator()
    {
        StartCoroutine(ShrinkIndicator(0.0f, 0.2f));
    }

    public void ShowIndicator()
    {
        if (indicatorType == WorldChanger.WorldState.Present && gameManager.hasPresentWisp)
        {
            StartCoroutine(GrowIndicator(1.0f, 0.2f));
        }
        else if (indicatorType == WorldChanger.WorldState.Future && gameManager.hasFutureWisp)
        {
            StartCoroutine(GrowIndicator(1.0f, 0.2f));
        }
        else if (indicatorType == WorldChanger.WorldState.Past && gameManager.hasPastWisp)
        {
            StartCoroutine(GrowIndicator(1.0f, 0.2f));
        }
        UpdateIndicatorScale(worldChanger.currentWorldState);
    }

    //  Enlarge the indicator relative to the world state when world change completes
    private void UpdateIndicatorScale(WorldChanger.WorldState worldState)
    {
        float growSize = 1.5f;
        if (indicatorType == worldState && gameManager.hasPresentWisp)
            StartCoroutine(GrowIndicator(growSize, 0.2f));
        else
            StartCoroutine(ShrinkIndicator(1.0f, 0.2f)); // Shrink other indicators scale to normal size
    /*
        //  Enlarge the new world indicator
        if (indicatorType == worldState && gameManager.hasPresentWisp)
        {
            StartCoroutine(GrowIndicator(growSize, 0.2f));
        }
        else if (indicatorType == worldState && gameManager.hasFutureWisp)
        {
            StartCoroutine(GrowIndicator(growSize, 0.2f));
        }
        else if (indicatorType == worldState && gameManager.hasPastWisp)
        {
            StartCoroutine(GrowIndicator(growSize, 0.2f));
        }
    */
    }

    //  Activate the indicator when a wisp has been added
    private void ActivateIndicator(Wisp.WispType wispType)
    {
        if (wispType == Wisp.WispType.Present && indicatorType == WorldChanger.WorldState.Present)
        {
            StartCoroutine(GrowIndicator(1.0f, 0.2f));
        }
        else if (wispType == Wisp.WispType.Past && indicatorType == WorldChanger.WorldState.Past)
        {
            StartCoroutine(GrowIndicator(1.0f, 0.2f));
        }
        else if (wispType == Wisp.WispType.Future && indicatorType == WorldChanger.WorldState.Future)
        {
            StartCoroutine(GrowIndicator(1.0f, 0.2f));
        }
    }

    void Update()
    {
        if(indicatorType == WorldChanger.WorldState.Past)
        {
            if (worldChanger.canSwitchPast && gameManager.hasPastWisp)
                StartCoroutine(EnableIndicator());
            else if (!worldChanger.canSwitchPast && gameManager.hasPastWisp)
                StartCoroutine(DisableIndicator());
            else
                HideIndicator();
        }
        else if (indicatorType == WorldChanger.WorldState.Present)
        {
            if (worldChanger.canSwitchPresent && gameManager.hasPresentWisp)
                StartCoroutine(EnableIndicator());
            else if (!worldChanger.canSwitchPresent && gameManager.hasPresentWisp)
                StartCoroutine(DisableIndicator());
            else
                HideIndicator();
        }
        else if (indicatorType == WorldChanger.WorldState.Future)
        {
            if (worldChanger.canSwitchFuture && gameManager.hasFutureWisp)
                StartCoroutine(EnableIndicator());
            else if (!worldChanger.canSwitchFuture && gameManager.hasFutureWisp)
                StartCoroutine(DisableIndicator());
            else
                HideIndicator();
        }
    }
}


