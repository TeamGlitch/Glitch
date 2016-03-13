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
		WHIPING,
		DEATH
    };

	///////////// Variables /////////////

	//State
	public player_state state;
	private bool godMode = false;

	//Player Components
	public SpriteRenderer spriteRenderer;			//Reference to the sprite renderer
	public CharacterController controller;
	public Rigidbody rigidBody;

	//External references
	public Camera mainCamera;
	public Camera godCamera;

	//Movement Variables
	public float speed = 12.0f;					// Horizontal speed
	public float jumpSpeed = 13.5f;			
	public float gravity = 50.0f;				
	public float maxJumpTime = 0.33f;			// Max time a jump can be extended
	public float jumpRest = 0.025f;				// Time of jump preparing and fall recovery
    public float cameraYBound = 0;
    public float vSpeed = 0.0f;

	private float startJumpPress = -1;				//When the extended jump started
	private float preparingJump = 0;				//Jump preparing time left
	private float fallRecovery = 0;					//Fall recovery time left
    public float zPosition = 0.0f;

	///// Powers
	//Teleport
    private bool teleportCooldown = false;
	public TeleportScript teleport;

	//Slow FPS
	public SlowFPS slowFPS;

	//Whip
	public float whipForce = 5.0f;
	public float maxAngleWhipForce = 60.0f;
	
	///// Other
	//Broken effect
	public Material brokenTexture;


	///////////// Functions /////////////

	void Start ()
	{
		state = player_state.IN_GROUND;
	}

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

		if ((controller.collisionFlags & CollisionFlags.Above) != 0)
			vSpeed = 0;
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
				
				// If it's not teleporting
				if (!ActivatingTeleport())
				{
					teleportCooldown = false;

					if (InputManager.ActiveDevice.Action1.WasPressed) 
					{
						preparingJump = jumpRest;
						state = player_state.PREPARING_JUMP;
						cameraYBound = transform.position.y + (transform.localScale.y * 4);
					} 
					else if (!controller.isGrounded) 
					{
						state = player_state.JUMPING;
					}
					else 
					{
						vSpeed = 0;
					}
				}
				break;

			case player_state.JUMPING:

				// If it's not teleporting
				if (!ActivatingTeleport())
				{
					//If it's grounded
					if (controller.isGrounded) 
                    {
						state = player_state.FALL_RECOVERING;
					} 
					else 
					{
						//If it's in the air
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
						if ((startJumpPress != -1) && (InputManager.ActiveDevice.Action1.IsPressed)
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
				break;

			case player_state.WHIPING:
			
				if(gameObject.transform.rotation.eulerAngles.z > 360.0f-maxAngleWhipForce || 
					gameObject.transform.rotation.eulerAngles.z < maxAngleWhipForce)
				{
					float whipDirection = InputManager.ActiveDevice.LeftStickX.Value;

					if (whipDirection == 1.0f)
					{
						rigidBody.AddForce (new Vector3 (whipForce, 0.0f, 0.0f));
					} 
					else if (whipDirection == -1.0f)
					{
						rigidBody.AddForce (new Vector3 (-whipForce, 0.0f, 0.0f));
					}
				}
				break;
        }

		//Non state-changing operations
		if (state != player_state.DEATH &&
			state != player_state.TELEPORTING &&
			state != player_state.WHIPING) 
        {	
			// Gravity
			vSpeed -= gravity * Time.deltaTime;

			// Control of movemente in X axis
			moveDirection.x = InputManager.ActiveDevice.LeftStickX.Value;
			moveDirection = transform.TransformDirection (moveDirection);
			moveDirection *= speed;

			// Flips the sprite renderer if is changing direction
			if ((moveDirection.x > 0) && (spriteRenderer.flipX == true)) 
            {
				spriteRenderer.flipX = false;
			} else if ((moveDirection.x < 0) && (spriteRenderer.flipX == false)) {
				spriteRenderer.flipX = true;
			}
			moveDirection.y = vSpeed;

			controller.Move(moveDirection * Time.deltaTime);

			if (transform.position.z != zPosition)
			{
				Vector3 pos = transform.position;
				pos.z = zPosition;
				transform.position = pos;
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

	private bool ActivatingTeleport(){

		if (InputManager.ActiveDevice.Action3.WasPressed && (!teleportCooldown)) 
        {	
			// We create a coroutine to do a delay in the teleport and the state of player is changed to teleporting
			StartCoroutine ("ActivateTeleport");
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

	public void StartWhip()
	{
		state = player_state.WHIPING;
		vSpeed = 0;
		rigidBody.isKinematic = false;
	}

	public void EndWhip()
	{
		startJumpPress = Time.time;
		state = player_state.JUMPING;
		vSpeed = jumpSpeed;
		rigidBody.isKinematic = true;
	}

}