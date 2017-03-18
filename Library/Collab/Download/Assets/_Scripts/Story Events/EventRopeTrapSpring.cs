using UnityEngine;
using System.Collections;

public class EventRopeTrapSpring : MonoBehaviour
{
    public HingeJoint mainRope;
    public Rope bottomRope;
    public Transform hookRopeSegment;

    private AudioSource audioSource;

    public int yPullTarget;
    public float yOffSet;

    private bool isTriggable = true;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isTriggable)
            return;

        if (other.CompareTag(Tags.Player))
        {
            StartCoroutine(CoSpringPlayerTrap(other.GetComponent<PlayerController>()));

            isTriggable = false;
        }
        else if (other.CompareTag(Tags.PushPullable))
        {

            StartCoroutine(CoSpringTrap(other.GetComponent<Rigidbody>()));

            isTriggable = false;
        }

        //  Audio
        audioSource.pitch = Random.Range(.8f, 1);
        audioSource.PlayOneShot(audioSource.clip);
    }

    IEnumerator CoSpringPlayerTrap(PlayerController playerController)
    {
        if (playerController.pushpullObject != null)
            playerController.CancelPushingPulling();

        yield return new WaitForSeconds(.1f);

        mainRope.connectedAnchor = new Vector3(mainRope.connectedAnchor.x, 13, mainRope.connectedAnchor.z);

        yield return new WaitForSeconds(.1f);

        playerController.isEnabled = false;
        playerController.animator.SetBool(Animator.StringToHash("isGrounded"), false);
        playerController.transform.SetParent(hookRopeSegment);
        playerController.transform.localPosition = new Vector3(0, yOffSet, 0);
        playerController.transform.localRotation = Quaternion.identity;

        yield return new WaitForSeconds(4);

        playerController.GetComponent<PlayerDeath>().ProcessRespawn();
    }

    IEnumerator CoSpringTrap(Rigidbody rigidBody)
    {
        PlayerController charController = FindObjectOfType<PlayerController>();
        if (charController != null && charController.currentState == PlayerController.PlayerState.PushingPulling)
            charController.CancelPushingPulling();

        yield return new WaitForSeconds(.1f);

        mainRope.connectedAnchor = new Vector3(mainRope.connectedAnchor.x, yPullTarget, mainRope.connectedAnchor.z);

        yield return new WaitForSeconds(.1f);

        rigidBody.isKinematic = true;
        rigidBody.transform.SetParent(hookRopeSegment);
        rigidBody.transform.localPosition = Vector3.zero;
        rigidBody.transform.localRotation = Quaternion.identity;

    }
}
