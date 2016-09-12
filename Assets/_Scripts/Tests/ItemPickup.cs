using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour
{
    bool hasItem;

    Transform heldItem;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q) && hasItem)
        {
            Dropitem();
        }

        if (hasItem)
        {
            //heldItem.transform.position = transform.position + Vector3.up * 2;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!hasItem && other.CompareTag(Tags.Item))
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                Pickup(other.transform);
            }
            //Debug.Log(other.name + " is pickable!");
        }
    }

    void Pickup(Transform item)
    {
        heldItem = item;

        heldItem.GetComponent<BoxCollider2D>().enabled = false;
        heldItem.GetComponent<Rigidbody2D>().isKinematic = true;

        heldItem.SetParent(transform);
        heldItem.transform.position = transform.position + Vector3.up * 1.2f;

        hasItem = true;

        Debug.Log("Picked up item!");
    }

    void Dropitem()
    {
        if (transform.parent.rotation.y == 0)
            heldItem.transform.position = transform.position + Vector3.right * 1.0f;
        else
            heldItem.transform.position = transform.position + Vector3.left * 1.0f;

        //heldItem.SetParent(FindObjectOfType<WorldChanger>().currentWorld.transform);

        heldItem.GetComponent<BoxCollider2D>().enabled = true;
        heldItem.GetComponent<Rigidbody2D>().isKinematic = true;

        //  Set material
        //heldItem.GetComponentInChildren<Renderer>().material.shader = FindObjectOfType<WorldChanger>().ObjectOneShader;

        hasItem = false;

        Debug.Log("Dropped item!");
    }
}
