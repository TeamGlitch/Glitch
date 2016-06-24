using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class GlitchSceneShader : ImageEffectBase
{
    Texture2D texture;
    public Transform playerTransform;
    private Player playerScript;
    private float maxXPos;
    private GlitchLineShader glitchLineShader;
    public float pixelSize = 0.005f;

    public Canvas gui;
    private RectTransform guiRectTrans;

    public float initialXPos = -375f;

    // Use this for initialization
    void Start()
    {
        maxXPos = initialXPos;
        playerScript = playerTransform.GetComponent<Player>();
        guiRectTrans = gui.GetComponent<RectTransform>();
        glitchLineShader = transform.GetComponent<GlitchLineShader>();
    }

    // Called by camera to apply image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!playerScript.lastCheckPoint.startPoint)
            maxXPos = playerScript.lastCheckPoint.transform.position.x;
        if (maxXPos > playerScript.transform.position.x && !glitchLineShader.active &&
            ((Time.unscaledTime % 20f >= 0f && Time.unscaledTime % 20f <= 3f) || (Time.unscaledTime % 20f >= 8f && Time.unscaledTime % 20f <= 12f)))
            glitchLineShader.active = true;
        else if (maxXPos > playerScript.transform.position.x && glitchLineShader.active &&
            ((Time.unscaledTime % 20f > 3f && Time.unscaledTime % 20f < 8f) || (Time.unscaledTime % 20f > 12f && Time.unscaledTime % 20f < 20f)))
            glitchLineShader.active = false;
        else if (maxXPos < playerScript.transform.position.x && glitchLineShader.active)
            glitchLineShader.active = false;
        Vector3 maxPos = new Vector3(maxXPos, playerTransform.position.y, playerTransform.position.z);
        float percentageMaxPosition = CalculatebGlitchPositionsInPercentage(maxPos);

        //If it does, creates a 2D texture with 1x'divisions'
        //texel size and arbitrary asigns 0 and 2 to glitchy 
        //divisions and 1 to non-glitchy divisions
        texture = new Texture2D(1, 1);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        material.SetTexture("_DispTex", texture);
        material.SetFloat("_PercentagePixel", percentageMaxPosition);
        material.SetFloat("_PixelSize", pixelSize);

        Graphics.Blit(source, destination, material);
    }

    private float CalculatebGlitchPositionsInPercentage(Vector3 posToCalculate)
    {
        Vector3 camPosition = Camera.main.WorldToScreenPoint(posToCalculate);
        float xPercentagePositionInCamera;
        camPosition.x *= guiRectTrans.rect.width / Camera.main.pixelWidth;
        xPercentagePositionInCamera = camPosition.x / guiRectTrans.rect.width;
        return xPercentagePositionInCamera;
    }
}
