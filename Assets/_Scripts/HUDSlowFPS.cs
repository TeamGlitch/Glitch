using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDSlowFPS : MonoBehaviour {
    public Slider bar;
    public Image barFill;
    public SlowFPS slowFPS;

    private float maxTime = 10.0f;
    private Color maxColor;
    private Color minColor;

    void Start()
    {
        maxColor = barFill.color;
        minColor = new Vector4(0.5f, 0.0f, 0.0f, 1.0f);
    }

	// Update is called once per frame
	void Update () {
        bar.value = slowFPS.timeRemaining;

        // 1.2 is a constant to delay the change of color
        barFill.color = Color.Lerp(minColor, maxColor, (bar.value / maxTime) * 1.2f);
	}
}
