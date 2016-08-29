using UnityEngine;
using System.Collections;
using InControl;

public class GlitchZone : MonoBehaviour {

    public enum statesOfGlitch
    {
        NORMAL,
        GLITCH
    };

    public Shader newShader;
    public Player playerScript;

    private statesOfGlitch state = statesOfGlitch.NORMAL;
    private Shader oldShader;
    private MeshRenderer rend;

    void OnTriggerEnter(Collider coll)
    {
        playerScript.IncreaseActivableBox();
    }

    void OnTriggerStay(Collider coll)
    {
        if (state != statesOfGlitch.GLITCH)
        {
            if (InputManager.ActiveDevice.Action4.WasPressed)
            {
                playerScript.DecreaseActivableBox();
                rend.material.shader = newShader;
                state = statesOfGlitch.GLITCH;
                Invoke("TurnOff", 1.0f);
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        playerScript.DecreaseActivableBox();
    }

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        oldShader = rend.material.shader;
        state = statesOfGlitch.NORMAL;
    }

	public void TurnOff()
    {
        rend.material.shader = oldShader;
        gameObject.SetActive(false);
    }
}
