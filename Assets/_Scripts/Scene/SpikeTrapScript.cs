using UnityEngine;
using System.Collections;

public class SpikeTrapScript : MonoBehaviour
{
	public SlowFPS slowFpsScript;
	public World world;
	public float timeTrapWaitsInActivation = 1.0f;
	public float timeTrapWaitsInDeactivation = 2.0f;
	public float lerpTime = 1.0f;
	public Vector3 moveDistance = new Vector3 (0f, 1.5f, 0f);

	Vector3 startPosition;
	Vector3 endPosition;
	float currentLerpTime;

	private bool activated = false;
	private bool deactivated = false;
	private float timeWhenActivated;
	private float timeWhenDeactivated;

	private bool isFPSActive = false;
	private float timeInFPS;
	// Use this for initialization
	void Start ()
	{
		slowFpsScript.SlowFPSActivated += ActivateFPS;
		slowFpsScript.SlowFPSDeactivated += DeactivateFPS;
		startPosition = transform.position;
		endPosition = transform.position + moveDistance;
		currentLerpTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isFPSActive) {
			timeInFPS += Time.deltaTime;
		}
		if (activated && (timeWhenActivated > timeTrapWaitsInActivation)) {
			if (isFPSActive) {
				currentLerpTime += timeInFPS / 2.0f;
				timeInFPS = 0.0f;
			}
			else
				currentLerpTime += Time.deltaTime;
			float lerpPercentage = currentLerpTime / lerpTime;
			if (lerpPercentage > 1.0f) {
				currentLerpTime = 1.0f;
				activated = false;
			}
			Vector3 temporalPos = Vector3.Lerp (startPosition, endPosition, lerpPercentage);
			transform.position = temporalPos;
				if (currentLerpTime == 1.0f) {
				activated = false;
				timeWhenDeactivated = 0.0f;
				currentLerpTime = 0.0f;
				deactivated = true;
			}
		}
		else if (activated) {
			if (isFPSActive) {
				timeWhenActivated += timeInFPS / 2.0f;
				timeInFPS = 0.0f;
			}
			else
				timeWhenActivated += Time.deltaTime;
		}
		else if (deactivated && (timeWhenDeactivated > timeTrapWaitsInDeactivation)) {
			if (isFPSActive) {
				currentLerpTime += timeInFPS / 2.0f;
				timeInFPS = 0.0f;
			}
			else
				currentLerpTime += Time.deltaTime;
			float lerpPercentage = currentLerpTime / lerpTime;
			if (lerpPercentage > 1.0f) {
				currentLerpTime = 1.0f;
				deactivated = false;
			}
			Vector3 temporalPos = Vector3.Lerp (endPosition, startPosition, lerpPercentage);
			transform.position = temporalPos;
					if(currentLerpTime == 1.0f)
				deactivated = false;
		}
		else if(deactivated) {
			if (isFPSActive) {
				timeWhenDeactivated += timeInFPS / 2.0f;
				timeInFPS = 0.0f;
			}
			else
				timeWhenDeactivated += Time.deltaTime;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		timeWhenActivated = 0.0f;
		currentLerpTime = 0.0f;
		activated = true;
	}

	void OnTriggerExit (Collider other)
	{
		if (!activated)
		{
			timeWhenDeactivated = 0.0f;
			currentLerpTime = 0.0f;
			deactivated = true;
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