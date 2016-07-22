using UnityEngine;
using System.Collections;

public class DisableParent : MonoBehaviour {

    public GameObject gameobjectToDisable;

	void OnTriggerExit(Collider collider)
	{
		if(collider.tag == "Player")
            gameobjectToDisable.SetActive (false);
	}

}
