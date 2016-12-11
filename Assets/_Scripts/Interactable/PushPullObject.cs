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
    private LayerMask originalLayer;
    private Rigidbody rigidBody;
    private BoxCollider boxColl;
    [HideInInspector] public bool isColliding;
    private CharacterController2D playerController;
    private WorldChanger worldChanger;
    public WorldChanger.WorldState currentWorldState;

    void OnEnable()
    {
        WorldChanger.OnWorldChangeStart += EvaluateTransitionStart;
        WorldChanger.OnWorldChangeComplete += EvaluateTransitionComplete;
    }

    void OnDisable()
    {
        WorldChanger.OnWorldChangeStart -= EvaluateTransitionStart;
        WorldChanger.OnWorldChangeComplete -= EvaluateTransitionComplete;
    }

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        boxColl = GetComponent<BoxCollider>();
        worldChanger = FindObjectOfType<WorldChanger>();
    }

    // Use this for initialization
    void Start ()
    {
        playerController = FindObjectOfType<CharacterController2D>();
        while (playerController == null)
            playerController = FindObjectOfType<CharacterController2D>();
        
        //  Assign the original layer of this game object
        originalLayer = gameObject.layer;

        //  Determine current worldstate
        if (transform.position.z - 0 < .1f)
            currentWorldState = WorldChanger.WorldState.Present;
        else if (transform.position.z - 50 < .1f)
            currentWorldState = WorldChanger.WorldState.Past;
        else if (transform.position.z - 100 < .1f)
            currentWorldState = WorldChanger.WorldState.Future;
    }

    public void OnPushPullStart()
    {
        if (interactType == InteractableType.Transferable)
        {
            Layers.ChangeLayers(gameObject, Layers.ViewAlways);
            Debug.Log("started pushpull");
            StartCoroutine(CoCheckWorldCollisions());
        }

        StartCoroutine(CoCheckCollisions());
    }

    public void OnPushPullEnd()
    {
        Layers.ChangeLayers(gameObject, originalLayer);
        Debug.Log("ended pushpull");
        StopAllCoroutines();
    }

    #region CoCheckCollisions(): Determine if there is any collisions in front of the box
    IEnumerator CoCheckCollisions()
    {
        float collisionDistance = .05f;
        RaycastHit hit;
        
        //  determine Player is facing direction
        Vector3 direction = (transform.position - playerController.transform.position).normalized;
        if (direction.x > 0)
            direction = Vector3.right;
        else
            direction = Vector3.left;
        
        //  Check for collisions while player is pushin/gpulling
        while (true)
        {
            //  Debug ray                                                                                                        
            if (Application.isEditor) Debug.DrawRay(transform.position, direction * (boxColl.bounds.extents.x + collisionDistance), Color.red, 0.01f);

            //  Test ray
            if (Physics.Raycast(transform.position, direction, out hit, boxColl.bounds.extents.x + collisionDistance))
            {
                //Debug.Log("hit: " + hit.collider.name);
                isColliding = true;
            }
            else
                isColliding = false;

            yield return null;
        }
    }
    #endregion

    #region CoCheckWorldCollisions(): Determine if there is any collisions in other worlds of the box
    IEnumerator CoCheckWorldCollisions()
    {
        //  Check for collisions while player is pushing/pulling
        while (true)
        {
            switch (currentWorldState)
            {
                case WorldChanger.WorldState.Present:
                    if (CheckWorldCollisions(WorldChanger.WorldState.Past))
                        worldChanger.canSwitchPast = false;
                    if (CheckWorldCollisions(WorldChanger.WorldState.Future))
                        worldChanger.canSwitchFuture = false;
                    break;
                case WorldChanger.WorldState.Past:
                    if (CheckWorldCollisions(WorldChanger.WorldState.Present))
                        worldChanger.canSwitchPresent = false;
                    if (CheckWorldCollisions(WorldChanger.WorldState.Future))
                        worldChanger.canSwitchFuture = false;
                    break;
                case WorldChanger.WorldState.Future:
                    if (CheckWorldCollisions(WorldChanger.WorldState.Present))
                        worldChanger.canSwitchPresent = false;
                    if (CheckWorldCollisions(WorldChanger.WorldState.Past))
                        worldChanger.canSwitchPast = false;
                    break;
            }
            //Debug.Log("Present: " + worldChanger.canSwitchPresent + " | Past: " + worldChanger.canSwitchPast + " | Future: " + worldChanger.canSwitchFuture);

            yield return null;
        }
    }
    #endregion

    private void EvaluateTransitionStart(WorldChanger.WorldState worldState)
    {
        //  If the interaction type of this object is non transferable then cancel the pushpull operation of the player
        if (interactType == InteractableType.NonTransferable && playerController.pushpullObject == this)
            playerController.CancelPushingPulling();

        if (interactType == InteractableType.Transferable)
            currentWorldState = worldState;

        //  If the object interation type is Always Transferable, evaluate
        if (interactType == InteractableType.AlwaysTransferable && CheckWorldCollisions(worldState))
        {
            //  if player is currently pushing/pulling this object...
            if (playerController.pushpullObject != this)
            {
                //  Set layer to ViewAlways so it always displays
                Layers.ChangeLayers(gameObject, Layers.ViewAlways);

                //  Determine the new Z position of the object after world change
                switch (worldState)
                {
                    case WorldChanger.WorldState.Present:
                        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                        currentWorldState = WorldChanger.WorldState.Present;
                        break;
                    case WorldChanger.WorldState.Past:
                        transform.position = new Vector3(transform.position.x, transform.position.y, 50);
                        currentWorldState = WorldChanger.WorldState.Past;
                        break;
                    case WorldChanger.WorldState.Future:
                        transform.position = new Vector3(transform.position.x, transform.position.y, 100);
                        currentWorldState = WorldChanger.WorldState.Future;
                        break;
                }
            }
        }   
    }

    private void EvaluateTransitionComplete(WorldChanger.WorldState currentState)
    {
        //  Reset the layer if object is always transferrable type
        if (interactType == InteractableType.AlwaysTransferable)
        {
            //Debug.Log("transition complete");
            Layers.ChangeLayers(gameObject, originalLayer);

            //Debug.Log("Revert layer");
        }
    }

    #region CheckWorldCollisions(): Determine which world object can be teleported too if there is non-collidable space
    private bool CheckWorldCollisions(WorldChanger.WorldState worldState)
    {
        Vector2 colliderExtents = boxColl.size * .39f;
        
        //  Evaluate the world state that player is transferring to.
        switch (worldState)
        {
            //  cast present ray & evaluate
            case WorldChanger.WorldState.Present:
                Vector3 presentPos = new Vector3(transform.position.x, transform.position.y, 0);

                if(Application.isEditor) ExtDebug.DrawBox(presentPos, colliderExtents, transform.rotation, Color.blue); 

                if (currentWorldState != WorldChanger.WorldState.Present && Physics.CheckBox(presentPos, colliderExtents, transform.rotation, Layers.Players))
                {
                    //Debug.Log("Colliding present...");
                    return false;
                }
                break;
            //  cast past ray & evaluate
            case WorldChanger.WorldState.Past:
                Vector3 pastPos = new Vector3(transform.position.x, transform.position.y, 50);

                if (Application.isEditor) ExtDebug.DrawBox(pastPos, colliderExtents, transform.rotation, Color.blue);

                if (currentWorldState != WorldChanger.WorldState.Past && Physics.CheckBox(pastPos, colliderExtents, transform.rotation, Layers.Players))
                {
                    //Debug.Log("Colliding past...");
                    return false;
                }
                break;
            //  cast future ray & evaluate
            case WorldChanger.WorldState.Future:
                Vector3 futurePos = new Vector3(transform.position.x, transform.position.y, 100);

                if (Application.isEditor) ExtDebug.DrawBox(futurePos, colliderExtents, transform.rotation, Color.blue);

                if (currentWorldState != WorldChanger.WorldState.Future && Physics.CheckBox(futurePos, colliderExtents, transform.rotation, Layers.Players))
                {
                    //Debug.Log("Colliding future...");
                    return false;
                }
                break;
        }
        //Debug.Log("No Collide...");
        return true;
    }
    #endregion
}
