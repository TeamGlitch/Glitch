using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour {

	public World world;
	public float speed;

	// Update is called once per frame
	void Update () {

		if (world.doUpdate) {
			transform.rotation *= Quaternion.Euler (new Vector3 (0, 0, speed * world.lag));
		}


	}
}

