using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	public float speed = 12.0f;					//Horizontal speed
	public float jumpSpeed = 13.5f;				//Vertical speed
	public float gravity = 50.0f;				//Gravity
	public float maxJumpTime = 0.33f;			//Max time a jump can be extended
	public float jumpRest = 0.025f;				//Time of jump preparing and fall recovery

	private float vSpeed = 0.0f;				//Vertical Speed
	private float startJumpPress = -1;			//When the extended jump started
	private float preparingJump = 0;				//Jump preparing time left
	private float fallRecovery = 0;				//Fall recovery time left

	private Vector3 moveDirection;				//Direction of movement

	CharacterController controller;				//Reference to the controller
	SpriteRenderer spriteRenderer;				//Reference to the sprite renderer

	// Use this for initialization
	void Start ()
	{
		controller = GetComponent<CharacterController>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update ()
	{
		//Gets the movement direction
		moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, 0);

		//Flips the sprite renderer if is changing direction
		if (moveDirection.x > 0 && spriteRenderer.flipX == true) {
			spriteRenderer.flipX = false;
		} else if (moveDirection.x < 0 && spriteRenderer.flipX == false) {
			spriteRenderer.flipX = true;
		}

		moveDirection = transform.TransformDirection (moveDirection);
		moveDirection *= speed;

		//If it's recovering from a fall, don't allow to jump
		if (fallRecovery > 0 && controller.isGrounded) {
			fallRecovery -= Time.deltaTime;
		} 
			
		//If it's preparing a jump, wait
		else if (preparingJump > 0) {
			preparingJump -= Time.deltaTime;

			//If it's ready to jump, start jump and give fall recovery time
			if (preparingJump <= 0) {
				vSpeed = jumpSpeed;
				startJumpPress = Time.time;
				fallRecovery = jumpRest;
			}
				
		//If it's not waiting for any reason
		} else {

			//If it's grounded
			if (controller.isGrounded) {

				//If the button is pushed, start preparing the jump
				if (Input.GetButton ("Jump")) {
					preparingJump = jumpRest;
				} else {
					vSpeed = 0;
				}

			} else {

				//If it's in the air
				//If the player keeps pushing the jump button give a little
				//vSpeed momentum - that gets gradually smaller - to get a
				//higher jump. Do until the press time gets to his max.
				//If the player releases the button, stop giving extra momentum to the jump.
				if (Input.GetButton ("Jump") && startJumpPress != -1){
					if ((Time.time - startJumpPress) <= maxJumpTime) {
						vSpeed = jumpSpeed;
					} else {
						startJumpPress = -1;
					}
				}
			}

		}

		//Add gravity speed
		vSpeed -= gravity * Time.deltaTime;
		moveDirection.y = vSpeed;

		//Move the controller
		controller.Move (moveDirection * Time.deltaTime);
	}


	/*
	void OnControllerColliderHit(ControllerColliderHit coll){		
		if (coll.gameObject.CompareTag("Floor"))
		{
			//TextureEffects.TextureFlicker(coll.gameObject, brokenTexture);
		}
		else
		{
			//TextureEffects.TextureFlickerRepeat(coll.gameObject, brokenTexture);
		}
	}

	void OnCollsionExit(Collision coll)
   	{
		TextureEffects.TextureRemove(coll.gameObject, brokenTexture);
    }
    */
}