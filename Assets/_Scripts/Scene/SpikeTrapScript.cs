using UnityEngine;
using System.Collections;

public enum spikeTrapState
{
	RESTING,
	WAITING,
	JUMPING,
	ACTIVE,
	HIDING
};

public class SpikeTrapScript : MonoBehaviour
{
	public World world;

	private spikeTrapState state;

	public float timeTrapWaitsInActivation = 1.0f;
	public float timeTrapWaitsInDeactivation = 2.0f;
	public float lerpTime = 1.0f;
	public Vector3 moveDistance = new Vector3 (0f, 1.5f, 0f);

    public AudioClip trapSound;
    public GameObject leaves;
	public ParticleSystem leavesParticle;
	bool leavesJumped = false;

	Vector3 startPosition;
	Vector3 endPosition;
	float currentLerpTime;

	private float timeToJump;
	private float timeToDeactivate;

	// Use this for initialization
	void Start ()
	{
		startPosition = transform.position;
		endPosition = transform.position + moveDistance;
		currentLerpTime = 0.0f;
		leavesParticle.Stop();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (world.doUpdate) {

			switch (state) {

			case spikeTrapState.WAITING:

				if (!leavesJumped && leavesParticle.isStopped && Time.time >= timeToJump - 1.0f) {
					leavesParticle.Stop();
					leavesParticle.Clear();
					leavesParticle.Play();
				}

				if (Time.time >= timeToJump) {
					state = spikeTrapState.JUMPING;
					if (!leavesJumped) {
						leaves.SetActive(false);
						leavesJumped = true;
					}
				}

				break;

			case spikeTrapState.JUMPING:

				currentLerpTime += world.lag;
				float lerpPercentage = currentLerpTime / lerpTime;
				if (lerpPercentage > 1.0f) {
					timeToDeactivate = Time.time + timeTrapWaitsInDeactivation;	
					state = spikeTrapState.ACTIVE;
				}

				transform.position = Vector3.Lerp (startPosition, endPosition, lerpPercentage);


				break;

			case spikeTrapState.ACTIVE:

				if (Time.time >= timeToDeactivate) {
					currentLerpTime = 0.0f;
					state = spikeTrapState.HIDING;
				}

				break;

			case spikeTrapState.HIDING:

				currentLerpTime += world.lag;
				float lerpPercentageO = currentLerpTime / lerpTime;
				if (lerpPercentageO > 1.0f) {
					currentLerpTime = 1.0f;
					state = spikeTrapState.RESTING;
				}

				transform.position = Vector3.Lerp (endPosition, startPosition, lerpPercentageO);

				break;

			}
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag ("Player") && state == spikeTrapState.RESTING)
		{
            startJump();
		}
	}

    public void startJump()
    {
        timeToJump = Time.time + timeTrapWaitsInActivation;
        currentLerpTime = 0.0f;
        state = spikeTrapState.WAITING;
    }
}