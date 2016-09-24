using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
    //  User Parameters variables
    public float runSpeed;
    public float climbSpeed;
    public float pushPullSpeed;
	public float gravity;
    public float terminalVelocity;
	public float verticalJumpForce;
    public float horizontalJumpForce;
    public int impactForceThreshold;                //  The threshold reached to to kill player caused by colliding object's collision.impulse magnitude          	
    public bool canMove = true;	 
	public bool canJump = true; 	
    public bool canClimb = true;
    public bool canInteract = true;
    public float interactiveDistance;
    public LayerMask interactiveLayer;	

    //  Private variables
    PlayerState currentState;
    enum PlayerState { None, Climbing, PushingPulling }
    FacingDirection facingDirection;
    enum FacingDirection { Right, Left }
    private Vector3 velocity = Vector3.zero;
    private Rigidbody interactingBody;
    private float interactingBreakDistance;

    private CharacterController charController;
    private GameManager gameManager;
    private WorldChanger worldChanger;
    private Puppet2D_GlobalControl puppet2DGlobalControl;

    //  Animation variables
    private Animator animator;
    int xVelocityHash = Animator.StringToHash("xVelocity");
    int yVelocityHash = Animator.StringToHash("yVelocity");
    int isGroundedHash = Animator.StringToHash("isGrounded");
    int isClimbingHash = Animator.StringToHash("isClimbing");
    int isClimbingUpHash = Animator.StringToHash("isClimbingUp");
    int isClimbingDownHash = Animator.StringToHash("isClimbingDown");
    int isPushPullingHash = Animator.StringToHash("isPushingPulling");
    int isPushingHash = Animator.StringToHash("isPushing");
    int isPullingHash = Animator.StringToHash("isPulling");
    int jumpTriggerHash = Animator.StringToHash("jumpTrigger");

    void Awake ()
    {
        //  Find and assign references
        charController = GetComponent<CharacterController> ();
        gameManager = FindObjectOfType<GameManager>();
        worldChanger = GetComponent<WorldChanger>();
        animator = GetComponent<Animator>();
        puppet2DGlobalControl = GetComponentInChildren<Puppet2D_GlobalControl>();
	}

    #region Update(): check and evaluate input/states every frame
    void Update ()
    {
        //  Check and update the facing direction of the player
        if (currentState == PlayerState.None || (worldChanger && !worldChanger.cameraTransition.IsRunning))
            UpdateFacingDirection();

        //  Check Push/Pull, else perform push/pull
        if (Input.GetKeyDown(KeyCode.E) && charController.isGrounded)
            CheckPushPull();
        else if (currentState == PlayerState.PushingPulling)
            PushPull();


        //  Climbing
        if (currentState == PlayerState.Climbing)
            Climb();

        //  Apply gravity
        if (currentState != PlayerState.Climbing && currentState != PlayerState.PushingPulling)
            ApplyGravity();

        //  Moving Horizontally
        if (currentState == PlayerState.None)
        {
            //  Get input from x axis
            float xAxis = Input.GetAxis("Horizontal");

            //  if player is on the ground... add run speed multiplier
            if (charController.isGrounded)
                velocity.x = xAxis * runSpeed;
            //  else... add horizontal jump multiplier when player is in air
            else
                velocity.x = xAxis * horizontalJumpForce;

            //  Animation
            animator.SetFloat(xVelocityHash, Mathf.Abs(xAxis));
        }

        //  Jumping
        if (Input.GetButtonDown("Jump") && canJump && ((charController.isGrounded && currentState == PlayerState.None) || currentState == PlayerState.Climbing))
        {
            if (currentState == PlayerState.Climbing)
                CancelClimbing();

            //  Animation
            animator.SetTrigger(jumpTriggerHash);
            //Jump();	
        }

        //  Move
        if (canMove || (worldChanger && !worldChanger.cameraTransition.IsRunning))
            charController.Move(velocity * Time.deltaTime);

        //  Animation
        animator.SetBool(isGroundedHash, charController.isGrounded);

        //Debug.Log(currentState);
        //Debug.Log(charController.isGrounded);
        //Debug.Log(velocity);
    }
    #endregion

    #region Gravity
    private void ApplyGravity()
    {
        if (!charController.isGrounded)
        {
            //  If the falling velocity has not reached the terminal velocity cap... 
            if (velocity.y >= terminalVelocity)
                velocity += Physics.gravity * gravity * Time.deltaTime;

            //  Animation
            animator.SetFloat(yVelocityHash, velocity.y);
        }
    }
    #endregion

    #region Direction facing
    private void UpdateFacingDirection()
    {
		//Vector3 flippedScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        	
        //  if player's velocity is positive... flip character scale to positive
        if (velocity.x > 0)
        {
            facingDirection = FacingDirection.Right;
            //  Flip the global control rig
            puppet2DGlobalControl.flip = false;
        }
        //  if player's velocity is negative... flip character scale to negative
        else if (velocity.x < 0)
        {
            facingDirection = FacingDirection.Left;
            //  Flip the global control rig
            puppet2DGlobalControl.flip = true;
        }
    }
    #endregion

    #region Jump
    public void Jump()		
	{
        //  Set vertical velocity
        velocity.y = verticalJumpForce;
            
        //  Animation
        //animator.SetTrigger(jumpTriggerHash);
    }
    #endregion

    #region Check Push/Pull Object
    void CheckPushPull()
    {
        //  if player currently already pushing/pull an object then cancel the push/pull interaction
        if (currentState == PlayerState.PushingPulling)
            CancelPushPullInteraction();
        //  else check for any objects within distance to push/pull
        else
        {
            //  Determine direction to cast ray
            Vector3 dir;
            if (facingDirection == FacingDirection.Right)
                dir = Vector3.right;
            else
                dir = Vector3.left;

            //  cast ray
            RaycastHit hit;
            Physics.Raycast(transform.position, dir, out hit, interactiveDistance, interactiveLayer);
            if (Application.isEditor) Debug.DrawRay(transform.position, dir * interactiveDistance, Color.red, 5f);

            //  Evaluate hit
            if (hit.collider)
            {
                //Debug.Log("Holding objecT: " + hit.collider.name);
                //  Update player state
                currentState = PlayerState.PushingPulling;

                //  Cache interacting body
                interactingBody = hit.collider.GetComponent<Rigidbody>();
                hit.collider.transform.SetParent(transform);

                //  Set the interaction break distance
                interactingBreakDistance = Vector3.Distance(hit.collider.transform.position, transform.position);

                //  Animation
                animator.SetBool(isPushPullingHash, true);
            }
        }
    }
    #endregion

    #region Push/Pull
    void PushPull()
    {
        //  Check if object is within the interaction break distance
        if (charController.isGrounded && Vector3.Distance(transform.position, interactingBody.transform.position) <= interactingBreakDistance + 0.15f)
        {
            if (Application.isEditor) Debug.DrawLine(transform.position, interactingBody.transform.position, Color.yellow, 0.05f);

            //  Get input axis with smoothing
            float xAxis = Input.GetAxisRaw("Horizontal");
            velocity.x = xAxis * pushPullSpeed;

            animator.speed = 1; //  Remove when idle animation exist

            //  Pushing - RIGHT
            if (velocity.x > 0 && facingDirection == FacingDirection.Right)
            {
                //  Animation - Pushing
                animator.SetBool(isPushingHash, true);
                animator.SetBool(isPullingHash, false);
            }
            //  Pushing - LEFT
            else if (velocity.x < 0 && facingDirection == FacingDirection.Left)
            {
                //  Animation - Pushing
                animator.SetBool(isPushingHash, true);
                animator.SetBool(isPullingHash, false);
            }
            //  Pulling - RIGHT
            else if (velocity.x > 0 && facingDirection == FacingDirection.Left)
            {
                //  Animation - pulling
                animator.SetBool(isPushingHash, false);
                animator.SetBool(isPullingHash, true);
            }
            //  Pulling - LEFT
            else if (velocity.x < 0 && facingDirection == FacingDirection.Right)
            {
                //  Animation - pulling
                animator.SetBool(isPushingHash, false);
                animator.SetBool(isPullingHash, true);
            }
            else
            {
                //  Animation - Idling
                animator.SetBool(isPushingHash, false);
                animator.SetBool(isPullingHash, false);
            }
        }
        else
            CancelPushPullInteraction();

    }
    #endregion

    #region Cancels the push/pull interaction
    private void CancelPushPullInteraction()
    {
        currentState = PlayerState.None;
        //interactingBody.isKinematic = false;
        //interactingBody.transform.SetParent(null);
        interactingBody.transform.SetParent(null);
        interactingBody = null;

        //  Animation
        animator.SetBool(isPushingHash, false);
        animator.SetBool(isPullingHash, false);
        animator.SetBool(isPushPullingHash, false);
        animator.speed = 1;     //  Remove when idle animation exist
    }
    #endregion

    #region Climb
    private void Climb()
    {
        //  Get input from y axis.
        float yAxisInput = Input.GetAxisRaw("Vertical");
        float xAxisInput = Input.GetAxisRaw("Horizontal");

        //  Apply movement vectors
        velocity.y = yAxisInput * climbSpeed;
        velocity.x = xAxisInput * climbSpeed / 2;

        //  if player inputs up or down...
        if (yAxisInput > 0)
        {
            //  Animation - Climb up
            animator.SetBool(isClimbingUpHash, true);
            animator.SetBool(isClimbingDownHash, false);
        }
        else if (yAxisInput < 0)
        {
            //  Animation - Climb down
            animator.SetBool(isClimbingUpHash, false);
            animator.SetBool(isClimbingDownHash, true);
        }
        else
        {
            //  Animation - Climb Idle
            animator.SetBool(isClimbingUpHash, false);
            animator.SetBool(isClimbingDownHash, false);
        }

        //  Cancels climbing when touching the ground at the bottom of ladder
        if (yAxisInput < 0 && charController.isGrounded)
            CancelClimbing();
    }
    #endregion

    #region Cancel climbing
    private void CancelClimbing()
    {
        currentState = PlayerState.None;

        // Animation
        animator.SetBool(isClimbingUpHash, false);
        animator.SetBool(isClimbingDownHash, false);
        animator.SetBool(isClimbingHash, false);
        //animator.speed = 1; //  Remove when idle animation exist
    }
    #endregion

    #region Die
    public void Die()
    {
        //  Respawn player at GameManager's respawn node
        transform.position = gameManager.RespawnNode.position;

        Debug.Log("Player died!");
    }
    #endregion

    #region Collision impacts
    //  called when player impacted by colliding object
    public void ProcessImpact(Vector3 collisionForce)
    {
        //Debug.Log(collisionForce.magnitude);

        //  Evaluate force and see if its enough to kill the player
        if (collisionForce.magnitude >= impactForceThreshold)
        {
            Die();
        }
    }
    #endregion

    //  Called when a collider enters another collider with isTrigger enabled
    void OnTriggerEnter(Collider other)
    {
        //  If player collides with a trap, perform death function
        if (other.CompareTag(Tags.Trap))
        {
            Die();   
        }
    }

    //  Called when a collider stay within another collider with isTrigger enabled
    void OnTriggerStay(Collider other)
    {
        #region Check Climb
        if (canClimb && currentState == PlayerState.None && other.CompareTag(Tags.Ladder))
        {
            //  If the player inputs up or down... evaluate
            float yAxisInput = Input.GetAxis("Vertical");
            if (yAxisInput > 0 || yAxisInput < 0)
            {
                //  Set state
                currentState = PlayerState.Climbing;
                
                // Animation
                animator.SetBool(isClimbingHash, true);
                transform.rotation = Quaternion.Euler(0, 0, 0);       // for 3d models
            }
        }
        #endregion
    }

    //  Called when a collider exits another collider with isTrigger enabled
    void OnTriggerExit(Collider other)
    {
        if (currentState == PlayerState.Climbing && other.CompareTag(Tags.Ladder))
            CancelClimbing();
    }
}

