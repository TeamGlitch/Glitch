using UnityEngine;
using System.Collections;

public class LavaStone1 : MonoBehaviour {

	enum lava_stone_state {
		JUMPING,
		FALLING,
		WAIT
	}

	public World world;

	private lava_stone_state state;

	public float waitingTimeToAddForce = 10.0f;
	public float possibleVariation = 1.0f;

	private Vector3 initialPos;
	private Vector3 endPos;
	private float jumpStart = 0;

	// Use this for initialization
	void Start () {
		waitingTimeToAddForce = Random.Range (waitingTimeToAddForce - possibleVariation, waitingTimeToAddForce + possibleVariation);
		initialPos = transform.position;
		endPos = initialPos;
		endPos.y += 10f;
		jumpStart = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (world.doUpdate){

			float timePassed = (Time.time - jumpStart) / 2.5f;

			Vector3 mov = transform.position;

			switch (state) {

			case lava_stone_state.JUMPING:

				if (timePassed >= 0.5) {
					state = lava_stone_state.FALLING;
				}

				//transform.position = Vector3.Slerp (initialPos, endPos, elapsed);
				Vector3 position = transform.position;
				position.y = initialPos.y + (10 * Mathf.Pow (Mathf.Cos (((timePassed * 10f) / Mathf.PI)), 2));
				transform.position = position;

				break;

			case lava_stone_state.FALLING:

				if (timePassed >= 1) {
					timePassed = 0;
					jumpStart = Time.time;
					state = lava_stone_state.JUMPING;
				}

				Vector3 positionO = transform.position;
				positionO.y = initialPos.y + (10 * Mathf.Pow (Mathf.Cos (((timePassed * 10f) / Mathf.PI)), 2));
				transform.position = positionO;

				break;

			}



		}
	}
}
