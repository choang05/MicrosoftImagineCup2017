using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
    PlayerState currentState;
    enum PlayerState { None, Climbing, PushingPulling}

    FacingDirection facingDirection;
    enum FacingDirection {  Right, Left    }

    public float runSpeed;
    public float climbSpeed;
    public float pushPullSpeed;
	public float gravity; 	
	public float jumpForce;	 
	public bool canJump = true; 	
    public bool laddersEnabled = true;
    public float interactiveDistance;
    public LayerMask interactiveLayer;	

    private Rigidbody interactingBody;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController charController;

    //  Animation
    private Animator animator;
    static int SpeedHash = Animator.StringToHash("Speed");
    static int isClimbingHash = Animator.StringToHash("isClimbing");
    static int isClimbingUpHash = Animator.StringToHash("isClimbingUp");
    static int isPushingHash = Animator.StringToHash("isPushing");
    static int isPullingHash = Animator.StringToHash("isPulling");

    void Start ()
    {	
		charController = GetComponent<CharacterController> ();
        animator = GetComponent<Animator>();
	}

	void Update ()
    {
        if (currentState == PlayerState.None)
            UpdateFacingDirection();

        //  Check Push/Pull, else perform push/pull
        if (Input.GetKeyDown(KeyCode.E) && charController.isGrounded)
            CheckPushPull();
        else if (currentState == PlayerState.PushingPulling)
            TestPushPull();

        //  Jumping
        if (Input.GetButtonDown("Jump") && currentState == PlayerState.None)
            Jump ();	

        //  Climbing
        if (currentState == PlayerState.Climbing)
        {
            //  Get input from y axis.
            float yAxis = Input.GetAxisRaw("Vertical");
            moveDirection.y = yAxis * climbSpeed;

            //  Animation
            animator.speed = 1;     //  Remove when idle animation exist
            if (yAxis > 0)
                animator.SetBool(isClimbingUpHash, true);
            else if (yAxis < 0)
                animator.SetBool(isClimbingUpHash, false);
            else
                animator.speed = 0; //  Replace with idle animation
        }

        //  Apply gravity
        if (currentState != PlayerState.Climbing)
        {
            moveDirection += Physics.gravity * gravity * Time.deltaTime;
        }

        //  Moving Horizontally
        if (currentState == PlayerState.None || currentState == PlayerState.Climbing)
        {
            //  Get input from x axis.
            float xAxis = Input.GetAxisRaw("Horizontal");
            moveDirection.x = xAxis * runSpeed;

            //  Animation
            animator.SetFloat(SpeedHash, Mathf.Abs(xAxis));
        }

        //  Move
        charController.Move(moveDirection * 10 * Time.deltaTime);



        /*if ((charController.collisionFlags & CollisionFlags.Above) != 0)
        {
			moveDirection.y = 0;
		} */

        //Debug.Log(currentState);
    }

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
                //interactingBody.isKinematic = true;
                //interactingBody.transform.SetParent(transform);
            }
        }
    }
    #endregion

    #region Test Pushing/Pulling
    void TestPushPull()
    {
        //  Check if object is within interacting distance
        if (Vector3.Distance(interactingBody.transform.position, transform.position) < interactiveDistance * 2)
        {
            //  Get input axis
            float xAxis = Input.GetAxisRaw("Horizontal");
            moveDirection.x = xAxis * pushPullSpeed;
            Vector3 bodyMoveDirection;

            animator.speed = 1; //  Remove when idle animation exist

            //  Pushing - RIGHT
            if (moveDirection.x > 0 && facingDirection == FacingDirection.Right)
            {
                bodyMoveDirection = new Vector3(xAxis, 0, 0) * pushPullSpeed * 11 * Time.deltaTime;
                interactingBody.MovePosition(interactingBody.transform.position + bodyMoveDirection);

                //  Animation
                animator.SetBool(isPushingHash, true);
                animator.SetBool(isPullingHash, false);
            }
            //  Pushing - LEFT
            else if (moveDirection.x < 0 && facingDirection == FacingDirection.Left)
            {
                bodyMoveDirection = new Vector3(xAxis, 0, 0) * pushPullSpeed * 11 * Time.deltaTime;
                interactingBody.MovePosition(interactingBody.transform.position + bodyMoveDirection);

                //  Animation
                animator.SetBool(isPushingHash, true);
                animator.SetBool(isPullingHash, false);
            }
            //  Pulling - RIGHT
            else if (moveDirection.x > 0 && facingDirection == FacingDirection.Left)
            {
                bodyMoveDirection = new Vector3(xAxis, 0, 0) * pushPullSpeed * 13 * Time.deltaTime;
                interactingBody.MovePosition(interactingBody.transform.position + bodyMoveDirection);

                //  Animation
                animator.SetBool(isPushingHash, false);
                animator.SetBool(isPullingHash, true);
            }
            //  Pulling - LEFT
            else if (moveDirection.x < 0 && facingDirection == FacingDirection.Right)
            {
                bodyMoveDirection = new Vector3(xAxis, 0, 0) * pushPullSpeed * 13 * Time.deltaTime;
                interactingBody.MovePosition(interactingBody.transform.position + bodyMoveDirection);

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
        interactingBody = null;

        //  Animation
        animator.SetBool(isPushingHash, false);
        animator.SetBool(isPullingHash, false);
        animator.speed = 1;     //  Remove when idle animation exist
    }
    #endregion

    #region Test Jump
    public void Jump()		
	{
        if (charController.isGrounded && canJump)
            moveDirection.y = jumpForce;	 	
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

    void OnTriggerStay(Collider other)
    {
        #region Ladders
        if (currentState == PlayerState.None && other.CompareTag(Tags.Ladder))
        {
            float moveDirInput = Input.GetAxis("Vertical");
            if (moveDirInput > 0 || moveDirInput < 0)
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
        {
            currentState = PlayerState.None;
            
            // Animation
            animator.SetBool(isClimbingHash, false);
            animator.speed = 1; //  Remove when idle animation exist
        }
    }
}

