using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdHoustPost : MonoBehaviour
{
    public Transform landingNode;
    public GameObject birdGO;

    void SpawnBird()
    {
        float randomX = Random.Range(-15, 16);

        Vector3 spawnPos = new Vector3(randomX, 10, landingNode.position.z) + landingNode.position;

        GameObject spawnedBird = Instantiate(birdGO, spawnPos, Quaternion.identity, transform);

        Bird bird = spawnedBird.GetComponent<Bird>();
        bird.xJumpRange = Vector2.zero;
        bird.jumpPower = .5f;

        bird.FlyTo(landingNode.position, 3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            SpawnBird();

            Destroy(GetComponent<BoxCollider>());

            Destroy(this);
        }
    }
}
