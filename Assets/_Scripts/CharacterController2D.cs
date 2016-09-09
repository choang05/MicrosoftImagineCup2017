using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
    PlayerState currentState;
    enum PlayerState { None, Climbing, Jumping, PushingPulling}

    FacingDirection facingDirection;
    enum FacingDirection {  Right, Left    }

    public float runSpeed;
    public float climbSpeed;
    public float pushPullSpeed;
	public float gravity;
    public float terminalVelocity; 	
	public float jumpForce;
    public bool canMove = true;	 
	public bool canJump = true; 	
    public bool canClimb = true;
    public bool canInteract = true;
    public float interactiveDistance;
    public LayerMask interactiveLayer;	

    private Rigidbody interactingBody;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController charController;

    //  Animation
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
        animator = GetComponent<Animator>();
	}

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

        //  Jumping
        if (Input.GetButtonDown("Jump") && canJump && currentState == PlayerState.None)
            Jump();

        //  Climbing
        if (currentState == PlayerState.Climbing)
            Climb();

        //  Apply gravity
        if (currentState == PlayerState.None || currentState == PlayerState.Jumping)
            ApplyGravity();

        //  Moving Horizontally
        if (currentState == PlayerState.None || currentState == PlayerState.Jumping)
        {
            //  Get input from x axis.
            float xAxis = Input.GetAxis("Horizontal");
            moveDirection.x = xAxis * runSpeed;

            //  Animation
            animator.SetFloat(SpeedHash, Mathf.Abs(xAxis));
        }

        //  Move
        if (canMove)
            charController.Move(moveDirection * 10 * Time.deltaTime);

        //  Animation
        //animator.SetBool(isGroundedHash, charController.isGrounded);

        /*if ((charController.collisionFlags & CollisionFlags.Above) != 0)
        {
			moveDirection.y = 0;
		} */

        //Debug.Log(currentState);
    }

    #region Gravity
    private void ApplyGravity()
    {
        if (!charController.isGrounded && Mathf.Abs(moveDirection.y) < terminalVelocity)
        {
            // Y = y + -9.81
            moveDirection += Physics.gravity * gravity * Time.deltaTime;
            animator.SetBool(isGroundedHash, false);

        }
        else
        {
            currentState = PlayerState.None;
            
            // Animation
            animator.SetBool(isGroundedHash, true);
        }

        //Debug.Log(moveDirection.y);
        //Debug.Log(moveDirection);
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
            Vector2 dir;
            if (facingDirection == FacingDirection.Right)
                dir = Vector2.right;
            else
                dir = Vector2.left;

            //  Shoot ray
            RaycastHit hit;
            Physics.Raycast(transform.position, dir, out hit, interactiveDistance, interactiveLayer);
            //Debug.DrawLine(transform.position, dir * interactiveDistance);

            //  Evaluate hit
            if (hit.collider)
            {
                //Debug.Log(hit.collider.name);
                currentState = PlayerState.PushingPulling;
                interactingBody = hit.collider.GetComponent<Rigidbody>();
                hit.collider.transform.SetParent(transform);
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
        if (charController.isGrounded && Vector3.Distance(interactingBody.transform.position, transform.position) < interactiveDistance * 2)
        {
            //  Get input axis
            float xAxis = Input.GetAxisRaw("Horizontal");
            moveDirection.x = xAxis * pushPullSpeed;

            animator.speed = 1; //  Remove when idle animation exist

            //  Pushing - RIGHT
            if (moveDirection.x > 0 && facingDirection == FacingDirection.Right)
            {
                //interactingBody.MovePosition(interactingBody.transform.position + new Vector3(xAxis, 0, 0) * pushPullSpeed * 11.5f * Time.deltaTime);

                //  Animation
                animator.SetBool(isPushingHash, true);
                animator.SetBool(isPullingHash, false);
            }
            //  Pushing - LEFT
            else if (moveDirection.x < 0 && facingDirection == FacingDirection.Left)
            {
                //interactingBody.MovePosition(interactingBody.transform.position + new Vector3(xAxis, 0, 0) * pushPullSpeed * 11.5f * Time.deltaTime);

                //  Animation
                animator.SetBool(isPushingHash, true);
                animator.SetBool(isPullingHash, false);
            }
            //  Pulling - RIGHT
            else if (moveDirection.x > 0 && facingDirection == FacingDirection.Left)
            {
                //interactingBody.MovePosition(interactingBody.transform.position + new Vector3(xAxis, 0, 0) * pushPullSpeed * 12 * Time.deltaTime);

                //  Animation
                animator.SetBool(isPushingHash, false);
                animator.SetBool(isPullingHash, true);
            }
            //  Pulling - LEFT
            else if (moveDirection.x < 0 && facingDirection == FacingDirection.Right)
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

    #region Jump
    public void Jump()		
	{
        if (charController.isGrounded)
        {
            currentState = PlayerState.Jumping;

            moveDirection.y = jumpForce;
            
            //  Animation
            animator.SetTrigger(jumpTriggerHash);
        }
    }
    #endregion

    #region Direction facing
    private void UpdateFacingDirection()
    {
		Vector3 theScale = transform.localScale;	
        theScale.x *= -1;	
        if (moveDirection.x > 0)
        {
            facingDirection = FacingDirection.Right;
            transform.rotation = Quaternion.Euler(0, 90, 0);        //  for 3d models
            if (transform.localScale.x < 0)
		        transform.localScale = theScale;	
        }
        else if (moveDirection.x < 0)
        {
            facingDirection = FacingDirection.Left;
            transform.rotation = Quaternion.Euler(0, -90, 0);       // for 3d models
            if (transform.localScale.x > 0)
		        transform.localScale = theScale;	
        }
    }
    #endregion

    #region Climb
    private void Climb()
    {
        //  Get input from y axis.
        float yAxisInput = Input.GetAxisRaw("Vertical");
        float xAxisInput = Input.GetAxisRaw("Horizontal");

        //  Apply movement vectors
        moveDirection.y = yAxisInput * climbSpeed;
        moveDirection.x = xAxisInput * climbSpeed / 2;

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

    void OnTriggerExit(Collider other)
    {
        if (currentState == PlayerState.Climbing && other.CompareTag(Tags.Ladder))
            CancelClimbing();
    }
}

