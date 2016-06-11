using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class GlitchSceneShader : ImageEffectBase
{
    Texture2D texture;
    public int divisions = 100;

    // Use this for initialization
    void Start()
    {
    }

    // Called by camera to apply image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        //If it does, creates a 2D texture with 1x'divisions'
        //texel size and arbitrary asigns 0 and 2 to glitchy 
        //divisions and 1 to non-glitchy divisions
        texture = new Texture2D(divisions, 1);

        for (int z = 0; z < divisions; z += 1)
        {
            if (z < 20)
            {
                texture.SetPixel(z, 0, new Color32(128, 128, 128, 128));
            }
            else
            {
                texture.SetPixel(z, 0, new Color32(1, 0, 0, 0));
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        material.SetTexture("_DispTex", texture);

        Graphics.Blit(source, destination, material);
    }
}
