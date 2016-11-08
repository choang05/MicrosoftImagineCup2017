using UnityEngine;
using System.Collections;

public class EventRopeTrapSpring : MonoBehaviour
{
    public HingeJoint mainRope;
    public Rope bottomRope;
    public Transform hookRopeSegment;
    private AudioSource trapSound;

    public int yPullTarget;

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
            StartCoroutine(CoSpringPlayerTrap(other.GetComponent<CharacterController2D>()));
        }
        else if (other.CompareTag(Tags.PushPullable))
        {
            StartCoroutine(CoSpringTrap(other.GetComponent<Rigidbody>()));
        }
    }

    IEnumerator CoSpringPlayerTrap(CharacterController2D charController)
    {
        charController.isControllable = false;
        charController.animator.SetBool(Animator.StringToHash("isGrounded"), false);
        charController.transform.SetParent(hookRopeSegment);
        charController.transform.localPosition = new Vector3(0, 1.5f, 0);
        charController.transform.localRotation = Quaternion.identity;

        mainRope.connectedAnchor = new Vector3(mainRope.connectedAnchor.x, 13, mainRope.connectedAnchor.z);

        yield return new WaitForSeconds(5);

        charController.GetComponent<PlayerDeath>().ProcessRespawn();

        Destroy(gameObject);
    }

    IEnumerator CoSpringTrap(Rigidbody rigidBody)
    {
        CharacterController2D charController = FindObjectOfType<CharacterController2D>();
        if (charController != null && charController.currentState == CharacterController2D.PlayerState.PushingPulling)
            charController.CancelPushingPulling();

        rigidBody.isKinematic = true;
        rigidBody.transform.SetParent(hookRopeSegment);
        rigidBody.transform.localPosition = Vector3.zero;
        rigidBody.transform.localRotation = Quaternion.identity;

        mainRope.connectedAnchor = new Vector3(mainRope.connectedAnchor.x, yPullTarget, mainRope.connectedAnchor.z);

        yield return null;

        Destroy(gameObject);
    }
}
