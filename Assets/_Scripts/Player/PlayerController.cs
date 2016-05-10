﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GlitchEffectsDLL;
using InControl;

public class PlayerController : MonoBehaviour 
{

    public enum player_state
    {
        IN_GROUND,
		PREPARING_JUMP,
        JUMPING,
		FALL_RECOVERING,
        TELEPORTING,
		DEATH
    };

    #region Variable Declaration

    //State
	public player_state state;
	public bool allowMovement;
	private bool godMode = false;

	//Player Components
	private SpriteRenderer spriteRenderer;			//Reference to the sprite renderer
	private Animator plAnimation;
	public Rigidbody rigidBody;

	//Particles
	private ParticleSystem glitchParticles;
	private ParticleSystem dustParticles;
	private ParticleSystem jumpParticles;

	//Movement Variables

	private bool playerActivedJump = false;		// The jump state is cause of a player jump? (If not, it could be a fall)

	private float zPosition;					// Position on the z axis. Unvariable
	public float speed = 12.5f;					// Horizontal speed
	public float gravity = -50.0f;				// Gravity
	public float maxJumpTime = 0.15f;			// Max time a jump can be extended
	public float jumpRest = 0.025f;				// Time of jump preparing and fall recovery

    [SerializeField]
    private float shortJumpSpeed = 20.0f;				// Base jump speed
    [HideInInspector]
    public float vSpeed = 0.0f;					// The vertical speed

    private float timePreparingJump = 0.0f;

	// Powers declarations
    [HideInInspector]
	public TeleportScript teleport;
    [HideInInspector]
    public SlowFPS slowFPS;

    private float distToGround;
    private BoxCollider boxCollider;

    private int layerMask = ~((1 << 1) | (1 << 2) | (1 << 4) | (1 << 5) | (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 13));


	float maxPos = 0.0f;

    #endregion

    #region Init and update

    void Start()
    {
        spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        plAnimation = transform.GetComponentInChildren<Animator>();

        glitchParticles = transform.FindChild("GlitchParticles").GetComponent<ParticleSystem>();
        jumpParticles = transform.FindChild("JumpParticles").GetComponent<ParticleSystem>();
        dustParticles = transform.FindChild("DustParticles").GetComponent<ParticleSystem>();

        teleport = transform.FindChild("Powers/Teleport").GetComponent<TeleportScript>();
        slowFPS = transform.FindChild("Powers/SlowFPS").GetComponent<SlowFPS>();

        boxCollider = transform.GetComponent<BoxCollider>();
        distToGround = boxCollider.bounds.extents.y;

        state = player_state.IN_GROUND;
        allowMovement = true;
    }

    void FixedUpdate()
    {
        // State-changing calculations
        switch (state)
        {
            case player_state.PREPARING_JUMP:

                timePreparingJump += Time.fixedDeltaTime;
                vSpeed = shortJumpSpeed;
                //If it's ready to jump, start jump and give fall recovery time
                state = player_state.JUMPING;
                plAnimation.SetBool("Jump", true);
                plAnimation.SetBool("Run", false);
                jumpParticles.Play();
    
                Movement();
                break;

		case player_state.FALL_RECOVERING:


				state = player_state.IN_GROUND;
				vSpeed = 0;

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
                        vSpeed = 0;
                    }
                    else if (!IsGrounded())
                    {
                        state = player_state.JUMPING;
                        plAnimation.SetBool("Falling", true);
                        plAnimation.SetBool("Run", false);
                    }
                    else
                    {
                        vSpeed = 0;
                    }

                    // To control movement of player
                    Movement();

                }
                break;

            case player_state.JUMPING:

                if (vSpeed < 0 && plAnimation.GetBool("Falling") == false)
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
						vSpeed = 0;
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

                        if(playerActivedJump && timePreparingJump <= maxJumpTime && InputManager.ActiveDevice.Action1.IsPressed)
                        {
                            vSpeed = shortJumpSpeed;
                        }

                        //If it's in the air
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
					boxCollider.enabled = true;
                }

                break;
        }

        //If a player-induced jump is checked but the jump key is not longer
        //being held, set it to false so it can jump again
        if (playerActivedJump && !InputManager.ActiveDevice.Action1.IsPressed && allowMovement)
            playerActivedJump = false;

    }

    private void Movement()
    {
        Vector3 moveDirection = Vector3.zero;

        float verticalMove = 0.0f;
        // Gravity
        if(playerActivedJump && timePreparingJump > maxJumpTime && (timePreparingJump - Time.fixedDeltaTime < maxJumpTime) && InputManager.ActiveDevice.Action1.IsPressed)
        {
            verticalMove = vSpeed * (maxJumpTime - (timePreparingJump - Time.fixedDeltaTime));
            verticalMove += vSpeed * (timePreparingJump - maxJumpTime) + 0.5f * gravity * Mathf.Pow((timePreparingJump - maxJumpTime), 2f);
			vSpeed += gravity * (timePreparingJump - maxJumpTime);
        }
		else if(playerActivedJump && timePreparingJump <= maxJumpTime)
		{
			verticalMove = vSpeed * Time.fixedDeltaTime;
		}
        else
        {
            verticalMove = vSpeed * Time.fixedDeltaTime + 0.5f * gravity * Mathf.Pow(Time.fixedDeltaTime, 2f);
	        vSpeed += gravity * Time.fixedDeltaTime;
        }

        //If the player is allowed to move
        if (allowMovement)
        {
            // Control of movemente in X axis
            moveDirection.x = InputManager.ActiveDevice.LeftStickX.Value;

            // Flips the sprite renderer if is changing direction
            if ((moveDirection.x > 0) && (spriteRenderer.flipX == true))
            {

                spriteRenderer.flipX = false;

                Vector3 dustPosition = dustParticles.gameObject.transform.localPosition;
                dustPosition.x *= -1;
                dustParticles.gameObject.transform.localPosition = dustPosition;

                Quaternion dustRotation = dustParticles.gameObject.transform.localRotation;
                dustRotation.y *= -1;
                dustParticles.gameObject.transform.localRotation = dustRotation;

            }
            else if ((moveDirection.x < 0) && (spriteRenderer.flipX == false))
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
            moveDirection.x = 0;
        }

        moveDirection.x *= speed * Time.fixedDeltaTime;
        moveDirection.y = verticalMove;

        Vector3 position = transform.position + moveDirection;

        if (position.z != zPosition)
            position.z = zPosition;

        rigidBody.MovePosition(position);

        if (state == player_state.IN_GROUND && moveDirection.x != 0)
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

        //Plays the dust particle effect
        if (state == player_state.IN_GROUND && moveDirection.x != 0)
        {
            if (dustParticles.isStopped)
            {
                dustParticles.Play();
            }
        }
        else if (dustParticles.isPlaying)
        {
            dustParticles.Stop();
        }
    }


    #endregion

    #region Functions

    public bool ActivatingTeleport()
    {
        if(InputManager.ActiveDevice.Action3.WasPressed && allowMovement && !teleport.teleportUsed && teleport.CheckTeleport(boxCollider))
        {
            state = player_state.TELEPORTING;
            vSpeed = 0.0f;
            plAnimation.SetTrigger("Teleport");
            plAnimation.speed = 1.0f / teleport.getDuration();
            rigidBody.detectCollisions = false;
            boxCollider.enabled = false;
            DoGlitchParticles();
            return true;
        }
        return false;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(new Vector3(transform.position.x + boxCollider.bounds.extents.x, transform.position.y, transform.position.z), -Vector3.up, distToGround + 0.1f, layerMask) ||
            Physics.Raycast(new Vector3(transform.position.x - boxCollider.bounds.extents.x, transform.position.y, transform.position.z), -Vector3.up, distToGround + 0.1f, layerMask);
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
        vSpeed = 0.0f;
    }

    #endregion

}