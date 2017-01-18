using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxPlayerFollower : MonoBehaviour
{
    private Transform playerGO;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(FetchPlayerPos());
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        if (playerGO == null)
            return;

        transform.position = new Vector3(playerGO.position.x, playerGO.position.y, transform.position.z);
	}

    private IEnumerator FetchPlayerPos()
    {
        playerGO = GameObject.FindGameObjectWithTag(Tags.Player).transform;

        while (playerGO == null)
        {
            playerGO = GameObject.FindGameObjectWithTag(Tags.Player).transform;
            yield return null;
        }
    }
}
