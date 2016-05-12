using UnityEngine;
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

	private float zPosition = 0.0f;				// Position on the z axis. Unvariable
	public float speed = 12.5f;					// Horizontal speed
	public float maxJumpTime = 0.25f;			// Max time a jump can be extended
    public float jumpForce = 700.0f;				// Base jump speed
    private float timePreparingJump = 0.0f;

    public float maxSpeed = 10.0f;
    public float increaseSpeed = 2.0f;
    public float decreaseSpeedWhenIdle = 1.0f;

	// Powers declarations
    [HideInInspector]
	public TeleportScript teleport;
    [HideInInspector]
    public SlowFPS slowFPS;

    private float distToGround;
    private BoxCollider boxCollider;

    private int layerMask = ~((1 << 1) | (1 << 2) | (1 << 4) | (1 << 5) | (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 13));

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
                    rigidBody.useGravity = true;
                    rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0.0f, 0.0f);                    
                }

                break;

            case player_state.DEATH:
                rigidBody.velocity = Vector3.zero;
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
        Vector3 currentVelocity = rigidBody.velocity;

        //If the player is allowed to move
        if (allowMovement)
        {

            // Control of movemente in X axis
            moveDirection.x = InputManager.ActiveDevice.LeftStickX.Value;

            if (moveDirection.x >= 0.5f)
            {
                currentVelocity.x = Mathf.Min(maxSpeed, currentVelocity.x + increaseSpeed);
            }
            else if (moveDirection.x <= -0.5f)
            {
                currentVelocity.x = Mathf.Max(-maxSpeed, currentVelocity.x - increaseSpeed);
            }
            else
            {
                if (currentVelocity.x > 0.0f)
                {
                    currentVelocity.x = Mathf.Max(0.0f, currentVelocity.x - decreaseSpeedWhenIdle);
                }
                else if (currentVelocity.x < 0.0f)
                {
                    currentVelocity.x = Mathf.Min(0.0f, currentVelocity.x + decreaseSpeedWhenIdle);
                }
            }


            // Flips the sprite renderer if is changing direction
            if ((currentVelocity.x > 0.0f) && (spriteRenderer.flipX == true))
            {

                spriteRenderer.flipX = false;

                Vector3 dustPosition = dustParticles.gameObject.transform.localPosition;
                dustPosition.x *= -1;
                dustParticles.gameObject.transform.localPosition = dustPosition;

                Quaternion dustRotation = dustParticles.gameObject.transform.localRotation;
                dustRotation.y *= -1;
                dustParticles.gameObject.transform.localRotation = dustRotation;

            }
            else if ((currentVelocity.x < 0.0f) && (spriteRenderer.flipX == false))
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

        Vector3 position = transform.position;

        if (position.z != zPosition)
            position.z = zPosition;

        transform.position = position;

        rigidBody.velocity = currentVelocity;

        if (state == player_state.IN_GROUND && currentVelocity.x != 0)
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
        if (state == player_state.IN_GROUND && currentVelocity.x != 0)
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
        if(InputManager.ActiveDevice.Action3.IsPressed && allowMovement && !teleport.teleportUsed && teleport.CheckTeleport(boxCollider))
        {
            state = player_state.TELEPORTING;
            rigidBody.useGravity = false;
            plAnimation.SetTrigger("Teleport");
            plAnimation.speed = 1.0f / teleport.getDuration();
            rigidBody.detectCollisions = false;
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
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0.0f, 0.0f);
    }

    #endregion

}