using UnityEngine;
using System.Collections;

public class ErrorBoxScript : MonoBehaviour {

	public ErrorBoxCreator errorBoxCreator;
	private float elapsedTime = 0;
	public float duration = -1;
		
	// Update is called once per frame
	void Update () {
		elapsedTime++;
		if (elapsedTime >= duration) {
			errorBoxCreator.errorBoxDeleted(1);
			Destroy (gameObject);
		}
	}
}
