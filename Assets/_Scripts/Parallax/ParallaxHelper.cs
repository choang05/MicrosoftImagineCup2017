using UnityEngine;
using System.Collections;

public class ParallaxHelper : MonoBehaviour
{
    private FreeParallax parallax;
    private CharacterController2D charController;

	void Start()
    {
        parallax = GetComponent<FreeParallax>();
        charController = FindObjectOfType<CharacterController2D>();
    }
    
	// Update is called once per frame
	void Update ()
    {
        parallax.Speed = charController.velocity.x * -1;

    }
}
