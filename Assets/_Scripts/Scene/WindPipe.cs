using UnityEngine;
using System.Collections;

public class WindPipe : MonoBehaviour {

	private Vector3 windDirection;
	public float windSpeed;

	// Use this for initialization
	void Start () {
		
		Vector3 start = transform.GetChild(0).transform.position;
		Vector3 end = transform.GetChild(1).transform.position;

		windDirection = Vector3.Normalize(end - start);

		Object.Destroy(transform.GetChild(1).gameObject);
		Object.Destroy(transform.GetChild(0).gameObject);

	}

	void OnTriggerStay(Collider other) {
		
		if (other.gameObject.tag == "Player") {
			if (other.gameObject.transform.parent.GetComponent<PlayerController> ().state != PlayerController.player_state.ROPE) {
				other.gameObject.transform.parent.Translate (windDirection * windSpeed);
			}

			//other.gameObject.transform.parent.GetComponent<Rigidbody>().AddForce(windDirection * windSpeed);
			//print (other.gameObject.transform.parent.GetComponent<Rigidbody> ().velocity);
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player") {
			PlayerController pc = other.gameObject.transform.parent.GetComponent<PlayerController> ();
			pc.vSpeed = (windDirection * windSpeed * 10).y;
			pc.state = PlayerController.player_state.JUMPING;
		}
	}
}
