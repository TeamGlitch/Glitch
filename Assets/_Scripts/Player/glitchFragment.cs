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

	private Rigidbody rigidBody;					//The rigidbody of the fragment
	private Player player;							//A reference to the player that only the first fragment has

	private fragmentPhases actualPhase;				//Actual phase

	private float phaseEnd = 0;						//When the next phase will end
	private float reagroupEnd = 0;					//When the reagroup phase will end

	private Vector3 regrupMovementVector;			//Vector to the checkpoint
	private Vector3 target;							//The position of the checkpoint

	private bool noMoreLives;

	void Awake(){
		rigidBody = GetComponent<Rigidbody>();
	}

	public void restart(Vector3 objective, bool noMoreLives = false, Player playerReference = null){

		this.noMoreLives = noMoreLives;
		target = objective;
		player = playerReference;
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

				//The first fragment (with a reference to the player)
				//will always start moving after half the possible
				//start-moving time has passed
				if (player != null)
					phaseEnd = Time.time + 0.4f;
				else
					phaseEnd = Time.time + Random.Range(0.0f, 0.8f);
				
				reagroupEnd = Time.time + 1.3f;
				rigidBody.velocity *= 0.0f;
			
			}

			break;

		case fragmentPhases.FREEZE:
			
			if (Time.time > phaseEnd) {
				if(!noMoreLives) {
					
					//Calculate the movement speed to the checkpoint
					actualPhase = fragmentPhases.REAGROUP;
					regrupMovementVector = ((target - transform.position) / 0.5f);

					//In this final phases phaseEnd is when the freezing phase ended
					phaseEnd = Time.time;

					//The first fragment tells the player to move to the checkpoint too
					if (player != null)
						player.moveToCheckPoint();
					
				}
			}
			break;

		case fragmentPhases.REAGROUP:

			//Move until enough time has passed in reagroup mode
			if (Time.time < phaseEnd + 0.5f) {
				transform.position += regrupMovementVector * Time.deltaTime;
			} else {
				actualPhase = fragmentPhases.WAIT_FOR_KILL;
			}
			break;

		case fragmentPhases.WAIT_FOR_KILL:
			
			//Wait until everyone has reagrouped and deactivate
			if (Time.time >= reagroupEnd) {

				//The first fragment tells the player to resurrect too
				if (player != null)
					player.Resurrected ();
				
				gameObject.SetActive(false);
			}
			break;
		}
	}
}
