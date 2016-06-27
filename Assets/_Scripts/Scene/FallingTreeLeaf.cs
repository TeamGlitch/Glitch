using UnityEngine;
using System.Collections;

public class FallingTreeLeaf : MonoBehaviour {

	public float timeBeforeFall = 0.5f;
	float timeSinceColliderTouched;
	Rigidbody rb;
	bool colliderTouched;
	Vector3 initPos;
	Quaternion initRot;
    public Player playerScript;

	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody> ();
		initPos = transform.position;
		initRot = transform.rotation;
        playerScript.PlayerDeadEvent += Reset;
    }

	void Update() {
		timeSinceColliderTouched += Time.deltaTime;
		if (colliderTouched && (timeSinceColliderTouched > timeBeforeFall)) {
			rb.isKinematic = false;
		}
	}

    public void Reset()
    {
        colliderTouched = false;
        rb.isKinematic = true;
        transform.position = initPos;
        transform.rotation = initRot;
        timeSinceColliderTouched = 0.0f;
    }
	
	void OnTriggerEnter(Collider collider) {
		if (collider.tag == "Player") {
			colliderTouched = true;
			timeSinceColliderTouched = 0.0f;
		}
	}

}
