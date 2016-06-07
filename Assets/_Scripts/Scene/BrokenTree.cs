using UnityEngine;
using System.Collections;

public class BrokenTree : MonoBehaviour {

	private Rigidbody _rb1;
	private Rigidbody _rb2;
    private Rigidbody _rb3;
    private Rigidbody _rb4;
    private Rigidbody _rb5;
    private Rigidbody _rb6;
    private Rigidbody _rb7;

	private Vector3 _initialPos0;
	private Vector3 _initialPos1;
	private Vector3 _initialPos2;
    private Vector3 _initialPos3;
    private Vector3 _initialPos4;
    private Vector3 _initialPos5;
    private Vector3 _initialPos6;
    private Vector3 _initialPos7;

    private Quaternion _initialRot0;
	private Quaternion _initialRot1;
	private Quaternion _initialRot2;
    private Quaternion _initialRot3;
    private Quaternion _initialRot4;
    private Quaternion _initialRot5;
    private Quaternion _initialRot6;
    private Quaternion _initialRot7;

	// Use this for initialization
	void Start () {
		_rb1 = transform.GetChild (0).GetComponent<Rigidbody> ();	
		_rb2 = transform.GetChild (1).GetComponent<Rigidbody> ();
        _rb3 = transform.GetChild(2).GetComponent<Rigidbody>();
        _rb4 = transform.GetChild(3).GetComponent<Rigidbody>();
        _rb5 = transform.GetChild(4).GetComponent<Rigidbody>();
        _rb6 = transform.GetChild(5).GetComponent<Rigidbody>();
        _rb7 = transform.GetChild(6).GetComponent<Rigidbody>();

        _initialPos0 = transform.position;
		_initialPos1 = transform.GetChild (0).position;
		_initialPos2 = transform.GetChild (1).position;
        _initialPos3 = transform.GetChild (2).position;
        _initialPos4 = transform.GetChild (3).position;
        _initialPos5 = transform.GetChild (4).position;
        _initialPos6 = transform.GetChild (5).position;
        _initialPos7 = transform.GetChild (6).position;
        
        _initialRot0 = transform.rotation;
		_initialRot1 = transform.GetChild (0).rotation;
		_initialRot2 = transform.GetChild (1).rotation;
        _initialRot3 = transform.GetChild (2).rotation;
        _initialRot4 = transform.GetChild (3).rotation;
        _initialRot5 = transform.GetChild (4).rotation;
        _initialRot6 = transform.GetChild (5).rotation;
        _initialRot7 = transform.GetChild (6).rotation;
    }

	public void RestartPos()
	{
		transform.position = _initialPos0;
		transform.GetChild (0).position = _initialPos1;
		transform.GetChild (1).position = _initialPos2;
        transform.GetChild (2).position = _initialPos3;
        transform.GetChild (3).position = _initialPos4;
        transform.GetChild (4).position = _initialPos5;
        transform.GetChild (5).position = _initialPos6;
        transform.GetChild (6).position = _initialPos7;
        
        transform.rotation = _initialRot0;
		transform.GetChild (0).rotation = _initialRot1;
		transform.GetChild (1).rotation = _initialRot2;
        transform.GetChild (2).rotation = _initialRot3;
        transform.GetChild (3).rotation = _initialRot4;
        transform.GetChild (4).rotation = _initialRot5;
        transform.GetChild (5).rotation = _initialRot6;
        transform.GetChild (6).rotation = _initialRot7;

		_rb1.isKinematic = true;
		_rb2.isKinematic = true;
		_rb3.isKinematic = true;
		_rb4.isKinematic = true;
		_rb5.isKinematic = true;
		_rb6.isKinematic = true;
		_rb7.isKinematic = true;
	}

	public void OnTriggerEnter(Collider collider)
	{
		_rb1.isKinematic = false;
		_rb2.isKinematic = false;
		_rb3.isKinematic = false;
		_rb4.isKinematic = false;
		_rb5.isKinematic = false;
		_rb6.isKinematic = false;
		_rb7.isKinematic = false;
		Invoke ("RestartPos", 5.0f);
	}

}
