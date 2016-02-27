using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDSlowFPS : MonoBehaviour {
    public Slider bar;
    public SlowFPS slowFPS;

	// Update is called once per frame
	void Update () {
        bar.value = slowFPS.timeRemaining;
	}
}
