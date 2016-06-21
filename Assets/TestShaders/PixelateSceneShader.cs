using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class PixelateSceneShader : ImageEffectBase
{
    Texture2D texture;

	public float pixelSize = 0.005f;

    // Called by camera to apply image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //If it does, creates a 2D texture with 1x'divisions'
        //texel size and arbitrary asigns 0 and 2 to glitchy 
        //divisions and 1 to non-glitchy divisions
        texture = new Texture2D(1, 1);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        material.SetTexture("_DispTex", texture);
		material.SetFloat("_PixelSize", pixelSize);

        Graphics.Blit(source, destination, material);
    }

}
