using UnityEngine;
using System.Collections;

public class SpikeTrapScript : MonoBehaviour
{
	public World world;
	public float timeTrapWaitsInActivation = 1.0f;
	public float timeTrapWaitsInDeactivation = 2.0f;
	public float lerpTime = 1.0f;
	public Vector3 moveDistance = new Vector3 (0f, 1.5f, 0f);

	public GameObject leaves;
	public ParticleSystem leavesParticle;
	bool leavesJumped = false;

	Vector3 startPosition;
	Vector3 endPosition;
	float currentLerpTime;

	private bool activated = false;
	private bool deactivated = false;
	private float timeWhenActivated;
	private float timeWhenDeactivated;

	// Use this for initialization
	void Start ()
	{
		startPosition = transform.position;
		endPosition = transform.position + moveDistance;
		currentLerpTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(world.doUpdate)
		{
			if (activated && (timeWhenActivated > timeTrapWaitsInActivation))
			{
				currentLerpTime += world.lag;
				float lerpPercentage = currentLerpTime / lerpTime;
				if (lerpPercentage > 1.0f)
				{
					currentLerpTime = 1.0f;
					activated = false;
				}
				Vector3 temporalPos = Vector3.Lerp (startPosition, endPosition, lerpPercentage);
				transform.position = temporalPos;
				if (currentLerpTime == 1.0f)
				{
					activated = false;
					timeWhenDeactivated = 0.0f;
					currentLerpTime = 0.0f;
					deactivated = true;
				}
			}
			else if (activated)
			{
				timeWhenActivated += world.lag;
				if (timeWhenActivated > 0.5f && !leavesJumped) {
					leaves.SetActive(false);
					leavesJumped = true;
				}
			}
			else if (deactivated && (timeWhenDeactivated > timeTrapWaitsInDeactivation))
			{
				currentLerpTime += world.lag;
				float lerpPercentage = currentLerpTime / lerpTime;
				if (lerpPercentage > 1.0f)
				{
					currentLerpTime = 1.0f;
					deactivated = false;
				}
				Vector3 temporalPos = Vector3.Lerp (endPosition, startPosition, lerpPercentage);
				transform.position = temporalPos;
				if(currentLerpTime == 1.0f)
					deactivated = false;
			}
			else if(deactivated)
			{
				timeWhenDeactivated += world.lag;
			}
		}
	}

	void OnTriggerEnter (Collider other)
	{
		timeWhenActivated = 0.0f;
		currentLerpTime = 0.0f;
		activated = true;
		if (!leavesJumped) {
			leavesParticle.Play();
		}
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
}