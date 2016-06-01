using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BigCollectibles : MonoBehaviour {

    private const float timeIn = 1.0f;
    private const float timeOut = 0.8f;

    public Image[] collects;
    public Sprite collect;
    public Sprite noCollect;
    public Image frame;

    private float timeFadeIn = timeIn;
    private float timeFadeOut;
    private bool fadeOut = false;

    public void FadeOut()
    {
        timeFadeOut = timeOut;
        fadeOut = true;
        for (int i = 0; i < collects.Length; i++)
        {
            collects[i].CrossFadeAlpha(0.0f, timeOut, false);
        }
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

    public void AddItem(int number){
        collects[number].sprite = collect;
    }
}
