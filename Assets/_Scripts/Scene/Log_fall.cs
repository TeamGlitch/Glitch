using UnityEngine;
using System.Collections;

public class Log_fall : MonoBehaviour {

	public World world;
	private Transform log;
	private Collider deathCollider;
	private float endY;

	public float speed = 40.0f;

	// Use this for initialization
	void Start () {
		log = transform.GetChild(0);
		deathCollider = log.GetChild(0).gameObject.GetComponent<Collider> ();
		endY = transform.GetChild(1).localPosition.y;
		Object.Destroy(transform.GetChild(1).gameObject);
	}

	// Update is called once per frame
	void Update () {

		if (world.slowFPSActived && deathCollider.enabled) {
			deathCollider.enabled = false;
		} else if (!world.slowFPSActived && !deathCollider.enabled) {
			deathCollider.enabled = true;
		}

		if (world.doUpdate) {
			if (log.localPosition.y <= endY) {
				log.localPosition = new Vector3(0,0);
				log.rotation = Quaternion.Euler(new Vector3(0, 0,  Random.Range (90 - 20, 90 + 20)));
			}

			log.Translate (new Vector3 (0f, -speed * world.lag), Space.World);
		}
	}

	void OnTriggerEnter (Collider collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			collision.gameObject.transform.parent.parent = log;
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
