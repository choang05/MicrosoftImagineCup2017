using UnityEngine;
using System.Collections;

public class ParallaxHelper : MonoBehaviour
{
    private FreeParallax parallax;
    private CharacterController2D charController;
    private Vector3 lastPlayerPosition = Vector3.zero;

	void Start()
    {
        parallax = GetComponent<FreeParallax>();

        //  Try to find character
        StartCoroutine(CoCachePlayer());
    }
    
	// Update is called once per frame
	void LateUpdate ()
    {
        if (charController == null)
            return;

        if (Vector3.Distance(charController.transform.position, lastPlayerPosition) >= 0.01f)
        {
            parallax.Speed = charController.velocity.x * -1;
        }
        else
            parallax.Speed = 0;

        lastPlayerPosition = charController.transform.position;

    }

    #region IEnumerator that checks for a indicator panel that may not have been created yet thus we need to keep checking till it exist.
    IEnumerator CoCachePlayer()
    {
        charController = FindObjectOfType<CharacterController2D>();

        //  If the charController hasn't been found yet because the character hasn't spawned yet, keep trying to find.
        while (charController == null)
        {
            charController = FindObjectOfType<CharacterController2D>();
            yield return null;
        }
    }
    #endregion
}
