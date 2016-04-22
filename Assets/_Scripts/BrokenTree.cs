using UnityEngine;
using System.Collections;

public class BrokenTree : MonoBehaviour {

	private Rigidbody _rb1;
	private Rigidbody _rb2;

	private Vector3 _initialPos0;
	private Vector3 _initialPos1;
	private Vector3 _initialPos2;
	private Quaternion _initialRot0;
	private Quaternion _initialRot1;
	private Quaternion _initialRot2;

	// Use this for initialization
	void Start () {
		_rb1 = transform.GetChild (0).GetComponent<Rigidbody> ();	
		_rb2 = transform.GetChild (1).GetComponent<Rigidbody> ();	
		_initialPos0 = transform.position;
		_initialPos1 = transform.GetChild (0).position;
		_initialPos2 = transform.GetChild (1).position;
		_initialRot0 = transform.rotation;
		_initialRot1 = transform.GetChild (0).rotation;
		_initialRot2 = transform.GetChild (1).rotation;
	}

	public void RestartPos()
	{
		transform.position = _initialPos0;
		transform.rotation = _initialRot0;
		transform.GetChild (0).position = _initialPos1;
		transform.GetChild (0).rotation = _initialRot1;
		transform.GetChild (1).position = _initialPos2;
		transform.GetChild (1).rotation = _initialRot2;	
		_rb1.isKinematic = true;
		_rb2.isKinematic = true;
	}

	public void OnTriggerEnter(Collider collider)
	{
		_rb1.isKinematic = false;
		_rb2.isKinematic = false;
		Invoke ("RestartPos", 5.0f);
	}

}
