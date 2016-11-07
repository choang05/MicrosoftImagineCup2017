using UnityEngine;
using System.Collections;

public class WorldChangeSetter : MonoBehaviour
{
    public bool isPresentAvaliable;
    public bool isPastAvaliable;
    public bool isFutureAvaliable;

    WorldChanger worldChanger;

    // Use this for initialization
    void Awake()
    {
        worldChanger = FindObjectOfType<WorldChanger>();
    }

    //  Called when a collider enters another collider with isTrigger enabled
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            //  Set world changer availibilities
            worldChanger.isPresentAvaliable = isPastAvaliable;
            worldChanger.isPastAvaliable = isFutureAvaliable;
            worldChanger.isFutureAvaliable = isFutureAvaliable;
        }
    }
}
