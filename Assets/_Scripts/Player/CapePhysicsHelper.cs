using UnityEngine;
using System.Collections;

public class CapePhysicsHelper : MonoBehaviour
{
    public Transform capeControlNode;
	
    void Start()
    {
        transform.position = FindObjectOfType<CharacterController2D>().transform.position;
    }

	// Update is called once per frame
	void LateUpdate ()
    {
        capeControlNode.transform.position = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y, capeControlNode.position.z);
	}
}
