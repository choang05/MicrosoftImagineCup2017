using UnityEngine;
using System.Collections;

public class CapePhysicsHelper : MonoBehaviour
{
    public Transform capeControlNode;

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += OnPlayerSpawned;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= OnPlayerSpawned;
    }

    private void Start()
    {
        //OnPlayerSpawned();
    }

    // Update is called once per frame
    void LateUpdate ()
    {
        if (capeControlNode == null)
            return;

        capeControlNode.transform.position = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y, capeControlNode.position.z);
    }

    private void OnPlayerSpawned()
    {
        //  Set up the cape helper
        transform.position = GameObject.FindGameObjectWithTag(Tags.Player).transform.position;
        capeControlNode = GameObject.FindGameObjectWithTag(Tags.bone_Cape_CTRL).transform;
        GetComponent<DistanceJoint2D>().connectedBody = GameObject.FindGameObjectWithTag(Tags.bone_Cape).GetComponent<Rigidbody2D>();
    }
}
