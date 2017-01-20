using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingPlatform : MonoBehaviour {

    private bool isUp;

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {

    }

    IEnumerable RaisePlatform()
    {
        yield return null;
    }
    IEnumerable LowerPlatform()
    {
        yield return null;
    }
}
