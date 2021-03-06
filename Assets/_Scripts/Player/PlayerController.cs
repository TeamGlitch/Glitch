﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GlitchEffectsDLL;
using InControl;

public class PlayerController : MonoBehaviour 
{
    public AudioClip jumpSound;
    public AudioClip walkSound;
    private AudioSource walkSource = null;

    public GlitchOffsetCamera glitchOffsetCamera;

    public enum player_state
    {
        IN_GROUND,
		PREPARING_JUMP,
        JUMPING,
		FALL_RECOVERING,
        TELEPORTING,
		STICKED,
		DEATH
    };

    #region Variable Declaration

    //State
	public player_state state;
	public bool allowMovement;       

	//Player Components
	private SpriteRenderer spriteRenderer;			//Reference to the sprite renderer
	private Animator plAnimation;
	public Rigidbody rigidBody;

	//Particles
	private ParticleSystem glitchParticles;
	private ParticleSystem dustParticles;
	private ParticleSystem jumpParticles;

	//Movement Variables

	public bool playerActivedJump = false;		// The jump state is cause of a player jump? (If not, it could be a fall)

	private float zPosition = 0.0f;				// Position on the z axis. Unvariable
	public float maxJumpTime = 0.25f;			// Max time a jump can be extended
    public float jumpForce = 700.0f;				// Base jump speed
    public float stickedJumpForce = 1000.0f;         // Force in jump from sticked surface
    private float timePreparingJump = 0.0f;
    public float maxTimeSticked = 0.5f;

    public float maxSpeedInAir = 20.0f;
    public float decreaseSpeedWhenIdle = 1.0f;

	private Vector3 directionStickObject;

	// Powers declarations
    [HideInInspector]
	public TeleportScript teleport;
    [HideInInspector]
    public SlowFPS slowFPS;

    private float distToGround;
    private BoxCollider boxCollider;


    public enum moving_type
    {
        IDLE,
        GOING_RIGHT,
        GOING_LEFT,
        CHANGING_DIRECTION,
        STOPING
    };

    public float maxSpeed = 15.0f;

    private float velocityWhenChangedState = 0.0f;
    [SerializeField]
    private float timeToMaxVelocity = 1.0f;
    [SerializeField]
    private float timeToStop = 0.1f;
    [SerializeField]
    private float minSpeed = 2.0f;
    private float timeToChangeDependingVelocity = 0.0f;
    private float timeSinceChangeMoving;
    private bool moveToRight = true;
    private bool playerIsMoving = false;
    private bool playerJumpingInSticked = false;

	private bool previouslySticked = false;

    public moving_type playerMovingType = moving_type.IDLE;

	private int layerMask = ~((1 << 1) | (1 << 2) | (1 << 4) | (1 << 5) | (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 13));

    #endregion

    #region Init and update

    void Awake()
    {
        boxCollider = transform.GetComponent<BoxCollider>();
        distToGround = boxCollider.bounds.extents.y;
    }

    void Start()
    {
        allowMovement = true;
        spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        plAnimation = transform.GetComponentInChildren<Animator>();

        glitchParticles = transform.FindChild("GlitchParticles").GetComponent<ParticleSystem>();
        jumpParticles = transform.FindChild("JumpParticles").GetComponent<ParticleSystem>();
        dustParticles = transform.FindChild("DustParticles").GetComponent<ParticleSystem>();

        teleport = transform.FindChild("Powers/Teleport").GetComponent<TeleportScript>();
        slowFPS = transform.FindChild("Powers/SlowFPS").GetComponent<SlowFPS>();

        state = player_state.IN_GROUND;
    }

    void FixedUpdate()
    {
		previouslySticked = false;

        // State-changing calculations
        switch (state)
        {
            case player_state.PREPARING_JUMP:

                SoundManager.instance.PlaySingle(jumpSound);

                timePreparingJump += Time.deltaTime;
                rigidBody.AddForce(new Vector3(0.0f, jumpForce, 0.0f));
                //If it's ready to jump, start jump and give fall recovery time
                state = player_state.JUMPING;
                plAnimation.SetBool("Jump", true);
                plAnimation.SetBool("Run", false);
                jumpParticles.Play();
    
                Movement();
                break;

		    case player_state.FALL_RECOVERING:


				state = player_state.IN_GROUND;

				plAnimation.SetBool ("Run", false);
				plAnimation.SetBool ("Jump", false);
				plAnimation.SetBool ("Falling", false);
	

                // To control movement of player
                Movement();

                break;


			case player_state.IN_GROUND:

                // If it's not teleporting
                if (!ActivatingTeleport())
                {
                    teleport.teleportUsed = false;

                    if (transform.parent != null)
                    {
                        transform.rotation = transform.parent.rotation;
                    }

                    //If the jump key is being pressed but it has been released since the
                    //last jump
					if (InputManager.ActiveDevice.Action1.IsPressed && allowMovement && !playerActivedJump)
                    {
                        timePreparingJump = 0.0f;
                        playerActivedJump = true;
                        state = player_state.PREPARING_JUMP;
                        rigidBody.useGravity = false;
                    }
                    else if (!IsGrounded())
                    {
                        state = player_state.JUMPING;
                        plAnimation.SetBool("Falling", true);
                        plAnimation.SetBool("Run", false);
                    }


                    // To control movement of player
                    Movement();

                }
                break;

            case player_state.JUMPING:

				//Change animation to falling
                if (rigidBody.velocity.y < 0 && plAnimation.GetBool("Falling") == false)
                {
                    plAnimation.SetBool("Jump", false);
                    plAnimation.SetBool("Falling", true);
                }

                // If it's not teleporting
                if (!ActivatingTeleport())
                {
                    //If it's grounded
                    if (IsGrounded())
                    {
                        //Start fall recovering and set the bools
                        state = player_state.FALL_RECOVERING;
                        plAnimation.SetBool("Falling", false);
                        if (plAnimation.GetBool("Jump") == true)
                        {
                            plAnimation.SetBool("Jump", false);
                        }
                    }
                    else
                    {

                        timePreparingJump += Time.fixedDeltaTime;

                        if(!playerActivedJump || (timePreparingJump > maxJumpTime))
                        {
                            rigidBody.useGravity = true;
                        }

                        //Angle correction
                        Vector3 eulerAngles = gameObject.transform.rotation.eulerAngles;
                        float rotationZ = 0.0f;

                        if (eulerAngles.z < 0.0f)
                        {
                            eulerAngles.z += 360.0f;
                        }

                        if (eulerAngles.z != 0.0f)
                        {
                            if (eulerAngles.z <= 3.0f || eulerAngles.z >= 357.0f)
                            {
                                rotationZ = 0.0f;
                            }
                            else if (eulerAngles.z <= 180.0f)
                            {
                                rotationZ = eulerAngles.z - 3.0f;
                            }
                            else if (eulerAngles.z > 180.0f)
                            {
                                rotationZ = eulerAngles.z + 3.0f;
                            }
                            gameObject.transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, rotationZ);
                        }
						//END Angle correction
                    }
                    // To control movement of player
                    Movement();
                }
                break;

            case player_state.TELEPORTING:

                Vector3 position;
                bool ended = teleport.movePosition(out position);
                rigidBody.MovePosition(position);

                if (ended)
                {
                    state = player_state.JUMPING;
                    plAnimation.speed = 1;
                    rigidBody.detectCollisions = true;
                    rigidBody.useGravity = true;
                    rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0.0f, 0.0f);
                    if (!slowFPS.powerActive)
                    {
                        glitchOffsetCamera.enabled = false;
                    }
                    else
                    {
                        glitchOffsetCamera.divisions = 20;
                        glitchOffsetCamera.inestability = 0.3f;
                        glitchOffsetCamera.frequency = 0.5f;
                    }
                }

                break;

            case player_state.DEATH:
                rigidBody.velocity = Vector3.zero;

                if (walkSource != null && walkSource.isPlaying)
                    walkSource.Stop();

                break;

			case player_state.STICKED:
				previouslySticked = true;
                timeSinceChangeMoving += Time.deltaTime;

                if (InputManager.ActiveDevice.Action1.IsPressed && !playerJumpingInSticked) {

                    playerJumpingInSticked = true;

                    rigidBody.useGravity = true;
					rigidBody.AddForce(new Vector3(0.0f, stickedJumpForce, 0.0f));
					rigidBody.velocity = new Vector3 (4f, -directionStickObject.x * rigidBody.velocity.y, 0.0f);
					
					velocityWhenChangedState = -directionStickObject.x * 20f;
					timeToChangeDependingVelocity = 1f;
					timeSinceChangeMoving = 0.0f;
					playerMovingType = moving_type.STOPING;

					state = player_state.JUMPING;
				}
                else if(timeSinceChangeMoving > maxTimeSticked)
                {
                    rigidBody.useGravity = true;
                    playerJumpingInSticked = false;

                    velocityWhenChangedState = 0f;
                    timeToChangeDependingVelocity = 1f;
                    timeSinceChangeMoving = 0.0f;
                    playerMovingType = moving_type.STOPING;

                    state = player_state.JUMPING;
                }
                else if(playerJumpingInSticked && !InputManager.ActiveDevice.Action1.IsPressed)
                {
                    playerJumpingInSticked = false;
                }
                break;
        }

        //If a player-induced jump is checked but the jump key is not longer
        //being held, set it to false so it can jump again
		if (playerActivedJump && !InputManager.ActiveDevice.Action1.IsPressed && allowMovement){
            playerActivedJump = false;
		}

    }

    private void Movement()
    {
        Vector3 moveDirection = Vector3.zero;
		float currentVelocity = rigidBody.velocity.x;
        bool isInGround = IsGrounded();

		if (allowMovement)
        {
            // Control of movemente in X axis
            moveDirection.x = InputManager.ActiveDevice.LeftStickX.Value;

			//If there's not enough input and speed
            if (rigidBody.velocity.x == 0.0f && moveDirection.x < 0.5f && moveDirection.x > -0.5f)
            {
                playerMovingType = moving_type.IDLE;
            }
			//If there's not enough input but there's speed
            else if (rigidBody.velocity.x != 0.0f && moveDirection.x < 0.5f && moveDirection.x > -0.5f)
            {
                if (playerMovingType != moving_type.STOPING)
                {
                    velocityWhenChangedState = rigidBody.velocity.x;
					timeToChangeDependingVelocity = timeToStop * Mathf.Abs(velocityWhenChangedState) / maxSpeed;
                    timeSinceChangeMoving = 0.0f;
                    playerMovingType = moving_type.STOPING;
                }
                else
                {
                    timeSinceChangeMoving += Time.fixedDeltaTime;
                }
            }
            else if ((rigidBody.velocity.x > 0.0f && moveDirection.x <= -0.5f) ||
                        (rigidBody.velocity.x < 0.0f && moveDirection.x >= 0.5f))
            {
                if (playerMovingType != moving_type.CHANGING_DIRECTION)
                {
                    playerMovingType = moving_type.CHANGING_DIRECTION;
                }
                else
                {
                    timeSinceChangeMoving += Time.fixedDeltaTime;
                }
            }
            else if (rigidBody.velocity.x >= 0.0f && moveDirection.x >= 0.5f)
            {
                if (playerMovingType != moving_type.GOING_RIGHT)
                {
                    velocityWhenChangedState = Mathf.Max(rigidBody.velocity.x, minSpeed);
                    timeToChangeDependingVelocity = timeToMaxVelocity * (maxSpeed - velocityWhenChangedState) / (maxSpeed - minSpeed);
                    timeSinceChangeMoving = 0.0f;
                    playerMovingType = moving_type.GOING_RIGHT;
                }
                else
                {
                    timeSinceChangeMoving += Time.fixedDeltaTime;
                }
            }
            else if (rigidBody.velocity.x <= 0.0f && moveDirection.x <= -0.5f)
            {
                if (playerMovingType != moving_type.GOING_LEFT)
                {
                    velocityWhenChangedState = Mathf.Min(rigidBody.velocity.x, -minSpeed);
                    timeToChangeDependingVelocity = timeToMaxVelocity * (maxSpeed - Mathf.Abs(velocityWhenChangedState)) / (maxSpeed - minSpeed);
                    timeSinceChangeMoving = 0.0f;
                    playerMovingType = moving_type.GOING_LEFT;
                }
                else
                {
                    timeSinceChangeMoving += Time.fixedDeltaTime;
                }
            }
            else
            {
                Debug.Log("AIXO CREC QUE NO POT ARRIBAR MAI A PASAR OMG OMG OMG. Hola mama! Estoy en la tele!");
            }

            switch (playerMovingType)
            {
                case moving_type.STOPING:
                    currentVelocity = Mathf.Lerp(velocityWhenChangedState, 0.0f, timeSinceChangeMoving / timeToChangeDependingVelocity);
                    break;
                case moving_type.CHANGING_DIRECTION:
                    if (moveDirection.x > 0.0f)
                    {
                        currentVelocity = minSpeed;
                    }
                    else if (moveDirection.x < 0.0f)
                    {
                        currentVelocity = -minSpeed;
                    }
                    break;
                case moving_type.GOING_RIGHT:
                    currentVelocity = Mathf.Lerp(velocityWhenChangedState, maxSpeed, timeSinceChangeMoving / timeToChangeDependingVelocity);
                    break;
                case moving_type.GOING_LEFT:
                    currentVelocity = Mathf.Lerp(velocityWhenChangedState, -maxSpeed, timeSinceChangeMoving / timeToChangeDependingVelocity);
                    break;
            }

            // Flips the sprite renderer if is changing direction
            if ((currentVelocity > 0.0f) && (spriteRenderer.flipX == true))
            {

                spriteRenderer.flipX = false;

                Vector3 dustPosition = dustParticles.gameObject.transform.localPosition;
                dustPosition.x *= -1;
                dustParticles.gameObject.transform.localPosition = dustPosition;

                Quaternion dustRotation = dustParticles.gameObject.transform.localRotation;
                dustRotation.y *= -1;
                dustParticles.gameObject.transform.localRotation = dustRotation;

            }
            else if ((currentVelocity < 0.0f) && (spriteRenderer.flipX == false))
            {

                spriteRenderer.flipX = true;
                Vector3 dustPosition = dustParticles.gameObject.transform.localPosition;
                dustPosition.x *= -1;
                dustParticles.gameObject.transform.localPosition = dustPosition;

                Quaternion dustRotation = dustParticles.gameObject.transform.localRotation;
                dustRotation.y *= -1;
                dustParticles.gameObject.transform.localRotation = dustRotation;

            }
        }
        else
        {
			currentVelocity = 0.0f;
        }

        // Correct Z position
        Vector3 position = transform.position;
        if (position.z != zPosition)
            position.z = zPosition;
        transform.position = position;

		//If there's horizontal movement
		if (!isInGround && currentVelocity != 0f) {

			//Three raycast are made
			Vector3 pos1 = transform.position;
			pos1.y -= boxCollider.bounds.extents.y;

			Vector3 pos2 = transform.position;

			Vector3 pos3 = transform.position;
			pos3.y += boxCollider.bounds.extents.y;


			//If it's to the right
			if (currentVelocity > 0f){

				//Checks the raycast
				//OPTIMIZATION NOTE: this if CAN'T be included in the last one)
				if (Physics.Raycast (pos1, Vector3.right, boxCollider.bounds.extents.x * 2, layerMask, QueryTriggerInteraction.Ignore)
					|| Physics.Raycast (pos2, Vector3.right, boxCollider.bounds.extents.x * 2, layerMask, QueryTriggerInteraction.Ignore)
					|| Physics.Raycast (pos3, Vector3.right, boxCollider.bounds.extents.x * 2, layerMask, QueryTriggerInteraction.Ignore)) {
					currentVelocity = 0f;
				}

			} else {

				//Checks the raycast
				//OPTIMIZATION NOTE: this if CAN'T be included in the last one)
				if (!isInGround && (Physics.Raycast (pos1, Vector3.left, boxCollider.bounds.extents.x * 2, layerMask, QueryTriggerInteraction.Ignore)
					|| Physics.Raycast (pos2, Vector3.left, boxCollider.bounds.extents.x * 2, layerMask, QueryTriggerInteraction.Ignore)
					|| Physics.Raycast (pos3, Vector3.left, boxCollider.bounds.extents.x * 2, layerMask, QueryTriggerInteraction.Ignore))) {
					currentVelocity = 0f;
				}
			}
				
		}

		Vector3 velocity = rigidBody.velocity;
        if(!float.IsNaN(currentVelocity))
            velocity.x = currentVelocity;
		rigidBody.velocity = velocity;

        if (state == player_state.IN_GROUND && currentVelocity != 0)
        {
            if (plAnimation.GetBool("Run") == false)
            {
                plAnimation.SetBool("Run", true);
            }
        }
        else if (plAnimation.GetBool("Run") == true)
        {
            plAnimation.SetBool("Run", false);
        }


        //Plays the dust particle effect & walking sound
        if (state == player_state.IN_GROUND && currentVelocity != 0)
        {
            if (dustParticles.isStopped)
            {
                dustParticles.Play();
            }

            if (walkSource == null || !walkSource.isPlaying){
                walkSource = SoundManager.instance.PlaySingle(walkSound);
            }
        }
        else
        {
            if (dustParticles.isPlaying)
                dustParticles.Stop();

            if (walkSource != null)
            {
                walkSource.Stop();
                walkSource = null;
            }
        }
    }


    #endregion

    #region Functions

    public bool ActivatingTeleport()
    {
        if (InputManager.ActiveDevice.Action3.IsPressed && allowMovement && !teleport.teleportUsed && teleport.CheckTeleport(boxCollider))
        {
            state = player_state.TELEPORTING;
            rigidBody.useGravity = false;
            plAnimation.SetTrigger("Teleport");
            plAnimation.speed = 1.0f / teleport.getDuration();
            rigidBody.detectCollisions = false;
            DoGlitchParticles();
            glitchOffsetCamera.divisions = 50;
            glitchOffsetCamera.inestability = 1.0f;
            glitchOffsetCamera.frequency = 1.0f;
            glitchOffsetCamera.enabled = true;
            return true;
        }
        return false;
    }

	public void GetSticked(Vector3 directionStick){
		state = player_state.STICKED;
        timeSinceChangeMoving = 0.0f;
        rigidBody.useGravity = false;
		rigidBody.velocity = new Vector3(0, 0, 0);
        if (InputManager.ActiveDevice.Action1.IsPressed)
            playerJumpingInSticked = true;
        else
            playerJumpingInSticked = false;

        directionStickObject = directionStick;
	}

    private bool IsGrounded()
    {
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f, layerMask, QueryTriggerInteraction.Ignore) ||
			Physics.Raycast(new Vector3(transform.position.x + boxCollider.bounds.extents.x / 2.0f, transform.position.y, transform.position.z), -Vector3.up, distToGround + 0.1f, layerMask, QueryTriggerInteraction.Ignore) ||
			Physics.Raycast(new Vector3(transform.position.x - boxCollider.bounds.extents.x / 2.0f, transform.position.y, transform.position.z), -Vector3.up, distToGround + 0.1f, layerMask, QueryTriggerInteraction.Ignore) ||
			Physics.Raycast(new Vector3(transform.position.x + boxCollider.bounds.extents.x, transform.position.y, transform.position.z), -Vector3.up, distToGround + 0.1f, layerMask, QueryTriggerInteraction.Ignore) ||
			Physics.Raycast(new Vector3(transform.position.x - boxCollider.bounds.extents.x, transform.position.y, transform.position.z), -Vector3.up, distToGround + 0.1f, layerMask, QueryTriggerInteraction.Ignore);
    }

    private bool IsTopColision()
    {
		return Physics.Raycast(transform.position, Vector3.up, distToGround + 0.1f, layerMask, QueryTriggerInteraction.Ignore) ||
			Physics.Raycast(new Vector3(transform.position.x + boxCollider.bounds.extents.x, transform.position.y, transform.position.z), Vector3.up, distToGround + 0.1f, layerMask, QueryTriggerInteraction.Ignore) ||
			Physics.Raycast(new Vector3(transform.position.x - boxCollider.bounds.extents.x, transform.position.y, transform.position.z), Vector3.up, distToGround + 0.1f, layerMask, QueryTriggerInteraction.Ignore);
    }

    #endregion

    #region Particles

    public void DoGlitchParticles()
    {
        glitchParticles.Play();
    }

    #endregion

    #region Colliders

    void OnCollisionEnter(Collision collision)
    {
		if (IsTopColision ()) {
			rigidBody.velocity = new Vector3 (rigidBody.velocity.x, 0.0f, 0.0f);
		}
    }

    #endregion

}