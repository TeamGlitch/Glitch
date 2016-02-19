﻿using UnityEngine;
using System.Collections;

public class TexturePainterBulletScript : MonoBehaviour {

	//public terrainType type;
	public Vector3 speed;
	public Material paintMaterial;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		this.transform.position += speed * Time.deltaTime;

		//If it's out of the screen, delete
		if (Camera.main.WorldToViewportPoint (this.transform.position).x > 1) {
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter(Collision col) {

		//Read the renderer.
		Renderer renderer = col.collider.GetComponentInParent<Renderer>();

		//If it exists, take the material and give it to the texture swapper
		if (renderer != null) {
			renderer.sharedMaterial = paintMaterial;
		}

		Destroy (gameObject);
	}

}
