using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    //  Events
    public delegate void DialogueEvent();
    public static event DialogueEvent OnConversationStart;
    public static event DialogueEvent OnConversationEnd;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public static void BroadcastOnConversationStart()
    {
        if (OnConversationStart != null)
            OnConversationStart();
    }

    public static void BroadcastOnConversationEnd()
    {
        if (OnConversationEnd != null)
            OnConversationEnd();
    }
}
