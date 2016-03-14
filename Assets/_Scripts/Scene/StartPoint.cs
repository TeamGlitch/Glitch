using UnityEngine;
using System.Collections;

public class StartPoint : CheckPoint {
    public Transform playerTransform;

	void Start () {
		playerTransform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		setThisAsCheckPoint();
	}
}