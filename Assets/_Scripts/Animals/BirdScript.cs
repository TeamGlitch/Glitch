using UnityEngine;
using System.Collections;

public class BirdScript : MonoBehaviour {

	//External references
	public World world;
	public Transform player;

	//Internal references
	Animator anim;

	Vector3 flySpeed;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

		//Fly away
		if (stateInfo.IsName ("fly") && world.doUpdate) {

			transform.position += flySpeed * world.lag;

			//If it's out of the screen, destroy
			Vector3 screenPos = Camera.main.WorldToViewportPoint(this.transform.position);
			if ((screenPos.x > 1) || (screenPos.x < 0)
				|| (screenPos.y > 1) || (screenPos.y < 0)) {
				GameObject.Destroy(gameObject);
			}
				
		} else if (Vector3.Distance(transform.position, player.position) < 7.0f) {

			//If the player is too close, fly away in the other direction
			anim.SetBool ("Fly", true);

			int flyDirection;
			if (player.position.x < transform.position.x)
			{
				flyDirection = 1;
			}
			else
			{
				flyDirection = -1;
			}

			flySpeed = new Vector3 (flyDirection, 1.0f, 0) * 10.0f;
		}
	}
}