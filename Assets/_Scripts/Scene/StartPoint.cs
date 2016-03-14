using UnityEngine;
using System.Collections;

public class StartPoint : CheckPoint {

	void Start () {
		GameObject player = GameObject.Find("Player");
		player.transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		setThisAsCheckPoint(player);
	}
}