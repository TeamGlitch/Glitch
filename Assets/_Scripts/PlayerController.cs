using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GlitchEffectsDLL;

public class PlayerController : MonoBehaviour 
{

    public enum player_state
    {
        IN_GROUND,
		PREPARING_JUMP,
        JUMPING,
		FALL_RECOVERING,
        TELEPORTING
    };

    public player_state state;
    public Material brokenTexture;
	public int lives;
    public int items = 0;						// Items collected
	public float speed = 12.0f;					// Horizontal speed
	public float jumpSpeed = 13.5f;			
	public float gravity = 50.0f;				
	public float maxJumpTime = 0.33f;			// Max time a jump can be extended
	public float jumpRest = 0.025f;				// Time of jump preparing and fall recovery
    public float preJumpPosY = 0;
    public bool teleportCooldown = false;
    public Camera mainCamera;
    public Camera godCamera;
	public GUIlives guiLife;
    public GUICollects guiItem;
    public TeleportScript teleport;
    public World world;
    public SlowFPS slowFPS;
    public float vSpeed = 0.0f;

    private bool godMode = false;
	private float startJumpPress = -1;				//When the extended jump started
	private float preparingJump = 0;				//Jump preparing time left
	private float fallRecovery = 0;					//Fall recovery time left
	private Vector3 moveDirection = Vector3.zero;	//Direction of movement
	private SpriteRenderer spriteRenderer;			//Reference to the sprite renderer
	private int numBoxes = 0;
	private CharacterController controller;

    void OnControllerColliderHit(ControllerColliderHit coll)
    {
        if (!coll.gameObject.CompareTag("ErrorBox"))
        {
            if (coll.gameObject.CompareTag("Floor"))
            {
                TextureEffects.TextureFlicker(coll.gameObject, brokenTexture);
            }
            else
            {
                TextureEffects.TextureFlickerRepeat(coll.gameObject, brokenTexture);
            }
        }
    }

	void Start ()
	{
        lives = 3;
		controller = GetComponent<CharacterController>();
	    spriteRenderer = GetComponent<SpriteRenderer>();
		state = player_state.IN_GROUND;
	}

	void Update () 
	{
		
		Vector3 moveDirection = new Vector3 (0, 0, 0);

        // State-changing calculations
        switch (state)
        {
			case player_state.PREPARING_JUMP:

				preparingJump -= Time.deltaTime;

				//If it's ready to jump, start jump and give fall recovery time
				if (preparingJump <= 0)
				{
					vSpeed = jumpSpeed;
					startJumpPress = Time.time;
					fallRecovery = jumpRest;
					state = player_state.JUMPING;
				}
				break;

			case player_state.FALL_RECOVERING:

				fallRecovery -= Time.deltaTime;
				if (fallRecovery <= 0){
					state = player_state.IN_GROUND;
				}
				break;

			case player_state.IN_GROUND: 
			case player_state.JUMPING:

			//If it's teleporting
			if ((Input.GetKeyDown (KeyCode.L)) && (!teleportCooldown)) {
				
				// We create a coroutine to do a delay in the teleport and the state of player is changed to teleporting
				StartCoroutine ("ActivateTeleport");
				ActivateTeleport ();
				state = player_state.TELEPORTING;
				vSpeed = 0;

			} else {

				//If it's grounded
				if (controller.isGrounded) {
					if (teleportCooldown) {
						teleportCooldown = false;
					}
					
					if (state == player_state.JUMPING) {
						state = player_state.FALL_RECOVERING;
					} else if (state != player_state.IN_GROUND) {
						state = player_state.IN_GROUND;
					}
					
					if (state == player_state.IN_GROUND && Input.GetButtonDown ("Jump")) {
						preparingJump = jumpRest;
						state = player_state.PREPARING_JUMP;
                        preJumpPosY = transform.position.y + (transform.localScale.y * 4);
					} else {
						vSpeed = 0;
					}
				} else {
					//If it's in the air

					//If the player keeps pushing the jump button give a little
					//vSpeed momentum - that gets gradually smaller - to get a
					//higher jump. Do until the press time gets to his max.
					//If the player releases the button, stop giving extra momentum to the jump.
					if (startJumpPress != -1 && Input.GetButton ("Jump") && (Time.time - startJumpPress) <= maxJumpTime) {
						vSpeed = jumpSpeed;
					} else {
						startJumpPress = -1;
					}

				}
			}

			break;
        }

		//Non state-changing operations
		if (state != player_state.TELEPORTING) {
			
			// Gravity
			vSpeed -= gravity * Time.deltaTime;

			// Control of movemente in X axis
			moveDirection.x = Input.GetAxis ("Horizontal");
			moveDirection = transform.TransformDirection (moveDirection);
			moveDirection *= speed;

			// Flips the sprite renderer if is changing direction
			if (moveDirection.x > 0 && spriteRenderer.flipX == true) {
				spriteRenderer.flipX = false;
			} else if (moveDirection.x < 0 && spriteRenderer.flipX == false) {
				spriteRenderer.flipX = true;
			}

		}

		moveDirection.y = vSpeed;
		controller.Move(moveDirection * Time.deltaTime);

        if (Input.GetButtonDown("SlowFPS"))
        {
            print(world.slow);
            if (world.slow == false)
            {
                world.slow = true;
            }
            else
            {        
                world.slow = false;
            }
        }

        // To active God mode camera
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (godMode == true)
            {
                mainCamera.gameObject.SetActive(true);
                godCamera.gameObject.SetActive(false);
                godMode = false;
            }
            else
            {
                godCamera.gameObject.SetActive(true);
                mainCamera.gameObject.SetActive(false);
                godMode = true;
            }
        }

	}

	public void errorBoxDeleted (int num)
	{
		numBoxes -= num;
	}

    // Function that active teleport. Necessary to Coroutine work
    IEnumerator ActivateTeleport()
    {
		teleportCooldown = true;
        
        if (controller.isGrounded)
        {
            // Wait for 0.3 seconds
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            // Wait for 0.5 seconds
            yield return new WaitForSeconds(0.5f);
        }

		teleportCooldown = teleport.Teleport(controller);
		state = player_state.JUMPING;
    }
}