using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RandomScale : MonoBehaviour
{
    public float min;
    public float max;

    void LateUpdate()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            float randomScale = Random.Range(min, max);
            transform.localScale = new Vector3(randomScale, randomScale, 1);

            //enabled = false;
        }
        else
            Destroy(this);
	}
}
