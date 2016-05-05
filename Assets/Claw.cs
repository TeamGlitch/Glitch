using UnityEngine;
using System.Collections;

public class Claw : MonoBehaviour {

	Animation animation;

	void Awake(){
		animation = gameObject.GetComponent<Animation>();
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player")
			animation.Play();
	}
}
