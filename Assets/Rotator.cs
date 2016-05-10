using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	public float speed;
	private float angle = 0;
	private float distance = 0;

	// Use this for initialization
	void Start () {
		distance = transform.localPosition.x;
	}

	// Update is called once per frame
	void Update () {
		
		angle += speed;
		transform.localPosition = new Vector3 (Mathf.Cos (angle) * distance, Mathf.Sin (angle) * distance, transform.localPosition.z);

		if (angle > 2 * Mathf.PI)
			angle -= 2 * Mathf.PI;
	}
}
