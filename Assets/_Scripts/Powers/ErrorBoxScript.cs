using UnityEngine;
using System.Collections;
using InControl;

public class ErrorBoxScript : MonoBehaviour {

    public enum box_state
    {
        OFF,
        CREATING,
        ON,
        COOLDOWN
    }

    public box_state state;
    public BoxCreatorUI boxCreatorUI;					//UI
    public ErrorBoxScript backErrorBox;
    public ErrorBoxScript nextErrorBox;
	public float startTime = -1;
	public float duration;
	public float cooldown = -1;
    public World world;
    public SpriteRenderer spriteRenderer;
    public BoxCollider boxCollider;

    private bool previsualization = false;				// Currently using the placeholder
    private float restartOriginalPosition = -1;			//If its later than this, restart the placeholder position
	private float nextFlicker;


	void Start()
    {
        state = box_state.OFF;
	}

	void Update () 
    {
        switch (state)
        {
            case box_state.OFF:
                //Moving box to know where it will be put down
                if (InputManager.ActiveDevice.RightTrigger.IsPressed && !InputManager.ActiveDevice.LeftTrigger.IsPressed)
                {
                    if (world.numBoxes < 3)
                    {
                        //If the previsualization hasn't started, activate it
                        if (!previsualization)
                        {
                            previsualization = true;

                            //If enough time has passed since the last box creation, restart the box creation
                            //position to the center of the screen
                            if (Time.time > restartOriginalPosition)
                            {
                                Vector3 position = new Vector3(0, 0, transform.position.z);
                                position = Camera.main.WorldToScreenPoint(position);
                                position.x = Screen.width / 2;
                                position.y = Screen.height / 2;
                                transform.position = Camera.main.ScreenToWorldPoint(position);
                            }
                            state = box_state.CREATING;
                            spriteRenderer.enabled = true;
                            spriteRenderer.color = new Color(1.0F, 1.0F, 1.0F, 0.4F);
                            ++world.numBoxes;
                        }
                    }
                } 
                break;

            case box_state.CREATING:

                //Calculate position and move placeholder
                transform.position = calculateErrorBoxPosition();

                if (InputManager.ActiveDevice.RightTrigger.WasReleased && !InputManager.ActiveDevice.LeftTrigger.IsPressed)
                {
                    //Send properties
                    startTime = Time.time;
                    cooldown = 3;
                    duration = 5;
                    boxCollider.enabled = true;
                    spriteRenderer.color = new Color(1, 1, 1, 1);
                    nextFlicker = -1;

                    //Notify the GUI
                    // boxCreatorUI.boxUsed(boxes.indexOf(errorBox), Time.time + cooldown);

                    // Set the restart position time
                    restartOriginalPosition = Time.time + 10.0f;

                    state = box_state.ON;
                    previsualization = false;
                }
                break;

            case box_state.ON:

                // Set first ficker if not stablished
                if (nextFlicker == -1)
                {
                    nextFlicker = Time.time + (duration - 1.0f);
                }

                // If it's flicker time
                if (Time.time >= nextFlicker)
                {

                    // Change alpha
                    if (spriteRenderer.color.a == 1)
                    {
                        spriteRenderer.color = new Color(1, 1, 1, 0.6f);
                    }
                    else
                    {
                        spriteRenderer.color = new Color(1, 1, 1, 1);
                    }

                    nextFlicker = Time.time + 0.15f;
                }

                duration -= Time.deltaTime;
                // If it's time over, remove renderer and collider
                if (duration <= 0)
                {
                    spriteRenderer.enabled = false;
                    boxCollider.enabled = false;
                    state = box_state.COOLDOWN;
                }

                if (!nextErrorBox.gameObject.activeSelf)
                {
                    nextErrorBox.gameObject.SetActive(true);
                }
                break;

            case box_state.COOLDOWN:
                cooldown -= Time.deltaTime;
                // If cooldown is over, disable the box
                if (cooldown <= 0)
                {
                    --world.numBoxes;
                    if ((nextErrorBox.state != box_state.COOLDOWN) || (backErrorBox.state != box_state.COOLDOWN))
                    {
                        state = box_state.OFF;
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        state = box_state.OFF;
                    }
                }
                break;
        }
	}

    //Calcule box position
    private Vector3 calculateErrorBoxPosition()
    {
        Vector3 mouse;

        if (InputManager.ActiveDevice.Name != "Keyboard/Mouse")
        {

            //If using a controller, move the placeholder with the right joystick
            float posx = transform.position.x + ((50.0f * Time.deltaTime) * InputManager.ActiveDevice.RightStickX);
            float posy = transform.position.y + ((50.0f * Time.deltaTime) * InputManager.ActiveDevice.RightStickY);

            //Get to screen coordinates and make sure it isn't out of the screen
            mouse = new Vector3(posx, posy, transform.position.z);
            mouse = Camera.main.WorldToScreenPoint(mouse);

            if (mouse.x < 0)
            {
                mouse.x = 0;
            }
            if (mouse.x > Screen.width)
            {
                mouse.x = Screen.width;
            }
            if (mouse.y < 0)
            {
                mouse.y = 0;
            }
            if (mouse.y > Screen.height)
            {
                mouse.y = Screen.height;
            }

            mouse = Camera.main.ScreenToWorldPoint(mouse);
        }
        else
        {
            // If using a mouse, just get the mouse position and transform to world coordinates
            mouse = Input.mousePosition;
            mouse.z = transform.position.z - Camera.main.transform.position.z;
            mouse = Camera.main.ScreenToWorldPoint(mouse);
        }

        return mouse;

    }
}
