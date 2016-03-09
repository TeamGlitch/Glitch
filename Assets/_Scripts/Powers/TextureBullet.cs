using UnityEngine;
using System.Collections;

public class TextureBullet : MonoBehaviour {

	public Vector3 speed;

	// Update is called once per frame
	public void Update () {
		this.transform.position += speed * Time.deltaTime;

		//If it's out of the screen, delete
		Vector3 screenPos = Camera.main.WorldToViewportPoint(this.transform.position);
		if (screenPos.x > 1 || screenPos.x < 0
			|| screenPos.y > 1 || screenPos.y < 0) {
			gameObject.SetActive(false);
		}
	}
}
