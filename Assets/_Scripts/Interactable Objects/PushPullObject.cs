using UnityEngine;
using System.Collections;

public class PushPullObject : MonoBehaviour
{
    public InteractableType interactType; 
    public enum InteractableType
    {
        Transferable,
        NonTransferable,
        AlwaysTransferable
    };

    LayerMask originalLayer;

	// Use this for initialization
	void Start ()
    {
        originalLayer = gameObject.layer;
        //Debug.Log(originalLayer.value);
	}
	
	public void OnPushPullStart(GameObject playerGO)
    {
        if (interactType == InteractableType.Transferable)
        {
            gameObject.layer = playerGO.layer;
        }
    }

    public void OnPushPullEnd()
    {
        gameObject.layer = originalLayer;
    }
}
