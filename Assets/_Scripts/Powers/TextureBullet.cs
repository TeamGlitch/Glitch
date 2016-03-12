using UnityEngine;
using System.Collections;

public class TextureBullet : MonoBehaviour {

	public Vector3 speed;
    public TextureSwapper textureSwapper = null;
    public Material paintMaterial = null;
    public bool painter;

    void OnCollisionEnter(Collision col)
    {

        // Read the renderer
        Renderer renderer = col.collider.GetComponentInParent<Renderer>();

        // If it exists
        if (renderer != null)
        {
            // If is a stealer bullet assign the new texture to actual texture to paint
            // else we paint de new texture
            if (!painter)
            {
                textureSwapper.actualTexture = renderer.sharedMaterial;
                textureSwapper.HUD_tex.AssignTexture(textureSwapper.actualTexture);
            }
            else
            {
                renderer.sharedMaterial = paintMaterial;
            }
        }

        gameObject.SetActive(false);
    }
	
	public void Update () {
        // Bullet movement
		this.transform.position += speed * Time.deltaTime;

		// If it's out of the screen, delete
		Vector3 screenPos = Camera.main.WorldToViewportPoint(this.transform.position);
		if ((screenPos.x > 1) || (screenPos.x < 0)
			|| (screenPos.y > 1) || (screenPos.y < 0)) {
			gameObject.SetActive(false);
		}
	}
}
