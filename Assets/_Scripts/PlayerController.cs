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
	public int lifes;
    public int items = 0;						// Items collected
	public float speed = 12.0f;					// Horizontal speed
	public float jumpSpeed = 13.5f;			
	public float gravity = 50.0f;				
	public float maxJumpTime = 0.33f;			// Max time a jump can be extended
	public float jumpRest = 0.025f;				// Time of jump preparing and fall recovery
    public bool coolDown = false;
	public GUILifes guiLife;
    public GUICollects guiItem;
    public TeleportScript teleport;
    public World world;

	private float startJumpPress = -1;			//When the extended jump started
	private float preparingJump = 0;				//Jump preparing time left
	private float fallRecovery = 0;				//Fall recovery time left
	private Vector3 moveDirection = Vector3.zero;				//Direction of movement
	private SpriteRenderer spriteRenderer;				//Reference to the sprite renderer
	private float vSpeed = 0.0f;
	private int numBoxes = 0;
    private SlowFPS slowFPS;
	private CharacterController controller;

    void OnControllerColliderHit(ControllerColliderHit coll)
    {
        if (coll.gameObject.CompareTag("Floor"))
        {
            TextureEffects.TextureFlicker(coll.gameObject, brokenTexture);
        }else{
            TextureEffects.TextureFlickerRepeat(coll.gameObject, brokenTexture);
        }
    }

	// Use this for initialization
	void Start ()
	{
        lifes = 3;
		controller = GetComponent<CharacterController> ();
        slowFPS = GetComponent<SlowFPS>();
	    spriteRenderer = GetComponent<SpriteRenderer>();
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

		        // Flips the sprite renderer if is changing direction
		        if (moveDirection.x > 0 && spriteRenderer.flipX == true) 
                {
			        spriteRenderer.flipX = false;
		        }else{
                    if (moveDirection.x < 0 && spriteRenderer.flipX == false) 
                    {
			            spriteRenderer.flipX = true;
                    }
		        }

		        //If it's recovering from a fall, don't allow to jump
                if (fallRecovery > 0 && controller.isGrounded)
                {
                    fallRecovery -= Time.deltaTime;
                }
                else
                {
                    //If it's preparing a jump, wait
                    if (preparingJump > 0)
                    {
                        preparingJump -= Time.deltaTime;

                        //If it's ready to jump, start jump and give fall recovery time
                        if (preparingJump <= 0)
                        {
                            vSpeed = jumpSpeed;
                            startJumpPress = Time.time;
                            fallRecovery = jumpRest;
                        }

                    }
                    else
                    {
                        //If it's not waiting for any reason
                        if (controller.isGrounded)
                        {
                            coolDown = false;
                            state = player_state.IN_GROUND;

                            if (Input.GetButtonDown("Jump"))
                            {
                                preparingJump = jumpRest;
                                state = player_state.JUMPING;
                            }
                            else
                            {
                                vSpeed = 0;
                            }
                        }
                        else
                        {
                            //If it's in the air
                            //If the player keeps pushing the jump button give a little
                            //vSpeed momentum - that gets gradually smaller - to get a
                            //higher jump. Do until the press time gets to his max.
                            //If the player releases the button, stop giving extra momentum to the jump.
                            if (startJumpPress != -1 && Input.GetButton("Jump") && (Time.time - startJumpPress) <= maxJumpTime)
                            {
                                vSpeed = jumpSpeed;
                            }
                            else
                            {
                                startJumpPress = -1;
                            }
                        }
                    }
                }

                if ((Input.GetKeyDown(KeyCode.L)) && (coolDown == false))
                {
                    // We create a coroutine to do a delay in the teleport and the state of player is changed to teleporting
                    StartCoroutine("ActivateTeleport");
                    ActivateTeleport();
                    state = player_state.TELEPORTING;
                }
                // Control of movemente in Y axis
                vSpeed -= gravity * Time.deltaTime;
                moveDirection.y = vSpeed;
                controller.Move(moveDirection * Time.deltaTime);
                break;

                // In teleporting the player can't move and physics didn't have any action to player
            case player_state.TELEPORTING:
                vSpeed = 0.0f;
                moveDirection = new Vector3(0, 0, 0);
                break;
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
        if (controller.isGrounded)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        coolDown = teleport.Teleport(controller);
        state = player_state.FALLING;
    }
}