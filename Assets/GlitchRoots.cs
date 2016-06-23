using UnityEngine;
using System.Collections;
using InControl;

public class GlitchRoots : MonoBehaviour {
    public GameObject rootAssociated;
    public Shader newShader;

    private Shader rootShader;
    private Renderer rootRender;

    void Start()
    {
        rootShader = rootAssociated.GetComponent<Shader>();
        rootRender = rootAssociated.GetComponent<Renderer>();
    }

    void OnTriggerStay(Collider coll)
    {
        if (InputManager.ActiveDevice.Action4.WasPressed && coll.CompareTag("Player"))
        {
            rootRender.material.shader = newShader;
        }
    }
}
