using UnityEngine;
using System.Collections;

public class WhipManager : MonoBehaviour {

	public GameObject chainPrefab;
	public float distanceToWhip;

	private GameObject chain;

	private GameObject player;
	private GameObject[] whipObjects;
	private PlayerController playerController;
	private CharacterJoint characterJoint;

	private bool activated;

	private Vector3 lastPosition;
	private Vector3 posToWhip;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		whipObjects = GameObject.FindGameObjectsWithTag ("WhipObject");

		chain = Instantiate (chainPrefab);
		chain.SetActive (false);

		playerController = player.GetComponent<PlayerController> ();

		posToWhip = Vector3.zero;
		activated = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (activated) {
			lastPosition = player.transform.position;
			float angle = Mathf.Atan2 (posToWhip.y - player.transform.position.y, posToWhip.x - player.transform.position.x) * Mathf.Rad2Deg + 180.0f;
			float angleInclinacion = angle + 90.0f;
			chain.transform.rotation =  Quaternion.Euler(0.0f, 0.0f, angleInclinacion);

			if (Input.GetButton ("Jump")) 
			{
				chain.SetActive (false);
				Destroy (characterJoint);
				player.transform.position = lastPosition;
				playerController.EndWhip ();
				activated = false;
			} 
		}
		else {
			if (Input.GetKeyDown (KeyCode.Z)) {
				int closerWhipObject = 0;
				Vector3 playerPosition = gameObject.transform.position;
				for (int i = 1; i < whipObjects.Length; ++i) 
				{
					if (Vector3.Distance (playerPosition, whipObjects[closerWhipObject].transform.position) > Vector3.Distance (playerPosition, whipObjects[i].transform.position))
					{
						closerWhipObject = i;
					}
				}

				posToWhip = whipObjects [closerWhipObject].transform.position;

				if (Vector3.Distance (playerPosition, posToWhip) <= distanceToWhip) 
				{

					chain.SetActive (true);
					chain.transform.position = posToWhip;

					characterJoint = player.AddComponent <CharacterJoint> ();
					characterJoint.connectedBody = whipObjects [closerWhipObject].GetComponent<Rigidbody> ();

					playerController.StartWhip();
					activated = true;
				}		
			}
		}
	}
}
