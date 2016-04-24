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

	///////////// Variables /////////////

	//State
	public player_state state;
	public bool allowMovement;
	private bool godMode = false;

	//Player Components
	public SpriteRenderer spriteRenderer;			//Reference to the sprite renderer
	private Animator plAnimation;
	public CharacterController controller;
	public Rigidbody rigidBody;

	//External references
	public Camera mainCamera;
	public Camera godCamera;
    public Camera staticCamera1;
    public Camera staticCamera2;
    public Camera staticCamera3;
    public Camera staticCamera4;

	//Movement Variables

	private bool playerActivedJump = false;		// The jump state is cause of a player jump? (If not, it could be a fall)

	private float zPosition;					// Position on the z axis. Unvariable
	public float speed = 7.2f;					// Horizontal speed
	public float jumpSpeed = 8.0f;				// Base jump speed
	public float gravity = 22.0f;				// Gravity
	public float maxJumpTime = 0.33f;			// Max time a jump can be extended
	public float jumpRest = 0.025f;				// Time of jump preparing and fall recovery
    public float vSpeed = 0.0f;					// The vertical speed

	private float startJumpPress = -1;				//When the extended jump started
	private float preparingJump = 0;				//Jump preparing time left
	private float fallRecovery = 0;					//Fall recovery time left
	private int nonGroundedFrames = 0;				// How many frames the player has being on air.

	///// Powers
	//Teleport
	public TeleportScript teleport;

	//Slow FPS
	public SlowFPS slowFPS;
	
	///// Other
	//Broken effect
	public Material brokenTexture;


	///////////// Functions /////////////

	void Start ()
	{
		plAnimation = GetComponent<Animator>();

		zPosition = transform.position.z;
		state = player_state.IN_GROUND;
		allowMovement = true;
	}

   void OnControllerColliderHit(ControllerColliderHit coll)
    {
		/*
		//   /\     /~~  /\  |\  /||~~\~|~  /\  |~~\|||
		//  /__\   |    /__\ | \/ ||--< |  /__\ |__/|||
		// /    \   \__/    \|    ||__/_|_/    \|  \...
		if (coll.gameObject.GetComponent<MeshRenderer>() != null)
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
		*/
		if ((controller.collisionFlags & CollisionFlags.Above) != 0) {
			vSpeed = -0;
			startJumpPress = -1;
		}
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
					plAnimation.SetBool ("Jump", true);	
					plAnimation.SetBool ("Run", false);
				}

                // To control movement of player
                Movement(moveDirection);
				break;

			case player_state.FALL_RECOVERING:
			
				fallRecovery -= Time.deltaTime;

				if (fallRecovery <= 0){
					state = player_state.IN_GROUND;
				}

                // To control movement of player
                Movement(moveDirection);
				break;

			case player_state.IN_GROUND: 
				
				// If it's not teleporting
				if (!ActivatingTeleport())
				{
					teleport.teleportUsed = false;

					//If the jump key is being pressed but it has been released since the
					//last jump
					if (InputManager.ActiveDevice.Action1.IsPressed && allowMovement && !playerActivedJump) 
					{
						//Start jump and set the player-activated jump to true so it
						//can't jump without releasing the button
						//We also assign 3 to nonGroundedFrames so the walking animation doesn't show
						preparingJump = jumpRest;
						playerActivedJump = true;
						nonGroundedFrames = 3;
						state = player_state.PREPARING_JUMP;
					} 
					else if (!controller.isGrounded) 
					{
						state = player_state.JUMPING;
						plAnimation.SetBool ("Falling", true);
						plAnimation.SetBool ("Run", false);
					}
					else 
					{
						vSpeed = 0;
					}
				}

                // To control movement of player
                Movement(moveDirection);
				break;

			case player_state.JUMPING:

				if (vSpeed < 0 && plAnimation.GetBool ("Falling") == false) {
					plAnimation.SetBool ("Jump", false);
					plAnimation.SetBool ("Falling", true);
				}

				// If it's not teleporting
				if (!ActivatingTeleport())
				{
					//If it's grounded
					if (controller.isGrounded) 
                    {
						//Start fall recovering and set the bools
						state = player_state.FALL_RECOVERING;
						plAnimation.SetBool ("Falling", false);
						if (plAnimation.GetBool ("Jump") == true) 
						{
 							plAnimation.SetBool ("Jump", false);
 						}
 						nonGroundedFrames = 0;
					} 
					else 
					{
						//If it's in the air
						nonGroundedFrames++;
						Vector3 eulerAngles = gameObject.transform.rotation.eulerAngles;
						float rotationZ = 0.0f;

						if (eulerAngles.z < 0.0f) {
							eulerAngles.z += 360.0f;
						}

						if (eulerAngles.z != 0.0f) {
							if (eulerAngles.z <= 3.0f || eulerAngles.z >= 357.0f) 
                            {
								rotationZ = 0.0f;
							}
							else if (eulerAngles.z <= 180.0f) {
								rotationZ = eulerAngles.z - 3.0f;
							} else if (eulerAngles.z > 180.0f) {
								rotationZ = eulerAngles.z + 3.0f;
							}
							gameObject.transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, rotationZ);
						}

						//If the player keeps pushing the jump button give a little
						//vSpeed momentum - that gets gradually smaller - to get a
						//higher jump. Do until the press time gets to his max.
						//If the player releases the button, stop giving extra momentum to the jump.
						if ((startJumpPress != -1) && (InputManager.ActiveDevice.Action1.IsPressed) & allowMovement
							&& ((Time.time - startJumpPress) <= maxJumpTime)) 
                        {
							vSpeed = jumpSpeed;
						} 
                        else 
                        {
							startJumpPress = -1;
						}
					}
				}

                // To control movement of player
                Movement(moveDirection);
				break;
        }

		//If a player-induced jump is checked but the jump key is not longer
		//being held, set it to false so it can jump again
		if (playerActivedJump && !InputManager.ActiveDevice.Action1.IsPressed && allowMovement)
			playerActivedJump = false;

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

        // To active a static camera
        if (mainCamera.isActiveAndEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                staticCamera1.gameObject.SetActive(true);
                mainCamera.gameObject.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                staticCamera2.gameObject.SetActive(true);
                mainCamera.gameObject.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                staticCamera3.gameObject.SetActive(true);
                mainCamera.gameObject.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                staticCamera4.gameObject.SetActive(true);
                mainCamera.gameObject.SetActive(false);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                mainCamera.gameObject.SetActive(true);
                staticCamera1.gameObject.SetActive(false);
                staticCamera2.gameObject.SetActive(false);
                staticCamera3.gameObject.SetActive(false);
                staticCamera4.gameObject.SetActive(false);
            }
        }

	}

    private void Movement(Vector3 moveDirection)
    {
        // Gravity
        vSpeed -= gravity * Time.deltaTime;

		//If the player is allowed to move
		if (allowMovement) {
			
			// Control of movemente in X axis
			moveDirection.x = InputManager.ActiveDevice.LeftStickX.Value;
			moveDirection = transform.TransformDirection (moveDirection);
			moveDirection *= speed;

			// Flips the sprite renderer if is changing direction
			if ((moveDirection.x > 0) && (spriteRenderer.flipX == true)) {
				spriteRenderer.flipX = false;
			} else if ((moveDirection.x < 0) && (spriteRenderer.flipX == false)) {
				spriteRenderer.flipX = true;
			}

		} else {
			moveDirection.x = 0;
		}

        moveDirection.y = vSpeed;

        controller.Move(moveDirection * Time.deltaTime);
        if (transform.position.z != zPosition)
        {
            Vector3 pos = transform.position;
            pos.z = zPosition;
            transform.position = pos;
        }

		//Play or stop the run animation if it's on ground or the character
 		//is in a minor fall. The nonGroundedFrames point out how many frames the
 		//character has been non-grounded, so the idle/falling animation doesn't
 		//play on minor falls and slopes.
 		//TODO: Maybe change to time?
 		if ((state == player_state.IN_GROUND) || (nonGroundedFrames < 3)) 
		{
			if(moveDirection.x != 0){
				if (plAnimation.GetBool("Run") == false) {
					plAnimation.SetBool("Run", true);
				}
			} else if(plAnimation.GetBool("Run") == true){
				plAnimation.SetBool("Run",false);
			}
		}
    }

	private bool ActivatingTeleport(){

		if (InputManager.ActiveDevice.Action3.WasPressed && allowMovement
			&& (!teleport.teleportUsed) && teleport.CheckTeleport(controller))
		{
                // We create a coroutine to do a delay in the teleport and the state of player is changed to teleporting
                StartCoroutine("ActivateTeleport");
                ActivateTeleport();
                state = player_state.TELEPORTING;
                vSpeed = 0;

                return true;
		}
		return false;
	}

    // Function that active teleport. Necessary to Coroutine work
    IEnumerator ActivateTeleport()
    {
		teleport.teleportUsed = true;
        
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

		teleport.teleportUsed = teleport.Teleport(controller);
		state = player_state.JUMPING;
    }

}