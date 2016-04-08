using UnityEngine;
using System.Collections;

public class DisableParent : MonoBehaviour {

	void OnTriggerExit(Collider collider)
	{
		if(collider.tag == "Player" && collider.transform.position.x > transform.position.x)
			transform.parent.gameObject.SetActive (false);
	}

}
