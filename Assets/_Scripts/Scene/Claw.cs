using UnityEngine;
using System.Collections;

public class Claw : MonoBehaviour {

	public World world;
	private Animation animation;
	private bool sfpsActivated = false;

	void Awake(){
		animation = gameObject.GetComponent<Animation>();
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player")
			animation.Play();
	}

	void Update(){
		
		if (animation.isPlaying) {

			if (world.slowFPSActived && sfpsActivated == false) {

				animation ["ClawTrap"].speed = 0;
				sfpsActivated = true;

			} else if (!world.slowFPSActived && sfpsActivated == true) {

				animation ["ClawTrap"].speed = 1;
				sfpsActivated = false;
			}

			if (sfpsActivated && world.doUpdate) {
				AnimationState animState = animation ["ClawTrap"];
				float time = animState.time;
				time += world.lag;
				animState.time = time;
				animState.speed = 0;
			}
		}

	}
}
