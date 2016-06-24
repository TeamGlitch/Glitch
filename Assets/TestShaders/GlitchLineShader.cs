using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;


public class GlitchLineShader : ImageEffectBase
{

    public bool active = false;

    private float _random;
    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (active)
        {
            _random = Random.Range(0.0f, 1.0f);
            material.SetFloat("yPercentage", _random);
        }
        else
        {
            material.SetFloat("yPercentage", -1f);
        }
        Graphics.Blit(source, destination, material);
    }

}
