using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class GlitchSceneShader : ImageEffectBase
{
    Texture2D texture;
    public int divisions = 100;
    public Transform _playerTransform;
    private float _maxXPos;
    public float _percentagePixel = 0.0f;

    private float correctionMarkerWidth = (-13.59751f * 11.02f);
    private float correctionMarkerHeight = (-13.59751f * 14.9f);
    private float correctionDueToAspect = 1.777605f;

    public Canvas gui;
    private RectTransform guiRectTrans;

    // Use this for initialization
    void Start()
    {
        _maxXPos = _playerTransform.transform.position.x;
        guiRectTrans = gui.GetComponent<RectTransform>();
    }

    // Called by camera to apply image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _maxXPos = Mathf.Max(_maxXPos, _playerTransform.transform.position.x);
        CalculatebPlayerPositionsInPercentage();
        //If it does, creates a 2D texture with 1x'divisions'
        //texel size and arbitrary asigns 0 and 2 to glitchy 
        //divisions and 1 to non-glitchy divisions
        texture = new Texture2D(1, 1);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        material.SetTexture("_DispTex", texture);
        material.SetFloat("_PercentagePixel", _percentagePixel);

        Graphics.Blit(source, destination, material);
    }

    private void CalculatebPlayerPositionsInPercentage()
    {
        Vector3 posToCalculate = _playerTransform.position;
        posToCalculate.x = _maxXPos;
        Vector3 camPosition = Camera.main.WorldToScreenPoint(posToCalculate);
        camPosition.x *= guiRectTrans.rect.width / Camera.main.pixelWidth;
        camPosition.y *= guiRectTrans.rect.height / Camera.main.pixelHeight;
        _percentagePixel = camPosition.x / guiRectTrans.rect.width - 0.15f;
    }
}
