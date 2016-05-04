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
	private SpriteRenderer _spriteRenderer;			//Reference to the sprite renderer
	private Animator _plAnimation;
	public Rigidbody _rigidBody;

	//Particles
	private ParticleSystem _glitchParticles;
	private ParticleSystem _dustParticles;
	private ParticleSystem _jumpParticles;

	//Movement Variables

	private bool playerActivedJump = false;		// The jump state is cause of a player jump? (If not, it could be a fall)

	private float _zPosition;					// Position on the z axis. Unvariable
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

    private float _distToGround;
    private BoxCollider _boxCollider;

    #endregion

    #region Init and update

    void Start()
    {
        _spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        _plAnimation = transform.GetComponentInChildren<Animator>();

        _glitchParticles = transform.FindChild("GlitchParticles").GetComponent<ParticleSystem>();
        _jumpParticles = transform.FindChild("JumpParticles").GetComponent<ParticleSystem>();
        _dustParticles = transform.FindChild("DustParticles").GetComponent<ParticleSystem>();

        teleport = transform.FindChild("Powers/Teleport").GetComponent<TeleportScript>();
        slowFPS = transform.FindChild("Powers/SlowFPS").GetComponent<SlowFPS>();

        _boxCollider = transform.GetComponent<BoxCollider>();
        _distToGround = _boxCollider.bounds.extents.y;

        state = player_state.IN_GROUND;
        allowMovement = true;
    }

    void Update()
    {

    }

    #endregion

    #region Functions

    public bool ActivatingTeleport()
    {
        if(InputManager.ActiveDevice.Action3.WasPressed && allowMovement && !teleport.teleportUsed && teleport.CheckTeleport(_boxCollider)
        {
            state = player_state.TELEPORTING;
            vSpeed = 0.0f;
            _plAnimation.SetBool("Teleport", true);
            _plAnimation.speed = 1.0f / teleport.getDuration();
            _rigidBody.detectCollisions = false;
            DoGlitchParticles();
            return true;
        }
        return false;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, _distToGround + 0.1f;)
    }

    #endregion

    #region Particles

    public void DoGlitchParticles()
    {
        _glitchParticles.Play();
    }

    #endregion

    #region Colliders

    void OnCollisionEnter(Collision collision)
    {
        vSpeed = 0.0f;
    }

    #endregion

}