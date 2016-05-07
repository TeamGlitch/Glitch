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
	public float speed = 7.2f;					// Horizontal speed
	public float jumpSpeed = 8.0f;				// Base jump speed
	public float gravity = 22.0f;				// Gravity
	public float maxJumpTime = 0.33f;			// Max time a jump can be extended
	public float jumpRest = 0.025f;				// Time of jump preparing and fall recovery

    [HideInInspector]
    public float vSpeed = 0.0f;					// The vertical speed

	private float preparingJump = 0;				//Jump preparing time left
	private float fallRecovery = 0;					//Fall recovery time left
	private int nonGroundedFrames = 0;				// How many frames the player has being on air.

	// Powers declarations
    [HideInInspector]
	public TeleportScript teleport;
    [HideInInspector]
    public SlowFPS slowFPS;

    private float distToGround;
    private BoxCollider boxCollider;

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

    void Update()
    {

        Vector3 moveDirection = new Vector3(0, 0, 0);

        // State-changing calculations
        switch (state)
        {
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
                        vSpeed = jumpSpeed;
                        playerActivedJump = true;
                        state = player_state.JUMPING;
                    }
                    else if (!IsGrounded())
                    {
                        state = player_state.JUMPING;
                    }
                    else
                    {
                        vSpeed = 0;
                    }

                    // To control movement of player
                    Movement(moveDirection);

                }
                break;

            case player_state.JUMPING:
                // If it's not teleporting
                if (!ActivatingTeleport())
                {
                    //If it's grounded
                    if (IsGrounded())
                    {
                        //Start fall recovering and set the bools
                        state = player_state.IN_GROUND;
                    }
                    else
                    {
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
                    Movement(moveDirection);
                }
                break;

            case player_state.TELEPORTING:

                Vector3 position;
                bool ended = teleport.movePosition(out position);
                transform.position = position;

                if (ended)
                {
                    state = player_state.JUMPING;
                    plAnimation.speed = 1;
                    rigidBody.detectCollisions = true;
                }

                break;
        }

        //If a player-induced jump is checked but the jump key is not longer
        //being held, set it to false so it can jump again
        if (playerActivedJump && !InputManager.ActiveDevice.Action1.IsPressed && allowMovement)
            playerActivedJump = false;

    }

    private void Movement(Vector3 moveDirection)
    {
        // Gravity
        vSpeed -= gravity * Time.deltaTime;

        //If the player is allowed to move
        if (allowMovement)
        {

            // Control of movemente in X axis
            moveDirection.x = InputManager.ActiveDevice.LeftStickX.Value;
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

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

        moveDirection.y = vSpeed;

        rigidBody.velocity = moveDirection;
        if (transform.position.z != zPosition)
        {
            Vector3 pos = transform.position;
            pos.z = zPosition;
            transform.position = pos;
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
            plAnimation.SetBool("Teleport", true);
            plAnimation.speed = 1.0f / teleport.getDuration();
            rigidBody.detectCollisions = false;
            DoGlitchParticles();
            return true;
        }
        return false;
    }

    private bool IsGrounded()
    {
        return Physics.CheckBox(boxCollider.bounds.center,new Vector3(boxCollider.bounds.center.x,boxCollider.bounds.min.y-0.1f,boxCollider.bounds.center.z));

//        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
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