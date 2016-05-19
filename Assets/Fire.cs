using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

	private bool active = false;
	private float timeToChange = 0;

	public float frequency;

	// Update is called once per frame
	void Update () {
		if (Time.time > timeToChange) {
			if (active) {
				active = false;
			} else {
				active = true;
			}
			timeToChange = Time.time + frequency;
		}
	}
}
