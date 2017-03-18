using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxPlayerFollower : MonoBehaviour
{
    private Transform playerTransform;

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += SetupPlayer;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= SetupPlayer;
    }

    private void SetupPlayer()
    {
        playerTransform = GameObject.FindGameObjectWithTag(Tags.Player).transform;
    }

    // Update is called once per frame
    void LateUpdate ()
    {
        if (playerTransform == null)
            return;

        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
	}
}
