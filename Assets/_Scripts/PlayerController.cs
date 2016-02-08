using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour 
{

	public Sprite errorBox;
	public float speed = 30.0f;
	public float jumpSpeed = 100.0f;
	public float gravity = 9.8f;
	public float scaleDesiredX = 1.0f;
	public float scaleDesiredY = 1.0f;
	public float scaleBoxColliderX = 1.0f;
	public float scaleBoxColliderY = 1.0f;
	public float scaleBoxColliderZ = 1.0f;

	private float vSpeed = 0.0f;
	private Vector3 moveDirection = Vector3.zero;
	private int numBoxes = 3;
	private float timeBoxes = 5.0f;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () 
	{

		CharacterController controller = GetComponent<CharacterController> ();

		moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, 0);
		moveDirection = transform.TransformDirection (moveDirection);
		moveDirection *= speed;

		if (controller.isGrounded) 
		{
			if (Input.GetButton ("Jump")) 
			{
				vSpeed = jumpSpeed;
			}
			else 
			{
				vSpeed = 0;
			}
		}

		vSpeed -= gravity * Time.deltaTime;
		moveDirection.y = vSpeed;
		controller.Move (moveDirection * Time.deltaTime);

		if (Input.GetMouseButtonDown (0)) 
		{
			Vector3 mouse = Input.mousePosition;
			mouse.z = 15;
			mouse = Camera.main.ScreenToWorldPoint(mouse);

			GameObject errorBoxObject = new GameObject("ErrorBox");
			errorBoxObject.transform.position =  new Vector3(mouse.x, mouse.y, 0);
			errorBoxObject.AddComponent<BoxCollider> ();

			BoxCollider bx = errorBoxObject.GetComponent<BoxCollider> ();
			bx.size = new Vector3 (bx.size.x * scaleBoxColliderX, bx.size.y * scaleBoxColliderY, bx.size.z);
			errorBoxObject.transform.localScale = new Vector3(scaleDesiredX, scaleDesiredY, 1.0f);
			SpriteRenderer renderer = errorBoxObject.AddComponent<SpriteRenderer>();
			renderer.sprite = errorBox;
		}

	}
		
}