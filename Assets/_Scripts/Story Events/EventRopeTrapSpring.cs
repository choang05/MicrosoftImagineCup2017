using UnityEngine;
using System.Collections;

public class EventRopeTrapSpring : MonoBehaviour
{
    public HingeJoint mainRope;
    public Rope bottomRope;
    public Transform hookRopeSegment;
    private AudioSource trapSound;

    public int yPullTarget;
    public float yOffSet;

    void Awake()
    {
        trapSound = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        playerAudio.randomizePitch(trapSound);
        trapSound.PlayOneShot(trapSound.clip, playerAudio.randomVolume());

        if (other.CompareTag(Tags.Player))
        {
            StartCoroutine(CoSpringPlayerTrap(other.GetComponent<PlayerController>()));
        }
        else if (other.CompareTag(Tags.PushPullable))
        {

            StartCoroutine(CoSpringTrap(other.GetComponent<Rigidbody>()));
        }
    }

    IEnumerator CoSpringPlayerTrap(PlayerController charController)
    {
        charController.isEnabled = false;
        charController.animator.SetBool(Animator.StringToHash("isGrounded"), false);
        charController.transform.SetParent(hookRopeSegment);
        charController.transform.localPosition = new Vector3(0, yOffSet, 0);
        charController.transform.localRotation = Quaternion.identity;

        mainRope.connectedAnchor = new Vector3(mainRope.connectedAnchor.x, 13, mainRope.connectedAnchor.z);

        yield return new WaitForSeconds(5);

        charController.GetComponent<PlayerDeath>().ProcessRespawn();

        Destroy(GetComponent<BoxCollider>());
    }

    IEnumerator CoSpringTrap(Rigidbody rigidBody)
    {
        PlayerController charController = FindObjectOfType<PlayerController>();
        if (charController != null && charController.currentState == PlayerController.PlayerState.PushingPulling)
            charController.CancelPushingPulling();

        rigidBody.isKinematic = true;
        rigidBody.transform.SetParent(hookRopeSegment);
        rigidBody.transform.localPosition = Vector3.zero;
        rigidBody.transform.localRotation = Quaternion.identity;

        mainRope.connectedAnchor = new Vector3(mainRope.connectedAnchor.x, yPullTarget, mainRope.connectedAnchor.z);

        yield return null;

        Destroy(GetComponent<BoxCollider>());

    }
}
