using UnityEngine;
using System.Collections;

public class EnableNextPart : MonoBehaviour {

	public GameObject nextPart;

	void OnTriggerExit(Collider collider)
	{
		if (collider.tag == "Player")
			nextPart.SetActive (true);
	}
}
