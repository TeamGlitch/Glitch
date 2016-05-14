using UnityEngine;
using System.Collections;

public class BreakableCeling : MonoBehaviour {

	void OnTriggerEnter(Collider coll){
		if (coll.gameObject.tag == "Player") {
			for (int i = 0; i < transform.childCount; i++) {
				Rigidbody rb = transform.GetChild(i).gameObject.GetComponent<Rigidbody>();
				rb.useGravity = true;
				rb.isKinematic = false;
				rb.AddExplosionForce (25.0f, coll.transform.position, 20f, 0.0f, ForceMode.Impulse);
			}
			Destroy(this);
		}
	}
}
