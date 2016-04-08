using UnityEngine;
using System.Collections;

public class SpikeTrapScript : MonoBehaviour
{
	public SlowFPS slowFpsScript;
	public World world;
	public float timeTrapWaitsInActivation = 1.0f;
	public float timeTrapWaitsInDeactivation = 2.0f;
	public float verticalMovementWhenActivated = 1.0f;

	private float initialYPos;
	private bool activated = false;
	private bool deactivated = false;
	private float timeWhenActivated;
	private float timeWhenDeactivated;

	private bool isFPSActive = false;
	private float timeInFPS;
	// Use this for initialization
	void Start ()
	{
		initialYPos = transform.position.y;
		slowFpsScript.SlowFPSActivated += ActivateFPS;
		slowFpsScript.SlowFPSDeactivated += DeactivateFPS;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isFPSActive) {
			if (world.doUpdate) {
				if (activated && (timeWhenActivated > timeTrapWaitsInActivation)) {
					if (transform.position.y - initialYPos < verticalMovementWhenActivated) {
						Vector3 temporalPos = transform.position;
						temporalPos.y += 0.1f;
						transform.position = temporalPos;
					} else {
						activated = false;
					}
				}
				else if (activated) {
					timeWhenActivated += timeInFPS/2.0f;
				}
				else if (deactivated && (timeWhenDeactivated > timeTrapWaitsInDeactivation)) {
					if (transform.position.y > initialYPos) {
						Vector3 temporalPos = transform.position;
						temporalPos.y -= 0.1f;
						transform.position = temporalPos;
					} else {
						deactivated = false;
					}
				}
				else if(deactivated) {
					timeWhenDeactivated += timeInFPS/2.0f;
				}
			}
			timeInFPS += Time.deltaTime;
		}
		if (world.doUpdate) {
			if (activated && (timeWhenActivated > timeTrapWaitsInActivation)) {
				if (transform.position.y - initialYPos < verticalMovementWhenActivated) {
					Vector3 temporalPos = transform.position;
					temporalPos.y += 0.1f;
					transform.position = temporalPos;
				} else {
					activated = false;
				}
			}
			else if (activated) {
				timeWhenActivated += Time.deltaTime;
			}
			else if (deactivated && (timeWhenDeactivated > timeTrapWaitsInDeactivation)) {
				if (transform.position.y > initialYPos) {
					Vector3 temporalPos = transform.position;
					temporalPos.y -= 0.1f;
					transform.position = temporalPos;
				} else {
					deactivated = false;
				}
			}
			else if(deactivated) {
				timeWhenDeactivated += Time.deltaTime;
			}
		}
	}

	void OnTriggerEnter (Collider other)
	{
		timeWhenActivated = 0.0f;
		activated = true;
	}

	void OnTriggerExit (Collider other)
	{
		timeWhenDeactivated = 0.0f;
		deactivated = true;
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