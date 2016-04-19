using UnityEngine;
using System.Collections;

public class LavaStone : MonoBehaviour {

	public World world;
	public float waitingTimeToAddForce = 10.0f;
	public float possibleVariation = 1.0f;
	public float forceApplied = 800.0f;
    public float rotationApplied = 1.0f;
	public SlowFPS slowFpsScript;
	Rigidbody rb;
	float timeWaiting = 0.0f;
	Vector3 initialPos;
	float timeInFPS;
	bool isFPSActive = false;
	Vector3 previousVelocity;

	// Use this for initialization
	void Start () {
		timeInFPS = 0.0f;
		rb = transform.GetComponent<Rigidbody> ();
		waitingTimeToAddForce = Random.Range (waitingTimeToAddForce - possibleVariation, waitingTimeToAddForce + possibleVariation);
		initialPos = transform.position;
		slowFpsScript.SlowFPSActivated += ActivateFPS;
		slowFpsScript.SlowFPSDeactivated += DeactivateFPS;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isFPSActive)
		{
			timeInFPS += Time.deltaTime;
			if (world.doUpdate)
			{
				Vector3 position = transform.position;
				position = position + previousVelocity * timeInFPS / 2.0f + Physics.gravity * Mathf.Pow (timeInFPS/ 2.0f, 2.0f) / 2.0f;
				if (position.y < initialPos.y)
					position.y = initialPos.y;
				transform.position = position;	
				previousVelocity += Physics.gravity * timeInFPS / 2.0f;
				timeWaiting += timeInFPS/2.0f;
				if (timeWaiting > waitingTimeToAddForce)
				{
					rb.AddForce (new Vector3 (0.0f, forceApplied, 0.0f));
					timeWaiting = 0.0f;
				}
				timeInFPS = 0.0f;
			}
		}
		else
		{
			timeWaiting += Time.deltaTime;
			if (timeWaiting > waitingTimeToAddForce)
			{
				rb.AddForce (new Vector3 (0.0f, forceApplied, 0.0f));
				timeWaiting = 0.0f;
			}
		}
	}

	void ActivateFPS()
	{
		previousVelocity = rb.velocity;
		isFPSActive = true;
		rb.isKinematic = true;
		rb.useGravity = false;
		timeInFPS = 0.0f;
	}

	void DeactivateFPS()
	{
		rb.useGravity = true;
		rb.isKinematic = false;
		isFPSActive = false;
		rb.velocity = previousVelocity;
	}
}
