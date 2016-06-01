using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AdvanceBarScript : MonoBehaviour {

    public Transform player;
    public Transform endPoint;
    public Slider slider;
	
	void Start () {
        slider.maxValue = endPoint.position.x;
        slider.minValue = player.position.x;
	}
	
	void Update () {
        slider.value = player.position.x;
	}
}
