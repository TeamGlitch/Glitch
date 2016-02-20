using UnityEngine;
using System.Collections;

public class ErrorBoxScript : MonoBehaviour {

	public ErrorBoxCreator errorBoxCreator;
	public float startTime = -1;
	public float duration = -1;
		
	// Update is called once per frame
	void Update () {
		if (startTime != -1 && (Time.time - startTime >= duration)) {
			errorBoxCreator.errorBoxDeleted(1);
			Destroy (gameObject);
		}
	}
}
