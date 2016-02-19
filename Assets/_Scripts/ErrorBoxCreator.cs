using UnityEngine;
using System.Collections;

public class ErrorBoxCreator : MonoBehaviour {

	private int numBoxes = 0;
	public GameObject errorBoxPrefab;

	// Update is called once per frame
	void Update () {
		//Errorbox creation
		if (Input.GetButtonDown("PowerAction_0") && (!Input.GetButton("ToggleTextureChange") && Input.GetAxisRaw("ToggleTextureChange") == 0) ) {
			if (numBoxes < 3) {
				Vector3 mouse = Input.mousePosition;
				mouse.z = 15;
				mouse = Camera.main.ScreenToWorldPoint (mouse);

				GameObject errorBox = (GameObject)Instantiate (errorBoxPrefab);
				errorBox.transform.position = new Vector3 (mouse.x, mouse.y, 0);
				errorBox.GetComponent<ErrorBoxScript>().duration = 500;
				errorBox.GetComponent<ErrorBoxScript>().errorBoxCreator = this;
				numBoxes++;
			}
		}
	}

	//Announce that a error box was deleted
	public void errorBoxDeleted (int num)
	{
		numBoxes -= num;
	}
}
