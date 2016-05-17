using UnityEngine;
using System.Collections;

public class StickyWall : MonoBehaviour {

	public bool viscosity = true;

	private float viscosityValue = 0;

	void OnTriggerStay(Collider coll){
		if (coll.gameObject.CompareTag("Player")) {
			PlayerController pc = coll.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
			pc.state = PlayerController.player_state.STICKED;

			pc.teleport.teleportUsed = false;

			if (viscosity) {
				pc.vSpeed = viscosityValue;
				viscosityValue -= 0.1f;
			} else {
				pc.vSpeed = 0;
			}
		}
	}

	void OnTriggerExit(Collider coll){
		if (coll.gameObject.CompareTag("Player")) {
			PlayerController pc = coll.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
			pc.state = PlayerController.player_state.JUMPING;
			if (viscosity)
				viscosityValue = 0;
		}
	}
}
