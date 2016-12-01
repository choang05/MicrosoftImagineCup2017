using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldIndicator : MonoBehaviour {

    public enum World { Past, Present, Future };
    public World world;
    public RectTransform rTransform;
    public GameManager gameManager;
    public void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        rTransform = GetComponent<RectTransform>();
        rTransform.localScale = new Vector3(0, 0, 0);
    }

    public void OnEnable()
    {
        Checkpoint.OnCheckpointReached += ShowHideIndicators;
        PauseMenu.OnPauseMenuActivated += HideAllIndicators;
        PauseMenu.OnPauseMenuDeactivated += ShowHideIndicators;
    }
    public void OnDisable()
    {
        Checkpoint.OnCheckpointReached -= ShowHideIndicators;
        PauseMenu.OnPauseMenuActivated -= HideAllIndicators;
        PauseMenu.OnPauseMenuDeactivated -= ShowHideIndicators;
    }
    public IEnumerator GrowIndicator(float growSize)
    {
        for(float f = 0.0f; f <= growSize; f+=0.2f)
        {
            rTransform.localScale = new Vector3(f, f, 1);
            yield return null;
        }
    }
    public IEnumerator ShrinkIndicator(float shrinkSize)
    {
        for (float f = rTransform.localScale.x; f >= shrinkSize; f -= 0.2f)
        {
            rTransform.localScale = new Vector3(f, f, 1);
            yield return null;
        }
    }
    public void HideAllIndicators()
    {
        StartCoroutine(ShrinkIndicator(0.0f));
    }
    public void ShowHideIndicators()
    {
        if (rTransform.localScale.Equals(new Vector3(0.0f, 0.0f, 0.0f)))
        {
            if(world == World.Present)
            {
                // Chad - code to grow present indicator size gradually using animation goes here. Scale the size of this gameobject to 1.5
                StartCoroutine(GrowIndicator(1.5f));
            }
            if(gameManager.CurrentCheckpointID >= 55 && gameManager.CurrentCheckpointID < 100)
            {
                if(world == World.Future)
                {
                    // Chad - code to grow future indicator size gradually using animation goes here. Scale the size of this gameobject to 1.0
                    StartCoroutine(GrowIndicator(1.0f));
                }
            }
            else if(gameManager.CurrentCheckpointID >= 100)
            {
                if(world == World.Past || world == World.Future)
                {
                    // Chad - code to grow past/future indicator size gradually using animation goes here. Scale the size of this gameobject to 1.0
                    StartCoroutine(GrowIndicator(1.0f));
                }
            }
        }
        

    }
	
}
