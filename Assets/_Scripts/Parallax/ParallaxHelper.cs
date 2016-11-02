using UnityEngine;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;

public class ParallaxHelper : MonoBehaviour
{
    public float parallaxSpeedMult;

    private FreeParallax parallax;

    void Awake()
    {
        parallax = GetComponent<FreeParallax>();
    }
    
	// Update is called once per frame
	void LateUpdate ()
    {
        parallax.Speed = ProCamera2D.Instance.GameCamera.velocity.x * parallaxSpeedMult * -1;
    }
}
