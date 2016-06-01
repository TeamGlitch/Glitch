using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	public CameraBehaviour behaviour;
	public PlayerController player;
	public GameObject CameraZones;

	//Common variables
	private Vector3 vSpeed;
	private float smooth = 0.15f;

	//Following mode
	public float maxUp = 0.5f;
	public float maxDown = 0.33f;
	private float playerPositionX = 0.27f; //In percent

	//On_rails mode
	public Transform upRail;
	public Transform downRail;

	void OnEnable() {
		CameraZones.SetActive(true);

	}

	void Update()
	{

		if (Camera.current == Camera.main) 
		{

			//Calculates the smooth depending on the ratio and the zoom
			float ratio = Camera.current.aspect;
			float distance = Camera.current.transform.position.z;
			smooth = (ratio * (0.0423f - (0.0068f * distance))) + (0.0011f * distance) - 0.0546f; 

			//Sets the camera movement so the player has the given x position (In viewport coordinates)
			Vector3 positionOnViewport = Camera.main.WorldToViewportPoint(player.transform.position);
			Vector3 expectedXPos = Camera.main.ViewportToWorldPoint(new Vector3(playerPositionX, 0, positionOnViewport.z));
			expectedXPos = expectedXPos - player.transform.position;
			float correct = transform.position.x - expectedXPos.x;

			switch (behaviour.mode) {

			case camera_mode.FOLLOWING:

				Vector3 objective = transform.position;
				objective.x = correct;

				// Camera only moves if it's over the 50% of the screen or over the 33% of the screen
				if (positionOnViewport.y > maxUp) 
				{
					//Checks where the expected player position would be (top 50% of the screen) and where it really is, and
					//moves the camera to that position calculating the distance
					Vector3 expectedPosition = Camera.main.ViewportToWorldPoint ( new Vector3(0, maxUp, positionOnViewport.z) );
					expectedPosition = expectedPosition - player.transform.position;
					objective.y -= expectedPosition.y;

				} else if (positionOnViewport.y < maxDown) {

					Vector3 expectedPosition = Camera.main.ViewportToWorldPoint ( new Vector3(0, maxDown, positionOnViewport.z) );
					expectedPosition = expectedPosition - player.transform.position;
					objective.y -= expectedPosition.y;
				}

				//The posx is a smooth transition between two points, in this case the current position and the current position with a x offset
				transform.position =Vector3.SmoothDamp(transform.position, objective, ref vSpeed, smooth);

				break;

			case camera_mode.ON_RAILS:

				//The Y position of the camera is the middle point between the bottom and top of the screen
				//Being this the opposite side, the adjacent side is calculated with the tangent of the
				//triangle angle (half the camera field of view). This is the Z distance from the points.
				float Yposition = (upRail.position.y - downRail.position.y) * 0.5f;
				float Zposition = Yposition / Mathf.Tan (Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);

				Vector3 target = downRail.position;
				target.y += Yposition;
				target.z -= Zposition;
				target.x = correct;

				transform.position = Vector3.SmoothDamp(transform.position, target, ref vSpeed, smooth);

				break;

			}
		}
	} 

}
