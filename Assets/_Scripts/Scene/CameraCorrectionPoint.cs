using UnityEngine;
using System.Collections;

public class CameraCorrectionPoint : MonoBehaviour {
	
	//How the camera is when going to the right
	//and how the camera is when going to the left
	public CameraBehaviour behaviour;

	// Use this for initialization
	void Awake () {

		//Get the Camera Zone Point base
		Transform zoneBase = gameObject.transform.parent.parent;

		//Get the pivots & centerer
		GameObject upPivot = zoneBase.GetChild(0).gameObject;
		GameObject downPivot = zoneBase.GetChild(1).gameObject;
		GameObject centerer = zoneBase.GetChild(2).gameObject;

		if (behaviour.mode == camera_mode.ON_RAILS) {

			//If the camera mode is "ON_RAILS", the pivot y positions are important,
			//so store them
			behaviour.upPivot = upPivot.transform.position.y;
			behaviour.downPivot = downPivot.transform.position.y;

		}

		//Destroy the pivots
		Destroy (upPivot);
		Destroy (downPivot);

		//Set the zoneBase as the new parent and delete the centerer
		gameObject.transform.parent = zoneBase;
		Destroy(centerer);

		gameObject.GetComponent<Renderer>().enabled = false;

	}
		
	void OnTriggerEnter(Collider coll){

		//If the player has gone trought the collider and is exiting...
		if (coll.gameObject.name == "Player") {
			//Get the main camera controller
			MainCamera controller = Camera.main.GetComponent<MainCamera> ();

			//Assign the current behaviour to the camera
			controller.behaviour = behaviour;

			//Move the camera rails if it's the on_rails behaviour
			if (behaviour.mode == camera_mode.ON_RAILS) {
				Vector3 upRail = controller.upRail.transform.position;
				Vector3 downRail = controller.downRail.transform.position;
				upRail.y = behaviour.upPivot;
				downRail.y = behaviour.downPivot;
				controller.upRail.transform.position = upRail;
				controller.downRail.transform.position = downRail;
			}

		}

	}
}