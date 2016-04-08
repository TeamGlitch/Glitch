using UnityEngine;
using System.Collections;

public class FallingTreeLeaf : MonoBehaviour {

	public float timeBeforeFall = 0.5f;
	float timeSinceColliderTouched;
	Rigidbody rb;
	bool colliderTouched;
	Vector3 initPos;

	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody> ();
		initPos = transform.position;
	}

	void Update() {
		timeSinceColliderTouched += Time.deltaTime;
		if (colliderTouched && (timeSinceColliderTouched > timeBeforeFall)) {
			rb.isKinematic = false;
		}
		if(colliderTouched && (timeSinceColliderTouched > (timeBeforeFall+2.0f)))
		{
			colliderTouched = false;
			rb.isKinematic = true;
			transform.position = initPos;
			timeSinceColliderTouched = 0.0f;
		}
	}
	
	void OnTriggerEnter(Collider collider) {
		if (collider.tag == "Player") {
			colliderTouched = true;
			timeSinceColliderTouched = 0.0f;
		}
	}

}
