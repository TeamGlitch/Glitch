using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour 
{

    public enum player_state
    {
        IDLE,
        JUMP
    };

	public Sprite errorBox;
	public float speed = 30.0f;
	public float jumpSpeed = 100.0f;
	public float gravity = 9.8f;
	public float scaleDesiredX = 1.0f;
	public float scaleDesiredY = 1.0f;
	public float scaleBoxColliderX = 1.0f;
	public float scaleBoxColliderY = 1.0f;
	public float scaleBoxColliderZ = 1.0f;
    public player_state state;
    static public bool coolDown = false;

	private float vSpeed = 0.0f;
	private Vector3 moveDirection = Vector3.zero;
	private int numBoxes = 3;

	GameObject errorBox1;
	GameObject errorBox2;
	GameObject errorBox3;

	// Use this for initialization
	void Start ()
	{
		errorBox1 = GameObject.Find ("ErrorBox1");
		errorBox2 = GameObject.Find ("ErrorBox2");
		errorBox3 = GameObject.Find ("ErrorBox3");
        state = player_state.IDLE;
	}
	
	// Update is called once per frame
	void Update () 
	{
        CharacterController controller = GetComponent<CharacterController>();

		moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, 0);
		moveDirection = transform.TransformDirection (moveDirection);
		moveDirection *= speed;

		if (controller.isGrounded) 
		{
            coolDown = false;
            state = player_state.IDLE;
			if (Input.GetButton ("Jump")) 
			{
				vSpeed = jumpSpeed;
                state = player_state.JUMP;
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

			if (!errorBox1.GetComponent<ErrorBoxScript> ().boxActive && errorBox1.GetComponent<ErrorBoxScript> ().canBeActivated) {
				errorBox1.GetComponent<ErrorBoxScript> ().ActivateBox (new Vector3(mouse.x, mouse.y, 0));
			}
			else if (!errorBox2.GetComponent<ErrorBoxScript> ().boxActive && errorBox2.GetComponent<ErrorBoxScript> ().canBeActivated) {
				errorBox2.GetComponent<ErrorBoxScript> ().ActivateBox (new Vector3(mouse.x, mouse.y, 0));
			}
			else if (!errorBox3.GetComponent<ErrorBoxScript> ().boxActive && errorBox3.GetComponent<ErrorBoxScript> ().canBeActivated) {
				errorBox3.GetComponent<ErrorBoxScript> ().ActivateBox (new Vector3(mouse.x, mouse.y, 0));
			}

		}

        if ((Input.GetKeyDown(KeyCode.L)) && (coolDown == false))
        {
            if (state == player_state.JUMP)
            {
                coolDown = true;
            }

            int direction = 1;
            if (Input.GetKey(KeyCode.A))
            {
                direction = -1;
            }
            GetComponent<TeleportScript>().RightTeleport(direction);
        }

	}
}