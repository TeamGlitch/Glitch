using UnityEngine;
using System.Collections;

public class glitchFragment : MonoBehaviour {


	public enum fragmentPhases
	{
		EXPLODE,
		SLOW_DOWN,
		FREEZE,
		REAGROUP,
		WAIT_FOR_KILL
	};

	private Rigidbody rigidBody;
	private Player player;

	private fragmentPhases actualPhase;

	private float phaseEnd = 0;
	private float reagroupEnd = 0;

	private Vector3 regrupMovementVector;
	private Vector3 target;

	private bool noMoreLives;

	void Awake(){
		rigidBody = GetComponent<Rigidbody>();
	}

	public void restart(Vector3 targt, Player plyr = null, bool noMoreLives = false){

		this.noMoreLives = noMoreLives;
		target = targt;
		player = plyr;
		actualPhase = fragmentPhases.EXPLODE;
		phaseEnd = Time.time + 1.5f;
		rigidBody.useGravity = true;

		float scale = Random.Range (1.0f, 3.0f);
		transform.localScale= new Vector3 (scale, scale, 1.0f);

		Vector3 explosionForce = new Vector3 (Random.Range (-5.0f, 5.0f), Random.Range (4.0f, 15.0f), Random.Range (-5.0f, 5.0f));
		GetComponent<Rigidbody>().AddForce(explosionForce);
	}

	// Update is called once per frame
	void Update () {

		switch (actualPhase) {

		case fragmentPhases.EXPLODE:
			if (Time.time > phaseEnd) {
				actualPhase = fragmentPhases.SLOW_DOWN;
				phaseEnd = Time.time + 1.0f;
				rigidBody.useGravity = false;
			}
			break;

		case fragmentPhases.SLOW_DOWN:
			if (Time.time < phaseEnd) {
				rigidBody.velocity *= 0.95f;
			} else {
				actualPhase = fragmentPhases.FREEZE;
				phaseEnd = Time.time + Random.Range(0.0f, 0.8f);
				reagroupEnd = Time.time + 1.3f;
				rigidBody.velocity *= 0.0f;
			}

			break;

		case fragmentPhases.FREEZE:
			if (Time.time > phaseEnd) {
				if(!noMoreLives)
					actualPhase = fragmentPhases.REAGROUP;
				phaseEnd = Time.time;
				regrupMovementVector = ((target - transform.position) / 0.5f);
			}
			break;

		case fragmentPhases.REAGROUP:
			if (Time.time < phaseEnd + 0.5f) {
				transform.position += regrupMovementVector * Time.deltaTime;
			} else {
				actualPhase = fragmentPhases.WAIT_FOR_KILL;
			}
			break;

		case fragmentPhases.WAIT_FOR_KILL:
			if (Time.time >= reagroupEnd) {
				if (player != null)
					player.Resurrected ();
				gameObject.SetActive(false);
			}
			break;
		}
	}
}
