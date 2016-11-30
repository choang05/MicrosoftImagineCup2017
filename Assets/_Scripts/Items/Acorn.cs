using UnityEngine;
using System.Collections;

public class Acorn : MonoBehaviour
{
    public void DestroyCollisions()
    {
        BoxCollider[] boxColls = GetComponents<BoxCollider>();
        for (int i = 0; i < boxColls.Length; i++)
            Destroy(boxColls[i]);

        Destroy(GetComponent<Rigidbody>());
    }
}
