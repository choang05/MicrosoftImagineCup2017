using UnityEngine;
using System.Collections;

public class CapePhysicsHelper : MonoBehaviour
{
    public Transform capeControlNode;
	
	// Update is called once per frame
	void Update ()
    {
        capeControlNode.transform.position = transform.position;
	}
}
