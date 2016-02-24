using UnityEngine;
using System.Collections;

public class TextureStealerBulletScript : TextureBullet {

	public TextureSwapper textureSwapper;
		
	void OnCollisionEnter(Collision col) {

		//Read the renderer.
		Renderer renderer = col.collider.GetComponentInParent<Renderer>();

		//If it exists, take the material and give it to the texture swapper
		if (renderer != null) {
			textureSwapper.actualTexture = renderer.sharedMaterial;
		}

		gameObject.SetActive(false);
	}
}
