using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

	private bool active = false;
	private Collider collider;
	private Renderer renderer;

	private float timeToChange = 0;

	public float frequency;

	void Start(){
		collider = GetComponent<Collider>();
		renderer = GetComponent<Renderer>();
	}

	// Update is called once per frame
	void Update () {
		if (Time.time > timeToChange) {
			if (active) {
				active = false;
				renderer.material.color = new Color (1f, 0, 0, 0.5f);
				collider.enabled = false;
			} else {
				active = true;
				renderer.enabled = true;
				collider.enabled = true;
				renderer.material.color = new Color (1f, 0, 0, 1f);
			}
			timeToChange = Time.time + frequency;
		}
	}
}
