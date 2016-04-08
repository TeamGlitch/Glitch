using UnityEngine;
using System.Collections;

public class MovingStoneScript : MonoBehaviour {

	public Vector3 distanceToMove = new Vector3 (0.0f, -5.0f, 0.0f);
	public float lerpTime = 1.0f;
	public SlowFPS slowFpsScript;
	public World world;

	Vector3 startPosition;
	Vector3 endPosition;
	float currentLerpTime;

	private float timeInFPS;
	private bool isFPSActive = false;

	// Use this for initialization
	void Start () {
		slowFpsScript.SlowFPSActivated += ActivateFPS;
		slowFpsScript.SlowFPSDeactivated += DeactivateFPS;
		startPosition = transform.position;
		endPosition = transform.position + distanceToMove;
		currentLerpTime = 0.0f;
		timeInFPS = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isFPSActive)
		{
			timeInFPS += Time.deltaTime;
			if(world.doUpdate)
			{
				currentLerpTime += timeInFPS / 2.0f;
				timeInFPS = 0.0f;
			}
		}
		else
		{
			currentLerpTime += Time.deltaTime;
		}
		if (currentLerpTime > lerpTime)
			currentLerpTime = lerpTime;

		float t = currentLerpTime / lerpTime;
		t = t * t * t * (t * (6.0f * t - 15.0f) + 10.0f);
		transform.position = Vector3.Lerp(startPosition, endPosition, t);

		if (currentLerpTime == lerpTime)
		{
			currentLerpTime = 0.0f;
			Vector3 aux = startPosition;
			startPosition = endPosition;
			endPosition = aux;
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
