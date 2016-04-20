using UnityEngine;
using System.Collections;

public class PendulumScript : MonoBehaviour {

	public float maxAngle = 60.0f;
	public float startAngle = -30.0f;
	public float timeToSwing = 2.0f;
	private float _timeSwinging = 0.0f;
	private Vector3 _rotatePosition;
	public Vector3 _rotateOffset;
	private float _previousAngle = 0f;
	private bool movingRight = true;

	// Use this for initialization
	void Start () {
		_rotatePosition = transform.position + _rotateOffset;
		transform.RotateAround (_rotatePosition, new Vector3(0f,0f,1f), startAngle);
	}
	
	// Update is called once per frame
	void Update () {
		_timeSwinging += Time.deltaTime;
		if (_timeSwinging > timeToSwing)
			_timeSwinging = timeToSwing;
		float angle;
		float t = _timeSwinging / timeToSwing;
		t = t*t*t*(t*(6f*t - 15f) +10f);
		if(movingRight)
			angle = Mathf.LerpAngle (0.0f, maxAngle, t);
		else
			angle = Mathf.LerpAngle (maxAngle, 0.0f, t);

		float moveAngle = angle - _previousAngle;
		_previousAngle = angle;

		transform.RotateAround(_rotatePosition, new Vector3(0f,0f,1f), moveAngle);
		if (_timeSwinging >= timeToSwing)
		{
			movingRight = !movingRight;
			_timeSwinging = 0.0f;
		}
	}

	void OnTriggerEnter (Collider collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			collision.gameObject.transform.rotation = transform.rotation;
			collision.gameObject.transform.parent = transform;

		}
	}

	void OnTriggerExit (Collider collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			collision.gameObject.transform.parent = null;
		}
	}
}
