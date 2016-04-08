using UnityEngine;
using System.Collections;

public class SpringPlantScript : MonoBehaviour {

	public PlayerController playerController;
	public float forceToPlayerWhenEnter = 50.0f;

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			playerController.vSpeed = forceToPlayerWhenEnter;
			playerController.RestartTeleportCooldown ();
		}
	}

}
