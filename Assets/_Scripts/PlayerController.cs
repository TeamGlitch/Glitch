using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	public float speed = 30.0f;
	public float jumpSpeed = 100.0f;
	public float gravity = 9.8f;
	public int maxJumpPress = 20;

	private float vSpeed = 0.0f;
	private int jumpPressTime = 0;
	private Vector3 moveDirection;

	private int numBoxes = 0;

	public GameObject errorBoxPrefab;
	CharacterController controller;
	SpriteRenderer spriteRenderer;

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

		//If it's grounded
		if (controller.isGrounded) {

			//If the button is pushed, start jump
			if (Input.GetButton ("Jump")) {
				vSpeed = jumpSpeed;
				jumpPressTime = 0;
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

		//Add gravity speed
		vSpeed -= gravity * Time.deltaTime;
		moveDirection.y = vSpeed;

		//Move the controller
		controller.Move (moveDirection * Time.deltaTime);

		//Errorbox creation
		if (Input.GetMouseButtonDown (0)) {
			if (numBoxes < 3) {
				Vector3 mouse = Input.mousePosition;
				mouse.z = 15;
				mouse = Camera.main.ScreenToWorldPoint (mouse);

				GameObject errorBox = (GameObject)Instantiate (errorBoxPrefab);
				errorBox.transform.position = new Vector3 (mouse.x, mouse.y, 0);
				errorBox.GetComponent<ErrorBoxScript> ().duration = 500;
				errorBox.GetComponent<ErrorBoxScript> ().player = this;
				numBoxes++;
			}
		}
	}

	//Announce that a error box was deleted
	public void errorBoxDeleted (int num)
	{
		numBoxes -= num;
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