using UnityEngine;
using System.Collections;

public class PixelateObject : MonoBehaviour {

    private Shader shader1;
    public Shader shader2;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        shader1 = Shader.Find("Diffuse");
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            if (rend.material.shader == shader1)
                rend.material.shader = shader2;
            else
                rend.material.shader = shader1;

    }

}
