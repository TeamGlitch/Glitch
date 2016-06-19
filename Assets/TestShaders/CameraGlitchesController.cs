using UnityEngine;
using System.Collections;

public class CameraGlitchesController : MonoBehaviour {

    private BlackAndWhiteShaderScript _blackAndWhiteOne;
    private BlackAndWhiteShaderScript2 _blackAndWhiteTwo;
    private InvertColorsSceneShader _invertColorsScene;
    private PixelateSceneShader _pixelateScene;

    private GlitchLineShader _glitchLine;

    public float timeWithGlitchedScreen = 3.0f;
    public float timeWithLineGlitchScreen = 2.0f;
    public float timeBetweenGlitches = 60.0f;

    private int _random;

    // Use this for initialization
    void Start () {
        _blackAndWhiteOne = transform.GetComponent<BlackAndWhiteShaderScript>();
        _blackAndWhiteTwo = transform.GetComponent<BlackAndWhiteShaderScript2>();
        _invertColorsScene = transform.GetComponent<InvertColorsSceneShader>();
        _pixelateScene = transform.GetComponent<PixelateSceneShader>();
        _glitchLine = transform.GetComponent<GlitchLineShader>();
        Invoke("StartGlitchScreen", timeBetweenGlitches);
    }

    public void StartGlitchScreen()
    {
        _glitchLine.enabled = true;
        Invoke("ContinueGlitchScreen", timeWithLineGlitchScreen);
    }

    public void ContinueGlitchScreen()
    {
        _random = Random.Range(1, 5);
        _glitchLine.enabled = false;
        switch(_random)
        {
            case 1:
                _blackAndWhiteOne.enabled = true;
                break;
            case 2:
                _blackAndWhiteTwo.enabled = true;
                break;
            case 3:
                _invertColorsScene.enabled = true;
                break;
            case 4:
                _pixelateScene.enabled = true;
                break;
        }
        Invoke("StartStopGlitchScreen", timeWithGlitchedScreen);
    }

    public void StartStopGlitchScreen()
    {
        _glitchLine.enabled = true;
        Invoke("StopGlitchScreen", timeWithLineGlitchScreen);
    }

    public void StopGlitchScreen()
    {
        _glitchLine.enabled = false;
        switch (_random)
        {
            case 1:
                _blackAndWhiteOne.enabled = false;
                break;
            case 2:
                _blackAndWhiteTwo.enabled = false;
                break;
            case 3:
                _invertColorsScene.enabled = false;
                break;
            case 4:
                _pixelateScene.enabled = false;
                break;
        }
        Invoke("StartGlitchScreen", timeBetweenGlitches);
    }

}
