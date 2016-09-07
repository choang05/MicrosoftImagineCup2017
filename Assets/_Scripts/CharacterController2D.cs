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
    public float pushSpeed;
    public float pullSpeed;
	public float gravity; 	
	public float jumpForce;	 
	public bool canJump = true; 	
    public bool laddersEnabled = true;
    public float interactiveDistance;
    public LayerMask interactiveLayer;	

    private Rigidbody interactingBody;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController charController;

	void Start ()
    {	
		charController = GetComponent<CharacterController> ();
	}

	void Update ()
    {

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
            moveDirection.y = Input.GetAxisRaw("Vertical");
            moveDirection.y *= climbSpeed;
        }

        //  Apply gravity
        if (currentState != PlayerState.Climbing)
        {
            moveDirection += Physics.gravity * gravity * Time.deltaTime;
        }

        //  Moving Horizontally
        if (currentState == PlayerState.None || currentState == PlayerState.Climbing)
        {
            moveDirection.x = Input.GetAxis("Horizontal");
            moveDirection.x *= runSpeed;
        }

        //  Move
        charController.Move(moveDirection * 10 * Time.deltaTime);

        if (currentState == PlayerState.None)
            UpdateFacingDirection();

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
            moveDirection.x = Input.GetAxis("Horizontal");
            Vector3 bodyMoveDirection;

            //  Pushing - RIGHT
            if (moveDirection.x > 0 && facingDirection == FacingDirection.Right)
            {
                bodyMoveDirection = new Vector3(moveDirection.x, 0, 0) * pushSpeed * 11 * Time.deltaTime;
                moveDirection *= pushSpeed;
                interactingBody.MovePosition(interactingBody.transform.position + bodyMoveDirection);
            }
            //  Pushing - LEFT
            else if (moveDirection.x < 0 && facingDirection == FacingDirection.Left)
            {
                bodyMoveDirection = new Vector3(moveDirection.x, 0, 0) * pushSpeed * 11 * Time.deltaTime;
                moveDirection *= pushSpeed;
                interactingBody.MovePosition(interactingBody.transform.position + bodyMoveDirection);
            }
            //  Pulling - RIGHT
            else if (moveDirection.x > 0 && facingDirection == FacingDirection.Left)
            {
                bodyMoveDirection = new Vector3(moveDirection.x, 0, 0) * pullSpeed * 13 * Time.deltaTime;
                moveDirection *= pullSpeed;
                interactingBody.MovePosition(interactingBody.transform.position + bodyMoveDirection);
            }
            //  Pulling - LEFT
            else if (moveDirection.x < 0 && facingDirection == FacingDirection.Right)
            {
                bodyMoveDirection = new Vector3(moveDirection.x, 0, 0) * pullSpeed * 13 * Time.deltaTime;
                moveDirection *= pullSpeed;
                interactingBody.MovePosition(interactingBody.transform.position + bodyMoveDirection);
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
        interactingBody = null;
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
            if (transform.localScale.x < 0)
		        transform.localScale = theScale;	
        }
        else if (moveDirection.x < 0)
        {
            facingDirection = FacingDirection.Left;
            if (transform.localScale.x > 0)
		        transform.localScale = theScale;	
        }
    }
    #endregion

    //Moving Platform handling
    /*void OnControllerColliderHit(ControllerColliderHit other)
    {
        if (other.gameObject.GetComponent<MovingPlatform>() != null && (playerController.collisionFlags & CollisionFlags.Below) != 0)
        {
            gameObject.transform.SetParent(other.transform);
        }
        else
        {
            gameObject.transform.SetParent(null);
        }
    }*/

    void OnTriggerStay(Collider other)
    {
        #region Ladders
        if (currentState == PlayerState.None && other.CompareTag(Tags.Ladder))
        {
            float moveDirInput = Input.GetAxis("Vertical");
            if (moveDirInput > 0 || moveDirInput < 0)
                currentState = PlayerState.Climbing;
        }
        #endregion
    }

    void OnTriggerExit(Collider other)
    {
        if (currentState == PlayerState.Climbing && other.CompareTag(Tags.Ladder))
            currentState = PlayerState.None;
    }
}

