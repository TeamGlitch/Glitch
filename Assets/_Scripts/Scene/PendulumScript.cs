using UnityEngine;
using System.Collections;

public class PendulumScript : MonoBehaviour {

	public Vector3 rotateVector;
	public float maxAngle = 60.0f;
	public float startAngle = -30.0f;
	public float timeToSwing = 2.0f;
	public SlowFPS slowFpsScript;
	public World world;
	public Vector3 _rotateOffset;

	private float _timeSwinging = 0.0f;
	private Vector3 _rotatePosition;
	private float _previousAngle = 0f;
	private bool movingRight = true;
	private float timeInFPS;
	private bool isFPSActive = false;

	private Transform previousParent;

	// Use this for initialization
	void Start () {
		_rotatePosition = transform.position + _rotateOffset;
		transform.RotateAround (_rotatePosition, rotateVector, startAngle);

		slowFpsScript.SlowFPSActivated += ActivateFPS;
		slowFpsScript.SlowFPSDeactivated += DeactivateFPS;
		timeInFPS = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {

		if(world.doUpdate)
		{			
			_timeSwinging += world.lag;

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

			transform.RotateAround(_rotatePosition, rotateVector, moveAngle);
			if (_timeSwinging >= timeToSwing)
			{
				movingRight = !movingRight;
				_timeSwinging = 0.0f;
			}
		}
	}

	void OnTriggerEnter (Collider collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			collision.gameObject.transform.parent = transform;
		}
	}

	void OnTriggerExit (Collider collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			collision.gameObject.transform.parent = null;
		}
	}

	void ActivateFPS()
	{
		isFPSActive = true;
		timeInFPS = 0.0f;
	}

	void DeactivateFPS()
	{
		isFPSActive = false;
	}
}
