using UnityEngine;
using System.Collections;

public class CapePhysicsHelper : MonoBehaviour
{
    public Transform capeControlNode;

	// Update is called once per frame
	void LateUpdate ()
    {
        if (capeControlNode == null)
            return;

        capeControlNode.transform.position = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y, capeControlNode.position.z);
    }
}
