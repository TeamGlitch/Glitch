using UnityEngine;
using System.Collections;

public class TexturePainterBulletScript : TextureBullet {

	public Material paintMaterial;

	void OnCollisionEnter(Collision col) {

		//Read the renderer.
		Renderer renderer = col.collider.GetComponentInParent<Renderer>();

		//If it exists, take the material and give it to the texture swapper
		if (renderer != null) {
			renderer.sharedMaterial = paintMaterial;
		}

		gameObject.SetActive(false);
	}

}
