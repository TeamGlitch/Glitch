using UnityEngine;
using System.Collections;

public class RotateModelToRec : MonoBehaviour {

	float angle = 180.0f;
		
	// Update is called once per frame
	void Update () {
		angle += 1.0f;
		gameObject.transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
	}
}
