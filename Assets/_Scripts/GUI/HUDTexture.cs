using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDTexture : MonoBehaviour {
    public RawImage fill;

    // Function that takes the main texture of the material passed
    // fill.color is to clarify the quad with the texture
	public void AssignTexture (Material tex) {
        fill.texture = tex.mainTexture;
        fill.color = Color.white;
	}
}
