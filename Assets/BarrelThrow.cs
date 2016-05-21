using UnityEngine;
using System.Collections;

public class BarrelThrow : MonoBehaviour {

	private ObjectPool barrelPool;
	public GameObject barrelPrefab;

	private Vector3 initPosition;
	private Quaternion initRotation;
	private Vector3 initScale;
	private float barrelLongitude;

	private float nextBarrel = 0;
	private float numberBarrels = 1;

	private bool stopThrowing = false;

	// Use this for initialization
	void Start () {

		barrelPool = new ObjectPool (barrelPrefab);
		initPosition = transform.GetChild(0).position;
		initRotation = transform.GetChild(0).localRotation;
		initScale = transform.GetChild(0).localScale;
		barrelLongitude = transform.GetChild (0).GetComponent<Collider>().bounds.size.x;
		Destroy(transform.GetChild(0).gameObject);

	}
	
	// Update is called once per frame
	void Update () {

		if (!stopThrowing) {


			if (Time.time > nextBarrel) {

				for (int i = 0; i < numberBarrels; i++) {
					GameObject barrel = barrelPool.getObject();
					barrel.name = "Barrel";
					barrel.transform.parent = transform;
					barrel.transform.position = new Vector3(initPosition.x - (i*barrelLongitude), initPosition.y, initPosition.z);
					barrel.transform.rotation = initRotation;
					barrel.transform.localScale = initScale;
					barrel.GetComponent<Rigidbody> ().velocity = new Vector3 (-20f, 0, 0);
				}

				nextBarrel = Time.time + Random.Range(2.0f, 3.0f);

				float num = Random.Range(0.0f, 100.0f);
				if (num < 65) {
					numberBarrels = 1;
				} if (num < 90) {
					numberBarrels = 2;
				} else {
					numberBarrels = 3;
				}
			}

			if (Camera.main != null) {
				Vector3 position = Camera.main.WorldToViewportPoint (initPosition);
				if (position.x < 1.0f && position.x > 0.0f &&
				   position.y < 1.0f && position.y > 0.0f) {
					stopThrowing = true;
				}
			}

		}
			
	}
}
