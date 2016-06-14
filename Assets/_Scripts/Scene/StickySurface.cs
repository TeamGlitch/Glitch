using UnityEngine;
using System.Collections;

public class StickySurface : MonoBehaviour {

	public Vector3 directionStick;

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			other.gameObject.GetComponent<PlayerController>().getSticked(directionStick);
		}
	}
}
