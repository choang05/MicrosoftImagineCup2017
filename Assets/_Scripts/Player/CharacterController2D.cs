using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
    //  User Parameters variables
    public float runSpeed;                                          //  The speed at which the player's runs horizontally
    public float climbSpeed;                                        //  The speed at which the player's climbs vertically
    public float pushPullSpeed;                                     //  The speed at which the player pushes/pulls an object
    public float pushpullDistance;                                  //  The farthest distance at which the player can push/pull objects
    public LayerMask pushpullLayer;	                                //  The layer assigned to push/pull objects
    public float gravity;                                           //  The incremental speed that is added to the player's y velocity
    public float terminalVelocity;                                  //  The max speed that is added to the player's y velocity 
    public float verticalJumpForce;                                 //  The amount of vertical force applied to jumps
    public float horizontalJumpForce;                               //  The amount of horizontal force applied to jumps
    public int impactForceThreshold;                                //  The threshold reached to to kill player caused by colliding object's collision.impulse magnitude          	
    public bool canMove = true;	                                    //  is the player allowed to move?
	public bool canJump = true; 	                                //  is the player allowed to jump?
    public bool canClimb = true;                                    //  is the player allowed to climb?
    public bool canPushPull = true;                                 //  is the player allowed to push/pull                            

    //  Private variables
    [HideInInspector] public PlayerState currentState;              //  The current state of the player
    public enum PlayerState { None, Climbing, PushingPulling }      //  The state the player can have
    [HideInInspector] public Vector3 velocity;                      //  The velocity of x and y of the player
    [HideInInspector] public PushPullObject pushpullObject;         //  The transform of the pushing/pulling object

    //  Private variables
    private FacingDirection facingDirection;                        //  The direction the player is facing
    private enum FacingDirection { Right, Left }                    //  The directions the player can have
    private float pushpullBreakDistance;                            //  The max distance between the player and the pushing/pulling object before it cancels the interaction
    private AudioSource grassStepSource;                            // The audio source for footsteps
    private AudioSource playerGroundImpactSource;                   // audio source for impact with ground for player

    //  References variables
    private CharacterController charController;
    private GameManager gameManager;
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
        animator = GetComponent<Animator>();
        puppet2DGlobalControl = GetComponentInChildren<Puppet2D_GlobalControl>();
        grassStepSource = GetComponentInChildren<AudioSource>();
	}

    #region Update(): check and evaluate input and states every frame
    void Update ()
    {

       

        //  Check and update the facing direction of the player
        if (currentState == PlayerState.None)
            UpdateFacingDirection();

        //  Check Push/Pull, else perform push/pull
        if (Input.GetKeyDown(KeyCode.E) && charController.isGrounded)
            CheckPushPull();
        else if (currentState == PlayerState.PushingPulling)
            PushingPulling();

        //  Climbing
        if (currentState == PlayerState.Climbing)
            Climb();

        //  Apply gravity
        if (currentState == PlayerState.None)
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
        if (canMove)
            charController.Move(velocity * Time.deltaTime);

        //  Animation
        animator.SetBool(isGroundedHash, charController.isGrounded);


        //Debug.Log(currentState);
        //Debug.Log(charController.isGrounded);
        //Debug.Log(velocity);
    }
    #endregion

    #region ApplyGravity()
    private void ApplyGravity()
    {
        //  If the character is not grounded...
        if (!charController.isGrounded)
        {
            //  If the falling velocity has not reached the terminal velocity cap... 
            if (velocity.y >= terminalVelocity)
                velocity += Physics.gravity * gravity * Time.deltaTime;
        }

        //  Animation
        animator.SetFloat(yVelocityHash, velocity.y);

    }
    #endregion

    #region UpdateFacingDirection()
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

    #region Jump(): called from jump animation event
    public void Jump()		
	{
        //  Set vertical velocity
        velocity.y = verticalJumpForce;
            
        //  Animation
        //animator.SetTrigger(jumpTriggerHash);
    }
    #endregion

    #region CheckPushPull(): Checks for push/pull Object
    void CheckPushPull()
    {
        //  if player currently already pushing/pull an object then cancel the push/pull interaction
        if (currentState == PlayerState.PushingPulling)
            CancelPushingPulling();
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
            Physics.Raycast(transform.position, dir, out hit, pushpullDistance, pushpullLayer);
            if (Application.isEditor) Debug.DrawRay(transform.position, dir * pushpullDistance, Color.red, 5f);

            //  Evaluate hit
            if (hit.collider)
            {
                //Debug.Log("Holding objecT: " + hit.collider.name);
                //  Update player state
                currentState = PlayerState.PushingPulling;

                //  Cache pushing/pulling body
                pushpullObject = hit.transform.GetComponent<PushPullObject>();
                pushpullObject.transform.SetParent(transform);

                //  Set the pushing/pulling break distance
                pushpullBreakDistance = Vector3.Distance(pushpullObject.transform.position, transform.position);

                //  Process interaction event to the push/pull object
                pushpullObject.OnPushPullStart();

                //  Animation
                animator.SetBool(isPushPullingHash, true);
            }
        }
    }
    #endregion

    #region PushingPulling()
    void PushingPulling()
    {
        //  Check if object is within the PushPull break distance... if not, cancel the push/pull interaction
        if (charController.isGrounded && Vector3.Distance(transform.position, pushpullObject.transform.position) <= pushpullBreakDistance + 0.15f)
        {
            if (Application.isEditor) Debug.DrawLine(transform.position, pushpullObject.transform.position, Color.yellow, 0.05f);

            //  Get input axis with smoothing
            float xAxis = Input.GetAxisRaw("Horizontal");
            velocity.x = xAxis * pushPullSpeed;

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
            CancelPushingPulling();

    }
    #endregion

    #region CancelPushingPulling(): Cancels the push/pull interaction
    public void CancelPushingPulling()
    {
        //  Update state
        currentState = PlayerState.None;

        //  Process push/pull end event
        pushpullObject.GetComponent<PushPullObject>().OnPushPullEnd();

        //  Return parent of pushing/pulling body
        pushpullObject.transform.SetParent(null);
        pushpullObject = null;

        //  Animation
        animator.SetBool(isPushingHash, false);
        animator.SetBool(isPullingHash, false);
        animator.SetBool(isPushPullingHash, false);
    }
    #endregion

    #region Climb()
    private void Climb()
    {
        //  Get input from y axis.
        float yAxisInput = Input.GetAxisRaw("Vertical");
        //float xAxisInput = Input.GetAxisRaw("Horizontal");

        //  Apply movement vectors
        velocity.y = yAxisInput * climbSpeed;
        //velocity.x = xAxisInput * climbSpeed / 2;

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
        if (charController.isGrounded)
            CancelClimbing();
    }
    #endregion

    #region CancelClimbing()
    private void CancelClimbing()
    {
        //  Set player state
        currentState = PlayerState.None;


        // Animation
        animator.SetBool(isClimbingUpHash, false);
        animator.SetBool(isClimbingDownHash, false);
        animator.SetBool(isClimbingHash, false);
    }
    #endregion

    #region Die()
    public void Die()
    {
        //  Respawn player at GameManager's respawn node
        transform.position = gameManager.RespawnNode.position;

        Debug.Log("Player died!");
    }
    #endregion

    #region ProcessImpact(): Evaluate collision impacts
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

        //  Perform Ledge climbs if within ledge colliders
        if (other.CompareTag(Tags.Ledge))
        {
            if (true)
            {
                //  If the player inputs up and forward... evaluate
                float yAxisInput = Input.GetAxisRaw("Vertical");
                float xAxisInput = Input.GetAxisRaw("Horizontal");

                if (true)
                {

                }
            }
        }
    }

    //  Called when a collider stay within another collider with isTrigger enabled
    void OnTriggerStay(Collider other)
    {
        #region Check Climb
        if (canClimb && currentState == PlayerState.None && other.CompareTag(Tags.Ladder))
        {
            //  If the player inputs up or down... evaluate
            float yAxisInput = Input.GetAxisRaw("Vertical");
            if (yAxisInput > 0 || (yAxisInput < 0 && !charController.isGrounded))
            {
                //  Set state
                currentState = PlayerState.Climbing;

                //  Set position to match ladder
                transform.position = new Vector3(other.transform.position.x, transform.position.y, transform.position.z);

                //  Reset horizontal speed so player does not slide horizontally during ladder use
                velocity.x = 0;
                
                // Animation
                animator.SetBool(isClimbingHash, true);
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

    //play audio source for footsteps when player is walking
    void grassFootstepAudio()
    { 
       randomizePitch(grassStepSource);
       grassStepSource.Play();
            
    }

    // Called to randomize the pitch of certain audio sources so they don't get dull to hear
    void randomizePitch(AudioSource audio)
    {
        audio.pitch = Random.Range(0.95f, 1.05f);
    }
}

