using UnityEngine;
using System.Collections;

public class PushPullObject : MonoBehaviour
{
    //  User defined parameters
    public InteractableType interactType; 
    public enum InteractableType
    {   Transferable, NonTransferable, AlwaysTransferable   };

    //  Private variables
    LayerMask originalLayer;
    GameObject playerGO;

    void OnEnable()
    {
        WorldChanger.OnWorldChanged += EvaluateWorldChange;
    }

    void OnDisable()
    {
        WorldChanger.OnWorldChanged -= EvaluateWorldChange;
    }

    // Use this for initialization
    void Start ()
    {
        //  Assign the original layer of this game object
        originalLayer = gameObject.layer;
	}
	
	public void OnPushPullStart(GameObject playergo)
    {
        playerGO = playergo;

        if (interactType == InteractableType.Transferable)
        {
            gameObject.layer = playerGO.layer;
        }
    }

    public void OnPushPullEnd()
    {
        playerGO = null;

        gameObject.layer = originalLayer;
    }

    private void EvaluateWorldChange(WorldChanger.WorldState worldState)
    {
        if (playerGO != null)
        {
            //  If the interaction type of this object is non transferable then cancel the pushpull operation of the player
            if (interactType == InteractableType.NonTransferable)
            {
                playerGO.GetComponent<CharacterController2D>().CancelPushingPulling();
            }
        }   
    }
}
