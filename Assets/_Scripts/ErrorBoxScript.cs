using UnityEngine;
using System.Collections;

public class ErrorBoxScript : MonoBehaviour {

	public Vector3 positionWhenNotActive;

	public bool boxActive;
	public bool canBeActivated;
	private float timeInitiated;
	private float timeUnactive;

	// Use this for initialization
	void Start () {
		timeInitiated = 0.0f;
		timeUnactive = 8.0f;
		boxActive = false;
	}
		
	// Update is called once per frame
	void Update () {
		if (boxActive) {
			timeInitiated += Time.deltaTime;
			if (timeInitiated > 5.0f) {
				boxActive = false;
				transform.position = positionWhenNotActive;
				timeUnactive = 0.0f;
			}
		} else if ((canBeActivated == false) && (timeUnactive >= 8.0f)) {
			canBeActivated = true;
		} else if (canBeActivated == false) {
			timeUnactive += Time.deltaTime;
		}
	}

	public void ActivateBox(Vector3 wantedPosition) {
		boxActive = true;
		canBeActivated = false;
		timeInitiated = 0.0f;
		transform.position = wantedPosition;
	}
}
