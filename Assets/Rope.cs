using UnityEngine;
using System.Collections;

public class Rope : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnTriggerEnter (Collider collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			collision.gameObject.transform.parent.parent = transform;
			PlayerController pc = collision.gameObject.transform.parent.GetComponent<PlayerController>();
			pc.state = PlayerController.player_state.ROPE;
			pc.ropeAttached = GetComponent<Rigidbody>();
		}
	}

	/*void OnTriggerExit (Collider collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			collision.gameObject.transform.parent.parent = null;
		}
	}*/
}
