using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GlitchEffectsDLL;

public class PlayerController : MonoBehaviour 
{

    public enum player_state
    {
        IN_GROUND,
        JUMPING,
        FALLING,
        TELEPORTING
    };

    public Material brokenTexture;
	public Sprite errorBox;
	public float speed = 30.0f;
	public float jumpSpeed = 100.0f;
	public float gravity = 9.8f;
    public player_state state;
    static public bool coolDown = false;

	private float vSpeed = 0.0f;
	private Vector3 moveDirection = Vector3.zero;
	private int numBoxes = 0;

	public GameObject errorBoxPrefab;
	CharacterController controller;

    void OnControllerColliderHit(ControllerColliderHit coll)
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

	// Use this for initialization
	void Start ()
	{
		controller = GetComponent<CharacterController> ();
        state = player_state.IN_GROUND;
	}

	// Update is called once per frame
	void Update () 
	{
        // State machine for player control depending on state
        switch (state)
        {
            case player_state.IN_GROUND: 
            case player_state.JUMPING:
                // Control of movemente in X axis
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= speed;

                // Control of movemente in Y axis
                vSpeed -= gravity * Time.deltaTime;
                moveDirection.y = vSpeed;
                controller.Move(moveDirection * Time.deltaTime);
                break;

            case player_state.FALLING:
                // Control of movemente in Y axis
                vSpeed -= gravity * Time.deltaTime;
                moveDirection.y = vSpeed;
                controller.Move(moveDirection * Time.deltaTime);
                break;
        }

		if (controller.isGrounded) 
            coolDown = false;
            state = player_state.IN_GROUND;
            
            // This is for know if the player input is "W" or "S"
			if (Input.GetButtonDown("Jump"))
				vSpeed = jumpSpeed;
                state = player_state.JUMPING;
			}
		}

		if (Input.GetMouseButtonDown (0)) 
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

        if ((Input.GetKeyDown(KeyCode.L)) && (coolDown == false))
        {
            // We create a coroutine to do a delay in the teleport and the state of player is changed to teleporting
            StartCoroutine("ActivateTeleport");
            ActivateTeleport();
            state = player_state.TELEPORTING;
        }

	}

    // Function that active teleport. Necessary to Coroutine work
    IEnumerator ActivateTeleport()
    {
        // If player is Jumping, we activate the cooldown
        if (state == player_state.JUMPING)
        {
            coolDown = true;
        }

        // Wait for 0.2 seconds
        yield return new WaitForSeconds(0.2f); 

        // With this after teleport not continues go up in a jumping
        vSpeed = 0.0f;
        moveDirection.x = 0.0f;
        state = GetComponent<TeleportScript>().Teleport();
    }
}