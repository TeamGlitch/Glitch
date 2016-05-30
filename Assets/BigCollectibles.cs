using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BigCollectibles : MonoBehaviour {

    private const float timeIn = 1.0f;
    private const float timeOut = 0.8f;

    public Image icon1;
    public Image icon2;
    public Image icon3;
    public Image frame;

    private float timeFadeIn = timeIn;
    private float timeFadeOut;
    private bool fadeOut = false;

    public void FadeOut()
    {
        timeFadeOut = timeOut;
        fadeOut = true;
        icon1.CrossFadeAlpha(0.0f, timeOut, false);
        icon2.CrossFadeAlpha(0.0f, timeOut, false);
        icon3.CrossFadeAlpha(0.0f, timeOut, false);
        frame.CrossFadeAlpha(0.0f, timeOut, false);
    }

    void Update()
    {
        timeFadeIn -= Time.deltaTime;
        if (timeFadeIn <= 0.0f && fadeOut == false)
        {
            FadeOut();
        }
        else if(fadeOut == true)
        {
            timeFadeOut -= Time.deltaTime;
            if (timeFadeOut <= 0.0f)
            {
                fadeOut = false;
                timeFadeIn = timeIn;
                this.gameObject.SetActive(false);
            }
        }
    }
}
