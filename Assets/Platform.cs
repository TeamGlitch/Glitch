using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {

	void OnTriggerEnter (Collider collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			collision.gameObject.transform.parent.parent = transform;
		}
	}

	void OnTriggerExit (Collider collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			collision.gameObject.transform.parent.parent = null;
		}
	}
}
