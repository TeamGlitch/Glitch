using UnityEngine;
using System.Collections;
using InControl;

public class ErrorBoxCreator : MonoBehaviour {

	public GameObject errorBoxPrefab;					//Prefab of the error box
	public BoxCreatorUI boxCreatorUI;					//UI

	private ObjectPool boxes;							//Boxes pool
	private GameObject placeholder;						//Placeholder that shows where the box will appear

	public float duration = 5;							//How many time a platform is active
	public float cooldown = 16;							//How many time until a new platform can be created

	private int numBoxes = 0;							//How many active boxes are present
	private bool previsualization = false;				//Currently using the placeholder

	private float restartOriginalPosition = -1;			//If its later than this, restart the placeholder position

	void Start(){
		
		boxes = new ObjectPool(errorBoxPrefab);

		//Initialize the placeholder
		placeholder = (GameObject)Instantiate(errorBoxPrefab);
		Destroy (placeholder.GetComponent<ErrorBoxScript>());
		Destroy (placeholder.GetComponent<BoxCollider>());
		placeholder.GetComponent<SpriteRenderer>().color = new Color(1.0F, 1.0F, 1.0F, 0.4F);
		placeholder.SetActive (false);
	}

	// Update is called once per frame
	void Update () {

		//Moving placeholder to know where it will be put down
		if (InputManager.ActiveDevice.RightTrigger.IsPressed && !InputManager.ActiveDevice.LeftTrigger.IsPressed) {
			if (numBoxes < 3) {

				//If the previsualization hasn't started, activate it
				if (!previsualization) {
					previsualization = true;
					placeholder.SetActive (true);

					//If enough time has passed since the last box creation, restart the box creation
					//position to the center of the screen
					if (Time.time > restartOriginalPosition) {
						Vector3 position = new Vector3 (0, 0, transform.position.z);
						position = Camera.main.WorldToScreenPoint (position);
						position.x = Screen.width / 2;
						position.y = Screen.height / 2;
						placeholder.transform.position = Camera.main.ScreenToWorldPoint(position);
					}

				}

				//Calculate position and move placeholder
				placeholder.transform.position = calculateErrorBoxPosition();

			}
		} 
		//Creating the box
		else if (InputManager.ActiveDevice.RightTrigger.WasReleased && !InputManager.ActiveDevice.LeftTrigger.IsPressed) {
			if (numBoxes < 3) {

				//Create an error box and place it
				GameObject errorBox = boxes.getObject();
				errorBox.transform.position = calculateErrorBoxPosition();

				//Send properties
				ErrorBoxScript EBScript = errorBox.GetComponent<ErrorBoxScript>(); 
				EBScript.errorBoxCreator = this;
				EBScript.startTime = Time.time;
				EBScript.duration = duration;
				EBScript.cooldown = cooldown;

				//Notify the GUI
				boxCreatorUI.boxUsed(boxes.indexOf(errorBox), Time.time + cooldown);

				//Increase number of boxes active and set the restart position time
				numBoxes++;
				restartOriginalPosition = Time.time + 10.0f;
			}

		}
		//Hiding the placeholder
		else if (previsualization) {
			previsualization = false;
			placeholder.SetActive (false);
		}
	}

	//Funcion for boxes to announce they have been deleted
	public void errorBoxDeleted (int num)
	{
		numBoxes -= num;
	}

	//Calcule box position
	private Vector3 calculateErrorBoxPosition(){

		Vector3 mouse;
		if (InputManager.ActiveDevice.Name != "Keyboard/Mouse") {

			//If using a controller, move the placeholder with the right joystick
			float posx = placeholder.transform.position.x + ((50.0f * Time.deltaTime) * InputManager.ActiveDevice.RightStickX);
			float posy = placeholder.transform.position.y + ((50.0f * Time.deltaTime) * InputManager.ActiveDevice.RightStickY);

			//Get to screen coordinates and make sure it isn't out of the screen
			mouse = new Vector3 (posx, posy, transform.position.z);
			mouse = Camera.main.WorldToScreenPoint(mouse);

			if (mouse.x < 0) mouse.x = 0;
			if (mouse.x > Screen.width) mouse.x = Screen.width;
			if (mouse.y < 0) mouse.y = 0;
			if (mouse.y > Screen.height) mouse.y = Screen.height;

			mouse = Camera.main.ScreenToWorldPoint(mouse);

		} else {

			//If using a mouse, just get the mouse position and transform to world coordinates
			mouse = Input.mousePosition;
			mouse.z = transform.position.z - Camera.main.transform.position.z;
			mouse = Camera.main.ScreenToWorldPoint(mouse);
		}

		return mouse;

	}
}
