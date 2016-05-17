using UnityEngine;
using System.Collections;
using InControl;

public class ThingsShooter : MonoBehaviour {

	public Rigidbody log;
	public SpriteRenderer rotationAxis;

	private bool active = false;
	private float nextLaunch = 0;

	void OnTriggerStay(Collider coll){
		if (coll.gameObject.CompareTag("Player")) {
			if (InputManager.ActiveDevice.Action1.WasPressed) {
				PlayerController pc = coll.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
				if (active) {
					Deactivate(pc);
				} else {
					pc.allowMovement = false;
					rotationAxis.enabled = true;
					active = true;
				}

			}
		}
	}

	void OnTriggerExit(Collider coll){
		if (coll.gameObject.CompareTag("Player")) {
			PlayerController pc = coll.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
			Deactivate(pc);
		}
	}

	void Update () {
		if (Time.time > nextLaunch) {
			log.gameObject.transform.localPosition = new Vector3 (0, 0, 0);
			nextLaunch = Time.time + 2.5f;
			log.velocity = new Vector3 (0, 0, 0);
			float angle = log.gameObject.transform.parent.localRotation.eulerAngles.z * Mathf.Deg2Rad;

			log.AddForce (new Vector3 (Mathf.Sin(angle) * 2000f, Mathf.Cos(angle) * -2000f, 0));
		}

		if (active) {
			float x = InputManager.ActiveDevice.LeftStickX.Value;

			if (x != 0) {
				rotationAxis.gameObject.transform.parent.localRotation *= Quaternion.Euler(new Vector3 (0, 0, x * 5));
			}

		}
	}

	void Deactivate(PlayerController pc){
		pc.allowMovement = true;
		rotationAxis.enabled = false;
		active = false;
	}
}