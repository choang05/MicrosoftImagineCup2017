﻿using UnityEngine;
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

    //  Animation variables
    private Animator animator;
    int SpeedHash = Animator.StringToHash("Speed");
    int isGroundedHash = Animator.StringToHash("isGrounded");
    int isClimbingHash = Animator.StringToHash("isClimbing");
    int isClimbingUpHash = Animator.StringToHash("isClimbingUp");
    int isPushingHash = Animator.StringToHash("isPushing");
    int isPullingHash = Animator.StringToHash("isPulling");
    int jumpTriggerHash = Animator.StringToHash("jumpTrigger");

    void Awake ()
    {
        //  Find and assign references
        charController = GetComponent<CharacterController> ();
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
	}

    #region Update(): check and evaluate input/states every frame
    void Update ()
    {
        //  Check and update the facing direction of the player
        if (currentState == PlayerState.None)
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
            //  Get input from x axis and add runspeed multiplier
            float xAxis = Input.GetAxis("Horizontal");
            velocity.x = xAxis * runSpeed;

            //  add horizontal jump multiplier
            if (!charController.isGrounded)
                velocity.x *= horizontalJumpForce;

            //  Animation
            animator.SetFloat(SpeedHash, Mathf.Abs(xAxis));
        }

        //  Jumping
        if (Input.GetButtonDown("Jump") && canJump && currentState == PlayerState.None)
            Jump();	

        //  Move
        if (canMove)
            charController.Move(velocity * 10 * Time.deltaTime);

        //  Animation
        animator.SetBool(isGroundedHash, charController.isGrounded);

        /*if ((charController.collisionFlags & CollisionFlags.Above) != 0)
        {
			moveDirection.y = 0;
		} */

        //Debug.Log(currentState);
        //Debug.Log(velocity);
    }
    #endregion

    #region Gravity
    private void ApplyGravity()
    {
        if (!charController.isGrounded)
        {
            //  If the falling velocity has not reached the terminal velocity cap... 
            if (Mathf.Abs(velocity.y) < terminalVelocity)
                velocity += Physics.gravity * gravity * Time.deltaTime;
        }
    }
    #endregion

    #region Direction facing
    private void UpdateFacingDirection()
    {
		Vector3 theScale = transform.localScale;	
        theScale.x *= -1;	
        if (velocity.x > 0)
        {
            facingDirection = FacingDirection.Right;
            transform.rotation = Quaternion.Euler(0, 90, 0);        //  for 3d models
            if (transform.localScale.x < 0)
		        transform.localScale = theScale;	
        }
        else if (velocity.x < 0)
        {
            facingDirection = FacingDirection.Left;
            transform.rotation = Quaternion.Euler(0, -90, 0);       // for 3d models
            if (transform.localScale.x > 0)
		        transform.localScale = theScale;	
        }
    }
    #endregion

    #region Jump
    public void Jump()		
	{
        if (charController.isGrounded)
        {
            //  Set vertical velocity
            velocity.y = verticalJumpForce;
            //  horizontal jump velocity multiplier
            //velocity.x += horizontalJumpForce;
            
            //  Animation
            animator.SetTrigger(jumpTriggerHash);
        }
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

            //  Shoot ray
            RaycastHit hit;
            Physics.Raycast(transform.position + Vector3.up, dir, out hit, interactiveDistance, interactiveLayer);
            //Debug.DrawLine(transform.position + Vector3.up, dir * 1, Color.red, 0.05f);

            //  Evaluate hit
            if (hit.collider)
            {
                //Debug.Log(hit.collider.name);
                currentState = PlayerState.PushingPulling;
                interactingBody = hit.collider.GetComponent<Rigidbody>();
                hit.collider.transform.SetParent(transform);
                interactingBreakDistance = Vector3.Distance(hit.collider.transform.position, transform.position);
                //interactingBody.isKinematic = true;
                //interactingBody.transform.SetParent(transform);
            }
        }
    }
    #endregion

    #region Push/Pull
    void PushPull()
    {
        //  Check if object is within interacting distance
        if (charController.isGrounded && Vector3.Distance(transform.position, interactingBody.transform.position) <= interactingBreakDistance + 0.2f)
        {
            Debug.DrawLine(transform.position + Vector3.up, interactingBody.transform.position, Color.yellow, 0.05f);

            //  Get input axis with smoothing
            float xAxis = Input.GetAxisRaw("Horizontal");
            velocity.x = xAxis * pushPullSpeed;

            animator.speed = 1; //  Remove when idle animation exist

            //  Pushing - RIGHT
            if (velocity.x > 0 && facingDirection == FacingDirection.Right)
            {
                //interactingBody.MovePosition(interactingBody.transform.position + new Vector3(xAxis, 0, 0) * pushPullSpeed * 11.5f * Time.deltaTime);

                //  Animation
                animator.SetBool(isPushingHash, true);
                animator.SetBool(isPullingHash, false);
            }
            //  Pushing - LEFT
            else if (velocity.x < 0 && facingDirection == FacingDirection.Left)
            {
                //interactingBody.MovePosition(interactingBody.transform.position + new Vector3(xAxis, 0, 0) * pushPullSpeed * 11.5f * Time.deltaTime);

                //  Animation
                animator.SetBool(isPushingHash, true);
                animator.SetBool(isPullingHash, false);
            }
            //  Pulling - RIGHT
            else if (velocity.x > 0 && facingDirection == FacingDirection.Left)
            {
                //interactingBody.MovePosition(interactingBody.transform.position + new Vector3(xAxis, 0, 0) * pushPullSpeed * 12 * Time.deltaTime);

                //  Animation
                animator.SetBool(isPushingHash, false);
                animator.SetBool(isPullingHash, true);
            }
            //  Pulling - LEFT
            else if (velocity.x < 0 && facingDirection == FacingDirection.Right)
            {
                //interactingBody.MovePosition(interactingBody.transform.position + new Vector3(xAxis, 0, 0) * pushPullSpeed * 12 * Time.deltaTime);

                //  Animation
                animator.SetBool(isPushingHash, false);
                animator.SetBool(isPullingHash, true);
            }
            else
                animator.speed = 0;     //  Replace later with idle animation
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

        //  Animation
        animator.speed = 1;     //  Remove when idle animation exist
        if (yAxisInput > 0)
            animator.SetBool(isClimbingUpHash, true);
        else if (yAxisInput < 0)
            animator.SetBool(isClimbingUpHash, false);
        else
            animator.speed = 0; //  Replace with idle animation

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
        animator.SetBool(isClimbingHash, false);
        animator.SetBool(isClimbingUpHash, false);
        animator.speed = 1; //  Remove when idle animation exist
        transform.rotation = Quaternion.Euler(0, 90, 0);       // for 3d models
    }
    #endregion

    #region Die
    private void Die()
    {
        //  Respawn player at GameManager's respawn node
        transform.position = gameManager.RespawnNode.position;
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
            float yAxisInput = Input.GetAxis("Vertical");
            if (yAxisInput > 0 || yAxisInput < 0)
            {
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
