using UnityEngine;
using System.Collections;

public class CollectibleScript : MonoBehaviour {

	public Player player;
	private bool picked = false;

	void OnTriggerEnter(Collider collider)
	{
		player.IncreaseItem ();
		gameObject.SetActive (false);
	}
}
