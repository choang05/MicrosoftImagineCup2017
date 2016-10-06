using UnityEngine;
using System.Collections;
using CameraTransitions;

public class PushPullObject : MonoBehaviour
{
    //  User defined parameters
    public InteractableType interactType; 
    public enum InteractableType
    {   Transferable, NonTransferable, AlwaysTransferable   };

    //  Private variables
    LayerMask originalLayer;
    GameObject playerGO;

    WorldChanger worldChanger;

    void OnEnable()
    {
        WorldChanger.OnWorldChangedState += OnWorldChangeStart;
        CameraTransition.OnTransitionComplete += OnWorldChangeComplete;
    }

    void OnDisable()
    {
        WorldChanger.OnWorldChangedState -= OnWorldChangeStart;
        CameraTransition.OnTransitionComplete -= OnWorldChangeComplete;
    }

    void Awake()
    {
        playerGO = FindObjectOfType<CharacterController2D>().gameObject;
        worldChanger = FindObjectOfType<WorldChanger>();
    }

    // Use this for initialization
    void Start ()
    {
        //  Assign the original layer of this game object
        originalLayer = gameObject.layer;
	}

    public void OnPushPullStart()
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

    private void OnWorldChangeStart(WorldChanger.WorldState worldState)
    {
        //  If the interaction type of this object is non transferable then cancel the pushpull operation of the player
        if (interactType == InteractableType.NonTransferable && playerGO.GetComponent<CharacterController2D>().pushpullObject == this)
        {
            playerGO.GetComponent<CharacterController2D>().CancelPushingPulling();
        }

        //  If the object interation type is Always Transferable, evaluate
        if (interactType == InteractableType.AlwaysTransferable && CheckWorldCollisions(worldState))
        {
            //  Set layer to player so it always displays
            gameObject.layer = playerGO.layer;

            //  Determine the new Z position of the object after world change
            switch (worldState)
            {
                case WorldChanger.WorldState.Present:
                    transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                    break;
                case WorldChanger.WorldState.Past:
                    transform.position = new Vector3(transform.position.x, transform.position.y, 25);
                    break;
                case WorldChanger.WorldState.Future:
                    transform.position = new Vector3(transform.position.x, transform.position.y, 50);
                    break;
            }
        }   
    }

    private void OnWorldChangeComplete()
    {
        //  Reset the layer if object is always transferrable type
        if (interactType == InteractableType.AlwaysTransferable)
        {
            gameObject.layer = originalLayer;
            Debug.Log("Revert layer");
        }
    }

    #region CheckWorldCollisions(): Determine which world object can be teleported too if there is non-collidable space
    private bool CheckWorldCollisions(WorldChanger.WorldState worldState)
    {
        Vector2 objectPos = new Vector2(transform.position.x, transform.position.y);
        RaycastHit hit;
        Vector3 rayDir;

        //  Evaluate the world state that player is transferring to.
        switch (worldState)
        {
            //  cast present ray & evaluate
            case WorldChanger.WorldState.Present:
                rayDir = new Vector3(objectPos.x, objectPos.y, -5);
                if (Physics.Raycast(rayDir, Vector3.forward * 10, out hit, 15))
                {
                    return false;
                }
                break;
            //  cast past ray & evaluate
            case WorldChanger.WorldState.Past:
                rayDir = new Vector3(objectPos.x, objectPos.y, 20);
                if (Physics.Raycast(rayDir, Vector3.forward * 10, out hit, 15))
                {
                    return false;
                }
                break;
            //  cast future ray & evaluate
            case WorldChanger.WorldState.Future:
                rayDir = new Vector3(objectPos.x, objectPos.y, 45);
                if (Physics.Raycast(rayDir, Vector3.forward * 10, out hit, 15))
                {
                    return false;
                }
                break;
        }

        return true;
    }
    #endregion
}
