using UnityEngine;
using System.Collections;

public class EventBridgeBreaks : MonoBehaviour
{
    public HingeJoint BridgePlank1;
    public HingeJoint BridgePlank2;
    public HingeJoint[] BridgePlank2RopeSupports;
    private AudioSource BridgePlankBreakSound;

    void Awake()
    {
        BridgePlankBreakSound = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            //  Remove the box collider so it no longer triggers
            Destroy(GetComponent<BoxCollider>());

            //  Start the bridge break coroutine
            StartCoroutine(CoBreakPlanks());

            // Play wood breaking sound
            playerAudio.randomizePitch(BridgePlankBreakSound);
            BridgePlankBreakSound.volume *= playerAudio.randomVolume();
            BridgePlankBreakSound.Play();
        }
    }

    IEnumerator CoBreakPlanks()
    {
        //  Adjust hinge break force so it breaks off
        BridgePlank1.breakForce = 0;
        BridgePlank1.breakTorque = 0;
        BridgePlank2.useLimits = false;
        yield return new WaitForSeconds(.25f);
        BridgePlank2.breakForce = 0;
        BridgePlank2.breakTorque = 0;

        //  Remove hingejoints from bridgeplank support
        HingeJoint[] bridge2HingeJoints = BridgePlank2.GetComponents<HingeJoint>();
        for (int i = 0; i < bridge2HingeJoints.Length; i++)
            Destroy(bridge2HingeJoints[i]);

        //  Remove limiter on rope supports to make them swing realisticly
        for (int i = 0; i < BridgePlank2RopeSupports.Length; i++)
            BridgePlank2RopeSupports[i].useLimits = false;

        BridgePlank2RopeSupports[BridgePlank2RopeSupports.Length - 1].GetComponent<Rigidbody>().AddForce(Vector2.left * 500);

        yield return new WaitForSeconds(5);

        //  Remove gameobject event and corresponding planks
        Destroy(BridgePlank1);
        //Destroy(BridgePlank2);
        Destroy(gameObject);
    }
}
