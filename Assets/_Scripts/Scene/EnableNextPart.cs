using UnityEngine;
using System.Collections;

public class EnableNextPart : MonoBehaviour {

	public GameObject nextPart;

	void OnTriggerExit(Collider collider)
	{
		if (collider.tag == "Player" && collider.transform.position.x > transform.position.x)
			nextPart.SetActive (true);
	}
}
