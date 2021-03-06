﻿using UnityEngine;
using System.Collections;
using InControl;

public class Whip : MonoBehaviour {
/*
	public GameObject chainPrefab;
	public float distanceToWhip;
	public GameObject sphereRotatorPrefab;

	private GameObject sphereRotator;
	private Rigidbody sphereRotatorRigidbody;
	private GameObject chain;

	private GameObject player;
	private GameObject[] whipObjects;
	private PlayerController playerController;
	private CharacterJoint characterJoint;
	private GameObject[] chainPieces;

	private bool activated;

	private Vector3 lastPosition;
	private Vector2 posToWhip;
	private int previousDeActivated;

	// Use this for initialization
	void Start () {

		sphereRotator = Instantiate (sphereRotatorPrefab);
		sphereRotator.SetActive (false);

		sphereRotatorRigidbody = sphereRotator.GetComponent<Rigidbody> ();

		chain = Instantiate (chainPrefab);
		chainPieces = new GameObject[13];

		chainPieces [0] = GameObject.Find ("ChainOnlySprites(Clone)/Chain1");
		chainPieces [1] = GameObject.Find ("ChainOnlySprites(Clone)/Chain1/Chain2");
		chainPieces [2] = GameObject.Find ("ChainOnlySprites(Clone)/Chain1/Chain2/Chain3");
		chainPieces [3] = GameObject.Find ("ChainOnlySprites(Clone)/Chain1/Chain2/Chain3/Chain4");
		chainPieces [4] = GameObject.Find ("ChainOnlySprites(Clone)/Chain1/Chain2/Chain3/Chain4/Chain5");
		chainPieces [5] = GameObject.Find ("ChainOnlySprites(Clone)/Chain1/Chain2/Chain3/Chain4/Chain5/Chain6");
		chainPieces [6] = GameObject.Find ("ChainOnlySprites(Clone)/Chain1/Chain2/Chain3/Chain4/Chain5/Chain6/Chain7");
		chainPieces [7] = GameObject.Find ("ChainOnlySprites(Clone)/Chain1/Chain2/Chain3/Chain4/Chain5/Chain6/Chain7/Chain8");
		chainPieces [8] = GameObject.Find ("ChainOnlySprites(Clone)/Chain1/Chain2/Chain3/Chain4/Chain5/Chain6/Chain7/Chain8/Chain9");
		chainPieces [9] = GameObject.Find ("ChainOnlySprites(Clone)/Chain1/Chain2/Chain3/Chain4/Chain5/Chain6/Chain7/Chain8/Chain9/Chain10");
		chainPieces [10] = GameObject.Find ("ChainOnlySprites(Clone)/Chain1/Chain2/Chain3/Chain4/Chain5/Chain6/Chain7/Chain8/Chain9/Chain10/Chain11");
		chainPieces [11] = GameObject.Find ("ChainOnlySprites(Clone)/Chain1/Chain2/Chain3/Chain4/Chain5/Chain6/Chain7/Chain8/Chain9/Chain10/Chain11/Chain12");
		chainPieces [12] = GameObject.Find ("ChainOnlySprites(Clone)/Chain1/Chain2/Chain3/Chain4/Chain5/Chain6/Chain7/Chain8/Chain9/Chain10/Chain11/Chain12/Chain13");

		player = GameObject.Find ("Player");
		whipObjects = GameObject.FindGameObjectsWithTag ("WhipObject");

		chain.SetActive (false);

		playerController = player.GetComponent<PlayerController> ();

		posToWhip = Vector3.zero;
		activated = false;

		previousDeActivated = 0;
	}

	// Update is called once per frame
	void Update () {
		if (activated) {
			lastPosition = player.transform.position;
			float angle = Mathf.Atan2 (posToWhip.y - player.transform.position.y, posToWhip.x - player.transform.position.x) * Mathf.Rad2Deg + 180.0f;
			float angleInclinacion = angle + 90.0f;
			chain.transform.rotation =  Quaternion.Euler(0.0f, 0.0f, angleInclinacion);

			if (InputManager.ActiveDevice.Action1) 
			{
				chainPieces [previousDeActivated].SetActive (true);
				chain.SetActive (false);
				Destroy (characterJoint);
				player.transform.position = lastPosition;
				playerController.EndWhip ();
				activated = false;
				sphereRotator.SetActive (false);
			} 
		}
		else {
			if (InputManager.ActiveDevice.Action4.WasPressed) {
				int closerWhipObject = 0;
				Vector2 playerPosition = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
				for (int i = 1; i < whipObjects.Length; ++i) 
				{
					if (Vector2.Distance (playerPosition, new Vector2(whipObjects[closerWhipObject].transform.position.x, whipObjects[closerWhipObject].transform.position.y)) > Vector2.Distance (playerPosition, new Vector2(whipObjects[i].transform.position.x, whipObjects[i].transform.position.y)))
					{
						closerWhipObject = i;
					}
				}

				posToWhip.x = whipObjects [closerWhipObject].transform.position.x;
				posToWhip.y = whipObjects [closerWhipObject].transform.position.y;

				if (Vector2.Distance (playerPosition, posToWhip) <= distanceToWhip) 
				{

					sphereRotator.SetActive (true);
					sphereRotator.transform.position = posToWhip;

					float angle = Mathf.Atan2 (posToWhip.y - player.transform.position.y, posToWhip.x - player.transform.position.x) * Mathf.Rad2Deg + 180.0f;
					float angleInclinacion = angle + 90.0f;
					player.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angleInclinacion);

					chain.SetActive (true);
					Vector3 newChainPos = new Vector3 (posToWhip.x, posToWhip.y, player.transform.position.z);
					chain.transform.position = newChainPos;
					chain.transform.rotation =  Quaternion.Euler(0.0f, 0.0f, angleInclinacion);

					characterJoint = player.AddComponent <CharacterJoint> ();
					//					characterJoint.connectedBody = whipObjects [closerWhipObject].GetComponent<Rigidbody> ();
					characterJoint.connectedBody = sphereRotatorRigidbody;
					playerController.StartWhip();
					activated = true;

					float totalDistance = Vector2.Distance(playerPosition, posToWhip);

					int chainsNeeded = Mathf.CeilToInt(totalDistance / 0.5f);

					if (chainsNeeded < chainPieces.Length)
					{
						chainPieces [chainsNeeded].SetActive (false);
						previousDeActivated = chainsNeeded;
					}


				}		
			}
		}
	}*/
}
