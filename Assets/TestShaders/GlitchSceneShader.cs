using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class GlitchSceneShader : ImageEffectBase
{
    Texture2D texture;
    public Transform _playerTransform;
	private Player _playerScript;
    private float _maxXPos;

	public float pixelSize = 0.005f;

    public Canvas gui;
    private RectTransform guiRectTrans;

    // Use this for initialization
    void Start()
    {
        _maxXPos = _playerTransform.transform.position.x;
		_playerScript = _playerTransform.GetComponent<Player>();
        guiRectTrans = gui.GetComponent<RectTransform>();
    }

    // Called by camera to apply image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
		_maxXPos = _playerScript.lastCheckPoint.transform.position.x;
		Vector3 maxPos = new Vector3(_maxXPos, _playerTransform.position.y, _playerTransform.position.z);
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
