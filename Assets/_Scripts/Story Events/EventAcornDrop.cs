using UnityEngine;
using System.Collections;

public class EventAcornDrop : MonoBehaviour
{
    public GameObject Acorn;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            //  Start the acorn drop coroutine
            StartCoroutine(CoDropAcorn());
        }
    }

    IEnumerator CoDropAcorn()
    {
        //  PLAY AUDIO OF SOMETHING ABOUT TO FALL FROM THE LEAVES OF A TREE.

        yield return new WaitForSeconds(1);

        Acorn.SetActive(true);

        Rigidbody rigidBody = Acorn.GetComponent<Rigidbody>();
        rigidBody.isKinematic = false;

        Destroy(gameObject);
    }
}
