using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RandomLayer : MonoBehaviour
{
    public int[] layers;

    void LateUpdate()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            // float randomScale = Random.Range(min, max);
            //transform.localScale = new Vector3(randomScale, randomScale, 1);
            //sprite
            //enabled = false;
            GetComponent<SpriteRenderer>().sortingOrder = layers[Random.Range(0, layers.Length)];
        }
        else
            Destroy(this);
	}
}
