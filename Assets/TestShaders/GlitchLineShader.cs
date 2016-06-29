using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;


public class GlitchLineShader : ImageEffectBase
{

    public bool active = false;

    private float random;
    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (active)
        {
            random = Random.Range(0.0f, 1.0f);
            material.SetFloat("yPercentage", random);
        }
        else
        {
            material.SetFloat("yPercentage", -1f);
        }
        Graphics.Blit(source, destination, material);
    }

}
