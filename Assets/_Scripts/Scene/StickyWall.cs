using UnityEngine;
using System.Collections;

public class StickyWall : MonoBehaviour {

	void OnTriggerStay(Collider coll){
		if (coll.gameObject.CompareTag("Player")) {
			PlayerController pc = coll.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
			pc.state = PlayerController.player_state.STICKED;
			pc.vSpeed = 0;
		}
	}

	void OnTriggerExit(Collider coll){
		if (coll.gameObject.CompareTag("Player")) {
			PlayerController pc = coll.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
			pc.state = PlayerController.player_state.JUMPING;
		}
	}
}
