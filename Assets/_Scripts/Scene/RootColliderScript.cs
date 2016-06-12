using UnityEngine;
using System.Collections;

public class RootColliderScript : MonoBehaviour {

    public RootScript root;
	
	void OnTriggerStay(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            root.enabled = true;
        }
    }
}
