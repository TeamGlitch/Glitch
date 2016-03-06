using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

	private World world;

	void Start() {
		world = GameObject.Find("World").GetComponent<World>();
	}


	// Update is called once per frame
	void Update () {
		if (world.doUpdate) {
			transform.position = new Vector3(10.0f + (2.0f * Mathf.Sin(Time.time)) ,0,0);
		}
	}
}
