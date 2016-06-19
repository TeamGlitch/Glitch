using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class GlitchSceneShader : ImageEffectBase
{
    Texture2D texture;
    public int divisions = 100;
    public Transform _playerTransform;
    private float _maxXPos;

    private float correctionMarkerWidth = (-13.59751f * 11.02f);
    private float correctionMarkerHeight = (-13.59751f * 14.9f);
    private float correctionDueToAspect = 1.777605f;

	public float pixelSize = 0.005f;
	public float startGlitchPixelationOffset = 0.15f;

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
		Vector3 maxPos = new Vector3(_maxXPos, _playerTransform.position.y, _playerTransform.position.z);
        Vector2 percentageMaxPosition = CalculatebGlitchPositionsInPercentage(maxPos);
        Vector2 percentagePlayerPosition = CalculatebGlitchPositionsInPercentage(_playerTransform.position);
		
        //If it does, creates a 2D texture with 1x'divisions'
        //texel size and arbitrary asigns 0 and 2 to glitchy 
        //divisions and 1 to non-glitchy divisions
        texture = new Texture2D(1, 1);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        material.SetTexture("_DispTex", texture);
        material.SetFloat("_PercentagePixel", percentageMaxPosition.x - startGlitchPixelationOffset);
        material.SetFloat("_PlayerPercentagePixelX", percentagePlayerPosition.x);
        material.SetFloat("_PlayerPercentagePixelY", percentagePlayerPosition.y);
		material.SetFloat("_PixelSize", pixelSize);

        Graphics.Blit(source, destination, material);
    }

	private Vector2 CalculatebGlitchPositionsInPercentage(Vector3 posToCalculate)
    {
        Vector3 camPosition = Camera.main.WorldToScreenPoint(posToCalculate);
		Vector2 percentagePositionInCamera;       
		camPosition.x *= guiRectTrans.rect.width / Camera.main.pixelWidth;
        camPosition.y *= guiRectTrans.rect.height / Camera.main.pixelHeight;
        percentagePositionInCamera.x = camPosition.x / guiRectTrans.rect.width;
        percentagePositionInCamera.y = camPosition.y / guiRectTrans.rect.height;
		return percentagePositionInCamera;
    }
}
