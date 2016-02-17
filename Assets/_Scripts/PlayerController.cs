﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	public float speed = 30.0f;
	public float jumpSpeed = 100.0f;
	public float gravity = 9.8f;

	private float vSpeed = 0.0f;
	private Vector3 moveDirection = Vector3.zero;
	private int numBoxes = 0;

	public GameObject errorBoxPrefab;
	CharacterController controller;

	// Use this for initialization
	void Start ()
	{
		controller = GetComponent<CharacterController> ();
	}

	// Update is called once per frame
	void Update ()
	{
		moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, 0);
		moveDirection = transform.TransformDirection (moveDirection);
		moveDirection *= speed;

		if (controller.isGrounded) {
			if (Input.GetButton ("Jump"))
				vSpeed = jumpSpeed;
			else
				vSpeed = 0;
		}

		vSpeed -= gravity * Time.deltaTime;
		moveDirection.y = vSpeed;
		controller.Move (moveDirection * Time.deltaTime);

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