using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	public float speed = 30.0f;					//Horizontal speed
	public float jumpSpeed = 100.0f;			//Vertical speed
	public float gravity = 9.8f;				//Gravity
	public int maxJumpPress = 20;				//Max Updates a jump can be extended
	public int jumpRest = 4;					//Time of jump preparing and fall recovery

	private float vSpeed = 0.0f;				//Vertical Speed
	private int jumpPressTime = 0;				//Jump extended time
	private int preparingJump = 0;				//Jump preparing time left
	private int fallRecovery = 0;				//Fall recovery time left

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
			fallRecovery--;
		} 
		//If it's preparing a jump, wait
		else if (preparingJump > 0) {
			preparingJump--;

			//If it's ready to jump, start jump and give fall recovery time
			if (preparingJump == 0) {
				vSpeed = jumpSpeed;
				jumpPressTime = 0;
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
				if (Input.GetButton ("Jump") && jumpPressTime < maxJumpPress) {
					vSpeed = jumpSpeed * (1 - (jumpPressTime/maxJumpPress));
					jumpPressTime++;
				} else {
					jumpPressTime = maxJumpPress;
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