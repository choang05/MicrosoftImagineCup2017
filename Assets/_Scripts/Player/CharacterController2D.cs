﻿using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
    //  User Parameters variables
    public float runSpeed;                                          //  The speed at which the player's runs horizontally
    public float ladderClimbSpeed;                                  //  The speed at which the player's climbs vertically on ladders
    public float ropeClimbSpeed;                                    //  The speed at which the player's climbs vertically on ropes
    public float swingForce;
    public float pushPullSpeed;                                     //  The speed at which the player pushes/pulls an object
    public float pushpullDistance;                                  //  The farthest distance at which the player can push/pull objects
    public float gravity;                                           //  The incremental speed that is added to the player's y velocity
    public float terminalVelocity;                                  //  The max speed that is added to the player's y velocity 
    public float verticalJumpForce;                                 //  The amount of vertical force applied to jumps
    public float horizontalJumpForce;                               //  The amount of horizontal force applied to jumps
    public int impactForceThreshold;                                //  The threshold reached to to kill player caused by colliding object's collision.impulse magnitude          	
    public bool canMove = true;	                                    //  is the player allowed to move?
	public bool canJump = true; 	                                //  is the player allowed to jump?
    public bool canClimb = true;                                    //  is the player allowed to climb?
    public bool canPushPull = true;                                 //  is the player allowed to push/pull 
    public PlayerAudio pa;
    

    //  Private variables
    [HideInInspector] public PlayerState currentState;              //  The current state of the player
    public enum PlayerState                                         //  The states the player can have
    {
        None,
        ClimbingRope,
        ClimbingLadder,
        ClimbingLedge,
        PushingPulling
    }      
    [HideInInspector] public Vector3 velocity;                      //  The velocity of x and y of the player
    [HideInInspector] public PushPullObject pushpullObject;         //  The transform of the pushing/pulling object

    //  Private variables
    private float velToVol = 0.2f;                                  //  velocity to volume, for calculation collision volume
    private FacingDirection facingDirection;                        //  The direction the player is facing
    private enum FacingDirection { Right, Left }                    //  The directions the player can have
    private float pushpullBreakDistance;                            //  The max distance between the player and the pushing/pulling object before it cancels the interaction

    public AudioClip boxSlide;
    public AudioClip playerImpactGrass;
    public AudioClip playerImpactWood;
    private AudioSource charSound;

    private bool isTouchingGround;                                  //  True if the player is on the ground(not platform)
    private BoxCollider currentLadderBoxCollider;                   //  The BoxCollider of the currently using ladder
    private Rigidbody currentRopeRigidBody;
    private bool canSwingRight;
    private bool canSwingLeft;


    //  References variables
    private CharacterController charController;
    private GameManager gameManager;
    private Puppet2D_GlobalControl puppet2DGlobalControl;

    //  Animation variables
    private Animator animator;
    int xVelocityHash = Animator.StringToHash("xVelocity");
    int yVelocityHash = Animator.StringToHash("yVelocity");
    int isGroundedHash = Animator.StringToHash("isGrounded");
    int isClimbingLadderHash = Animator.StringToHash("isClimbingLadder");
    int isClimbingLadderUpHash = Animator.StringToHash("isClimbingLadderUp");
    int isClimbingLadderDownHash = Animator.StringToHash("isClimbingLadderDown");
    int isClimbingRopeHash = Animator.StringToHash("isClimbingRope");
    int isClimbingRopeUpHash = Animator.StringToHash("isClimbingRopeUp");
    int isClimbingRopeDownHash = Animator.StringToHash("isClimbingRopeDown");
    int isSwingingForwardHash = Animator.StringToHash("isSwingingForward");
    int isSwingingBackwardHash = Animator.StringToHash("isSwingingBackward");
    int ledgeClimbUpRightTriggerHash = Animator.StringToHash("ledgeClimbUpRightTrigger");
    int ledgeClimbUpLeftTriggerHash = Animator.StringToHash("ledgeClimbUpLeftTrigger");
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
        charSound = GetComponent<AudioSource>();
	}

    #region Update(): check and evaluate input and states every frame
    void Update ()
    {

       

        //  Check and update the facing direction of the player
        if (currentState == PlayerState.None)
            UpdateFacingDirection();
        
        //  Apply gravity
        if (currentState == PlayerState.None)
            ApplyGravity();

        //  Check Push/Pull, else perform push/pull
        if (Input.GetKeyDown(KeyCode.E) && charController.isGrounded)
            CheckPushPull();
        else if (currentState == PlayerState.PushingPulling)
            PushingPulling();

        //  Climbing Ladders
        if (currentState == PlayerState.ClimbingLadder)
            ClimbLadder();

        //  Climbing Ropes
        if (currentState == PlayerState.ClimbingRope)
            ClimbRope();

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
        if (Input.GetButtonDown("Jump") && canJump && ((charController.isGrounded && currentState == PlayerState.None) 
            || currentState == PlayerState.ClimbingLadder 
            || currentState == PlayerState.ClimbingRope))
        {
            if (currentState == PlayerState.ClimbingLadder || currentState == PlayerState.ClimbingRope)
                CancelClimbing();

            //  Animation
            animator.SetTrigger(jumpTriggerHash);
            //Jump();	
        }

        //  Move
        if (canMove && currentState != PlayerState.ClimbingRope)
            charController.Move(velocity * Time.deltaTime);

        //  Animation
        animator.SetBool(isGroundedHash, charController.isGrounded);

        //Debug.Log(currentState);
        //Debug.Log(charController.isGrounded);
        //Debug.Log(isTouchingGround);
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
            Physics.Raycast(transform.position, dir, out hit, pushpullDistance, Layers.PushPullable);
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
                if (!charSound.isPlaying)
                {
                    pa.randomizePitch(charSound);
                    charSound.loop = true;
                    charSound.PlayOneShot(boxSlide, pa.randomVolume());
                }
            }
            //  Pushing - LEFT
            else if (velocity.x < 0 && facingDirection == FacingDirection.Left)
            {
                //  Animation - Pushing
                animator.SetBool(isPushingHash, true);
                animator.SetBool(isPullingHash, false);
                if (!charSound.isPlaying)
                {
                    pa.randomizePitch(charSound);
                    charSound.loop = true;
                    charSound.PlayOneShot(boxSlide, pa.randomVolume());
                }
            }
            //  Pulling - RIGHT
            else if (velocity.x > 0 && facingDirection == FacingDirection.Left)
            {
                //  Animation - pulling
                animator.SetBool(isPushingHash, false);
                animator.SetBool(isPullingHash, true);
                if (!charSound.isPlaying)
                {
                    pa.randomizePitch(charSound);
                    charSound.loop = true;
                    charSound.PlayOneShot(boxSlide, pa.randomVolume());
                }
            }
            //  Pulling - LEFT
            else if (velocity.x < 0 && facingDirection == FacingDirection.Right)
            {
                //  Animation - pulling
                animator.SetBool(isPushingHash, false);
                animator.SetBool(isPullingHash, true);
                if (!charSound.isPlaying)
                {
                    pa.randomizePitch(charSound);
                    charSound.loop = true;
                    charSound.PlayOneShot(boxSlide, pa.randomVolume());
                }
            }
            else
            {
                //  Animation - Idling
                animator.SetBool(isPushingHash, false);
                animator.SetBool(isPullingHash, false);
                if (charSound.isPlaying)
                    charSound.loop = false;
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

        // Stop Audio Playback
        if (charSound.isPlaying)
            charSound.loop = false;

    }
    #endregion

    #region ClimbLadder()
    private void ClimbLadder()
    {
        //  Get input from y axis.
        float yAxisInput = Input.GetAxisRaw("Vertical");
        //float xAxisInput = Input.GetAxisRaw("Horizontal");

        //  Apply movement vectors
        velocity.y = yAxisInput * ladderClimbSpeed;
        //velocity.x = xAxisInput * climbSpeed / 2;

        //  if player inputs up or down...
        if (yAxisInput > 0)
        {
            //  Animation - ClimbLadder up
            animator.SetBool(isClimbingLadderUpHash, true);
            animator.SetBool(isClimbingLadderDownHash, false);
        }
        else if (yAxisInput < 0)
        {
            //  Animation - ClimbLadder down
            animator.SetBool(isClimbingLadderUpHash, false);
            animator.SetBool(isClimbingLadderDownHash, true);
        }
        else
        {
            //  Animation - ClimbLadder Idle
            animator.SetBool(isClimbingLadderUpHash, false);
            animator.SetBool(isClimbingLadderDownHash, false);
        }

        //  Cancels climbing when touching the ground at the bottom of ladder
        if (isTouchingGround && charController.isGrounded)
            CancelClimbing();

        //  Cancels climb when distance between ladder length and player is too far. Using this method over OnTriggerExit due to bugs
        if (Vector2.Distance(currentLadderBoxCollider.center + currentLadderBoxCollider.transform.position, transform.position) >= currentLadderBoxCollider.size.y/2)
            CancelClimbing();
    }
    #endregion

    #region ClimbRope()
    private void ClimbRope()
    {
        //  Get input from y axis.
        float yAxisInput = Input.GetAxisRaw("Vertical");
        float xAxisInput = Input.GetAxisRaw("Horizontal");

        //  if player insputs left or right... apply forces to rope
        if (xAxisInput != 0)
        {
            if (xAxisInput > 0 && canSwingRight)
            {
                canSwingRight = false;
                canSwingLeft = true;

                //  Apply swing force in the right direction
                currentRopeRigidBody.AddForce(Vector2.right * swingForce, ForceMode.Force);

                //  Animation
                if (facingDirection == FacingDirection.Right)
                {
                    animator.SetBool(isSwingingForwardHash, true);
                    animator.SetBool(isSwingingBackwardHash, false);
                }
                else
                {
                    animator.SetBool(isSwingingForwardHash, false);
                    animator.SetBool(isSwingingBackwardHash, true);
                }
            }
            else if (xAxisInput < 0 && canSwingLeft)
            {
                canSwingRight = true;
                canSwingLeft = false;

                //  Apply swing force to the left direction
                currentRopeRigidBody.AddForce(Vector2.left * swingForce, ForceMode.Force);

                //  Animation
                if (facingDirection == FacingDirection.Right)
                {
                    animator.SetBool(isSwingingForwardHash, false);
                    animator.SetBool(isSwingingBackwardHash, true);
                }
                else
                {
                    animator.SetBool(isSwingingForwardHash, true);
                    animator.SetBool(isSwingingBackwardHash, false);
                }
            }
        }
        else
        {
            //  Animation - not swinging
            animator.SetBool(isSwingingForwardHash, false);
            animator.SetBool(isSwingingBackwardHash, false);

            //  if player inputs up or down...
            if (yAxisInput > 0)
            {
                //  Animation - ClimbLadder up
                animator.SetBool(isClimbingRopeUpHash, true);
                animator.SetBool(isClimbingRopeDownHash, false);
            }
            else if (yAxisInput < 0)
            {
                //  Animation - ClimbLadder down
                animator.SetBool(isClimbingRopeUpHash, false);
                animator.SetBool(isClimbingRopeDownHash, true);
            }
            else
            {
                //  Animation - ClimbLadder Idle
                animator.SetBool(isClimbingRopeUpHash, false);
                animator.SetBool(isClimbingRopeDownHash, false);
            }

            //  Move vertically
            transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + yAxisInput * ropeClimbSpeed * Time.deltaTime);
        }


        //  Cancels climbing when touching the ground at the bottom of ladder
        //if (isTouchingGround && charController.isGrounded)
        //CancelClimbing();

        //  Cancels climb when distance between ladder length and player is too far. Using this method over OnTriggerExit due to bugs
        /*float distance = Vector2.Distance(currentRopeRigidBody.transform.parent.position, transform.position);
        if (distance >= currentRopeRigidBody.transform.parent.localScale.y / 2f)
        {
            Debug.Log("distance: " + distance + " break distance: " + currentRopeRigidBody.transform.parent.localScale.y / 2f);
            CancelClimbing();
        }*/
    }
    #endregion

    #region CancelClimbing()
    private void CancelClimbing()
    {
        if (currentState == PlayerState.ClimbingLadder)
        {
            //  Revert collision agianst platforms when climbing downwards
            Physics.IgnoreLayerCollision(gameObject.layer, Layers.Platforms, false);
            
            // Animation
            animator.SetBool(isClimbingLadderUpHash, false);
            animator.SetBool(isClimbingLadderDownHash, false);
            animator.SetBool(isClimbingLadderHash, false);
        }
        
        else if (currentState == PlayerState.ClimbingRope)
        {
            //  Reset parent
            transform.SetParent(null);
            //  Reset rotation
            transform.rotation = Quaternion.identity;

            // Animation
            animator.SetBool(isClimbingRopeUpHash, false);
            animator.SetBool(isClimbingRopeDownHash, false);
            animator.SetBool(isClimbingRopeHash, false);
        }
        
        //  Set player state
        currentState = PlayerState.None;
    }
    #endregion

    #region LedgeClimbUp(): Called when player climbs up a ledge
    void ClimbUpLedge()
    {
        //  Update state
        currentState = PlayerState.ClimbingLedge;

        //  Reset the velocity so player does not slide
        velocity = Vector2.zero;

        //  Determine the direction of the ledge climb clip
        if (facingDirection == FacingDirection.Right)
            animator.SetTrigger(ledgeClimbUpRightTriggerHash);
        else
            animator.SetTrigger(ledgeClimbUpLeftTriggerHash);
    }
    #endregion

    #region OnLedgeClimbUpComplete(): Called when player completes ledge climbing animation. Animation Event
    public void OnLedgeClimbUpComplete()
    {
        //  Set state
        currentState = PlayerState.None;

        //  Determine the direction of the ledge climb clip
        if (facingDirection == FacingDirection.Right)
            transform.position = new Vector2(transform.position.x + 1, transform.position.y + 1.5f);
        else
            transform.position = new Vector2(transform.position.x - 1, transform.position.y + 1.5f);
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
            Die();

        #region Perform Ledge climbs if within ledge colliders
        if (other.CompareTag(Tags.Ledge))
        {
            if (true)
            {
                //  If the player inputs up and forward... evaluate
                float yAxisInput = Input.GetAxisRaw("Vertical");
                float xAxisInput = Input.GetAxisRaw("Horizontal");

                if (yAxisInput > 0 && (xAxisInput > 0 || xAxisInput < 0))
                {
                    ClimbUpLedge();
                }
            }
        }
        #endregion

        #region Update rope parents when climbing
        if (currentState == PlayerState.ClimbingRope && other.CompareTag(Tags.Rope))
        {
            if (Input.GetAxisRaw("Horizontal") == 0)
            {
                currentRopeRigidBody = other.GetComponent<Rigidbody>();
                transform.SetParent(other.transform);
                transform.localPosition = new Vector3(currentRopeRigidBody.transform.localPosition.x, transform.localPosition.y, transform.position.z);
                transform.localRotation = Quaternion.identity;
                transform.localScale = new Vector3(currentRopeRigidBody.transform.lossyScale.y, currentRopeRigidBody.transform.lossyScale.x, 1);
            }
        }
        #endregion
    }

    //  Called when a collider stay within another collider with isTrigger enabled
    void OnTriggerStay(Collider other)
    {
        #region Check Ladder Climb
        if (canClimb && currentState == PlayerState.None && other.CompareTag(Tags.Ladder))
        {
            //  If the player inputs up or down... evaluate
            float yAxisInput = Input.GetAxisRaw("Vertical");
            if (yAxisInput > 0 || (yAxisInput < 0 && !isTouchingGround))
            {
                //  Set state
                currentState = PlayerState.ClimbingLadder;

                //  Ignore collision agianst platforms when climbing upwards
                Physics.IgnoreLayerCollision(gameObject.layer, Layers.Platforms, true);

                //  Cache the ladder's BoxCollider
                currentLadderBoxCollider = other.GetComponent<BoxCollider>();

                //  Set position to match ladder
                if (facingDirection == FacingDirection.Right)
                    transform.position = new Vector3(other.transform.position.x - 0.5f, transform.position.y, transform.position.z);
                else
                    transform.position = new Vector3(other.transform.position.x + 0.5f, transform.position.y, transform.position.z);

                //  correct facing direction
                /*if (facingDirection == FacingDirection.Left)
                {
                    facingDirection = FacingDirection.Right;
                    //  Flip the global control rig
                    puppet2DGlobalControl.flip = false;
                }*/

                //  Reset horizontal speed so player does not slide horizontally during ladder use
                velocity.x = 0;

                // Animation
                animator.SetBool(isClimbingLadderHash, true);
                animator.SetFloat(yVelocityHash, 0);
            }
        }
        #endregion

        #region Check Rope Climb
        if (canClimb && currentState == PlayerState.None && other.CompareTag(Tags.Rope))
        {
            //  If the player inputs up or down... evaluate
            float yAxisInput = Input.GetAxisRaw("Vertical");
            if (yAxisInput > 0 || (yAxisInput < 0 && !isTouchingGround))
            {
                //  Set state
                currentState = PlayerState.ClimbingRope;

                canSwingRight = true;
                canSwingLeft = true;

                //  Cache the rope
                currentRopeRigidBody = other.GetComponent<Rigidbody>();

                //  Set position to match rope
                transform.position = other.transform.position;

                transform.SetParent(other.transform);
                transform.rotation = Quaternion.identity;

                //  Reset horizontal speed so player does not slide horizontally during ladder use
                velocity = Vector2.zero;

                // Animation
                animator.SetBool(isClimbingRopeHash, true);
                animator.SetFloat(yVelocityHash, 0);
            }
        }
        #endregion
    }

    //  Must use this because OnCollisionEnter/Exit does not work for character controller
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float hitVol = hit.controller.velocity.magnitude * velToVol;
        if (hitVol >= 1f)
        {
            if (hit.collider.CompareTag(Tags.Ground) || hit.collider.CompareTag(Tags.Platform))
            {
                if (!charSound.isPlaying)
                {
                    pa.randomizePitch(charSound);
                    charSound.PlayOneShot(playerImpactGrass, hitVol);
                }
            }
            else if (hit.collider.CompareTag(Tags.Box))
            {
                if (!charSound.isPlaying)
                {
                    pa.randomizePitch(charSound);
                    charSound.PlayOneShot(playerImpactWood, hitVol);
                }
            }
        }



        //  Evaluate what if the object hit is the ground (lowest platform/terrain)
        if (hit.collider.CompareTag(Tags.Ground))
            isTouchingGround = true;
        else
            isTouchingGround = false;
    }

}

