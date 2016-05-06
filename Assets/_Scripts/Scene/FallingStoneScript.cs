using UnityEngine;
using System.Collections;

public class FallingStoneScript : MonoBehaviour {

	Rigidbody rb;
	Vector3 initPos;
	Quaternion initRotation;
	float lastTime;
	Vector3 previousVelocity;

	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody> ();
		initPos = transform.position;
		initRotation = transform.rotation;
	}
		
	public void DoUpdate(float timeToUpdate) {
		Vector3 position = transform.position;
		position = position + previousVelocity * timeToUpdate / 2.0f + Physics.gravity * Mathf.Pow (timeToUpdate/ 2.0f, 2.0f) / 2.0f;
		transform.position = position;	
		previousVelocity += Physics.gravity * timeToUpdate / 2.0f;
	}

	public void ActiveGravity()
	{
		rb.useGravity = true;
	}

	public void RestartPos()
	{
		rb.useGravity = false;
		transform.position = initPos;
		transform.rotation = initRotation;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}

	public bool IsGravityActivated()
	{
		return rb.useGravity;
	}

	public void SetKinematic(bool kinematicActive)
	{
		if (kinematicActive) {
			previousVelocity = rb.velocity;
		}
		else {
			rb.velocity = previousVelocity;
		}
		rb.useGravity = !kinematicActive;
		rb.isKinematic = kinematicActive;
	}

}
