using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialClouds : MonoBehaviour {

    //public RawImage backLayer;
    public RawImage frontLayer;
	
	// Update is called once per frame
	void Update () {
        Rect rect = frontLayer.uvRect;
        rect.x += 0.2f * Time.deltaTime;
        frontLayer.uvRect = rect;
	}
}
