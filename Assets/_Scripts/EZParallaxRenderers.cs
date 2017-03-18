using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EZParallaxRenderers : MonoBehaviour
{
    public GameObject PresentEZParallax;
    public GameObject PastEZParallax;
    public GameObject FutureEZParallax;

    private void OnEnable()
    {
        WorldChanger.OnWorldChangeStart += OnWorldChangeStart;
        WorldChanger.OnWorldChangeComplete += OnWorldChangeComplete;
    }
    private void OnDisable()
    {
        WorldChanger.OnWorldChangeStart -= OnWorldChangeStart;
        WorldChanger.OnWorldChangeComplete -= OnWorldChangeComplete;
    }

    private void Start()
    {
        //  Disable the parallaxes that aren't in use
        if (WorldChanger.CurrentWorldState != WorldChanger.WorldState.Present)
            PresentEZParallax.SetActive(false);
        if (WorldChanger.CurrentWorldState != WorldChanger.WorldState.Past)
            PastEZParallax.SetActive(false);
        if (WorldChanger.CurrentWorldState != WorldChanger.WorldState.Future)
            FutureEZParallax.SetActive(false);
    }

    private void OnWorldChangeComplete(WorldChanger.WorldState newWorldState)
    {
        //  Disable the parallaxes that aren't in use
        if (WorldChanger.CurrentWorldState != WorldChanger.WorldState.Present)
            PresentEZParallax.SetActive(false);
        if (WorldChanger.CurrentWorldState != WorldChanger.WorldState.Past)
            PastEZParallax.SetActive(false);
        if (WorldChanger.CurrentWorldState != WorldChanger.WorldState.Future)
            FutureEZParallax.SetActive(false);
    }
    private void OnWorldChangeStart(WorldChanger.WorldState newWorldState)
    {
        //  Set active the one we are transitioning too
        if (newWorldState == WorldChanger.WorldState.Present)
            PresentEZParallax.SetActive(true);
        else if (newWorldState == WorldChanger.WorldState.Past)
            PastEZParallax.SetActive(true);
        else if (newWorldState == WorldChanger.WorldState.Future)
            FutureEZParallax.SetActive(true);
    }
}
