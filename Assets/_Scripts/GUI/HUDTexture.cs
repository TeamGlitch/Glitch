using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDTexture : MonoBehaviour {
    public RawImage fill;

	public void AssignTexture (Material tex) {
        fill.texture = tex.mainTexture;
        fill.color = Color.white;
	}
}
