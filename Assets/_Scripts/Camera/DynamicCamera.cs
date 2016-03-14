using UnityEngine;
using System.Collections;
using InControl;

public class DynamicCamera : MonoBehaviour {

	public enum dynamic_camera_state
	{
		PANNING,
		WAITING,
		ZOOMING
	};
		
    public Transform player;
    public Camera mainCamera;
    public World world;
    public GameObject titles;
    public int speed = 10;
    public int zoomSpeed;


	private dynamic_camera_state state = dynamic_camera_state.PANNING;
    private float delay = 5.0f;
	private int offsetX = 7;
	private int zPosition = -20;

	private Vector3 zoomSpeedVector;

    void Update () {

        // If any button is pressed the intro is skipped
		if (InputManager.ActiveDevice.AnyButton.IsPressed) {
            titles.SetActive(false);
			transform.position = new Vector3(player.position.x + offsetX, transform.position.y, zPosition);
			beginGame();

		} else {

			switch (state) {

				case dynamic_camera_state.PANNING:
					
					if (transform.position.x <= (player.position.x + offsetX)) {
						state = dynamic_camera_state.WAITING;
						titles.SetActive(true);
					} else {
						// Camera moves to left until reach player
						transform.Translate((Time.deltaTime) * -speed, 0.0f, 0.0f);
					}
					break;
							
				case dynamic_camera_state.WAITING:
					
					delay -= Time.deltaTime; 
					if (delay <= 0.0f) {
						state = dynamic_camera_state.ZOOMING;
						titles.SetActive(false);
						Vector3 destination = new Vector3 (player.transform.position.x, player.transform.position.y, zPosition);
						zoomSpeedVector = (destination - transform.position) / 2.0f;
					};
					break;
							
				case dynamic_camera_state.ZOOMING:
				
					transform.Translate(zoomSpeedVector * Time.deltaTime);
					if (transform.position.z >= zPosition)
					{
						beginGame();
					}
					break;
			}
		}
    }

	// Function that positions the main camera, active "World" (that active player
	// movements) and deactive this class.
    void beginGame()
    {
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        world.enabled = true;
        gameObject.SetActive(false);
    } 
}
