using UnityEngine;
using System.Collections;

public class BarrelDeleter : MonoBehaviour {

	void OnTriggerEnter (Collider collision){
		if (collision.gameObject.name == "Barrel") {
			collision.gameObject.SetActive (false);
		}
	}
}
