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

    public player_state state;
    public Material brokenTexture;
	public float speed = 30.0f;
	public float jumpSpeed = 100.0f;
	public float gravity = 9.8f;
    public bool coolDown = false;
    public GameObject errorBoxPrefab;
    public World world;

	private float vSpeed = 0.0f;
	private Vector3 moveDirection = Vector3.zero;
	private int numBoxes = 0;
    private SlowFPS slowFPS;
    private TeleportScript teleport;
	private CharacterController controller;

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
        teleport = GetComponent<TeleportScript>();
        slowFPS = GetComponent<SlowFPS>();
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
            case player_state.FALLING:
                // Control of movemente in X axis
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= speed;

                // Control of movemente in Y axis
                vSpeed -= gravity * Time.deltaTime;
                moveDirection.y = vSpeed;
                controller.Move(moveDirection * Time.deltaTime);
                break;
        }

		if (controller.isGrounded){ 
            coolDown = false;
            state = player_state.IN_GROUND;
            
			if (Input.GetButtonDown("Jump")){
				vSpeed = jumpSpeed;
                state = player_state.JUMPING;
			}
		}

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
		
		if ((Input.GetKeyDown(KeyCode.L)) && (coolDown == false))
        {
            // We create a coroutine to do a delay in the teleport and the state of player is changed to teleporting
            StartCoroutine("ActivateTeleport");
            ActivateTeleport();
            state = player_state.TELEPORTING;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (world.slow == false)
            {
                world.slow = true;
            }
            else
            {        
                world.slow = false;
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
        coolDown = true;
        // Wait for 0.2 seconds
        if (!controller.isGrounded)
        {
            yield return new WaitForSeconds(0.2f);
        }

        // With this after teleport not continues go up in a jumping
        vSpeed = 0.0f;
        moveDirection.x = 0.0f;

        coolDown = teleport.Teleport(controller);
        state = player_state.FALLING;
    }
}