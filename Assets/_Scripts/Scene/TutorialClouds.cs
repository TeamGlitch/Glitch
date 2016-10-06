using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialClouds : MonoBehaviour {

    private enum distortionEffect
    {
        NONE,
        LINE_SHOW,
        WAVY_SHOW,
        PLAINMOVE,
        FLICKER,
        RUN_AND_COLLAPSE,
        TREMBLE,
        INCONSISTENCE
    }

    public RawImage backLayer;
    public RawImage frontLayer;
    public RawImage plainDistortion;
    public RawImage wavyDistortion;

	public Color[] colors;

	private int actualColor = 0;
	private float colorStart;
	private bool colorChanging;

    private bool extending = true;
    private float timeExtendingStarted = 0;

    private distortionEffect distorsion = distortionEffect.NONE;
    private float effectBegin = 0;
    private float effectEnd = 0;
    private float nextSubEnd;

    private Color mainEffectColor = new Color(0f, 0f, 0f, 0.41f);
    private Color alternativeEffectColor = new Color(0.70f, 0.70f, 0.70f, 0.41f);

    public AudioClip whiteMain;
    public AudioClip whiteAlternative;
    private AudioSource source;

	void Start(){
		actualColor = 0;
		frontLayer.color = colors[0];
		colorStart = Time.time;
		colorChanging = false;

        effectEnd = Time.deltaTime + 1.5f;
	}
	
	// Update is called once per frame
	void Update () {

        //Extension
		Rect rect = frontLayer.uvRect;
        float percent = (Time.time - timeExtendingStarted) / 15.0f;

        if (extending)
        {
            if (percent > 1)
            {
                percent = 1;
                timeExtendingStarted = Time.time;
                extending = false;
            }

            float value = (1.5227f * Mathf.Pow(percent, 3)) - (2.284f * Mathf.Pow(percent, 2)) + (0.2113f * percent) + 1;
            rect.height = value;
            rect.width = value;

        }
        else
        {

            if (percent > 1)
            {
                percent = 1;
                timeExtendingStarted = Time.time;
                extending = true;
            }

            percent = 1 - percent;
            float value = (1.5227f * Mathf.Pow(percent, 3)) - (2.284f * Mathf.Pow(percent, 2)) + (0.2113f * percent) + 1;
            rect.height = value;
            rect.width = value;
        }
		rect.x += 0.1f * Time.deltaTime;
		frontLayer.uvRect = rect;

        //Backlayer movement
        rect = backLayer.uvRect;
        rect.x -= 0.05f * Time.deltaTime;
        backLayer.uvRect = rect;

        //Color changing
		float end;
		if (colorChanging)
		{
			end = colorStart + 2.0f;
			percent = 1 - ((end - Time.time) / 2.0f);

			int nextColor = actualColor + 1;
			if (nextColor == colors.Length)
				nextColor = 0;

			Color trans;
			if (percent < 1.0f) {
				trans = new Color();
				trans.a = colors[nextColor].a;
				trans.r = colors[actualColor].r + ( percent * (colors [nextColor].r - colors [actualColor].r));
				trans.g = colors[actualColor].g + ( percent * (colors [nextColor].g - colors [actualColor].g));
				trans.b = colors[actualColor].b + ( percent * (colors [nextColor].b - colors [actualColor].b));
			}
			else
			{
				trans = colors[nextColor];
				actualColor = nextColor;
				colorStart = Time.time;
				colorChanging = false;
			}

			frontLayer.color = trans;

		}
		else
		{
			end = colorStart + 4.0f;

			if (Time.time >= end)
			{
				colorStart = Time.time;
				colorChanging = true;
			}

		}

        switch (distorsion)
        {
            case distortionEffect.NONE:

                if (Time.time >= effectEnd)
                {
                    float chance = Random.value;
                    int value;

                    effectBegin = Time.time;

                    if (chance < 0.19f)
                        value = 3; //Line_Show
                    else if (chance < 0.38f)
                        value = 4;  //Wavy_show
                    else if (chance < 0.55f)
                        value = 6;  //Inconsistence
                    else if (chance < 0.70f)
                        value = 5;  //Tremble
                    else if (chance < 0.83f)
                        value = 0; // PlainMove
                    else if (chance < 0.93f)
                        value = 1; // Flicker
                    else
                        value = 2; // Run_And_Collapse

                    switch (value)
                    {
                        case 2:
                            distorsion = distortionEffect.RUN_AND_COLLAPSE;
                            rect = wavyDistortion.uvRect;
                            rect.height = 0.35f;
                            wavyDistortion.uvRect = rect;
                            effectEnd = Time.time + Random.Range(2.0f, 3.0f);
                            break;
                        case 1:
                            distorsion = distortionEffect.FLICKER;
                            nextSubEnd = 0;
                            effectEnd = Time.time + Random.Range(0.7f, 1f);
                            break;
                        case 0:
                            distorsion = distortionEffect.PLAINMOVE;
                            plainDistortion.enabled = true;
                            effectEnd = Time.time + Random.Range(0.5f, 1.5f);
                            break;
                        case 4:
                            distorsion = distortionEffect.WAVY_SHOW;
                            wavyDistortion.enabled = true;
                            rect = wavyDistortion.uvRect;
                            rect.y = 0;
                            wavyDistortion.uvRect = rect;
                            effectEnd = Time.time + Random.Range(0.5f, 1.5f);
                            break;
                        case 3:
                            distorsion = distortionEffect.LINE_SHOW;
                            plainDistortion.enabled = true;
                            rect = plainDistortion.uvRect;
                            rect.y = 0;
                            plainDistortion.uvRect = rect;
                            effectEnd = Time.time + Random.Range(0.5f, 1.5f);
                            break;
                        case 5:
                            distorsion = distortionEffect.TREMBLE;
                            plainDistortion.enabled = true;
                            nextSubEnd = 0;
                            effectEnd = Time.time + Random.Range(1f, 2.6f);
                            break;
                        case 6:
                            distorsion = distortionEffect.INCONSISTENCE;
                            effectEnd = Time.time + Random.Range(1f, 2.2f);
                            break;

                    }
                }

                break;

            case distortionEffect.LINE_SHOW:

                KeepPlaying();

                if (Time.time > effectEnd)
                {
                    distorsion = distortionEffect.NONE;
                    plainDistortion.enabled = false;
                    source.Stop();
                    effectEnd = Time.time + Random.Range(4f, 8f);
                }
                break;

            case distortionEffect.WAVY_SHOW:

                KeepPlaying(false);

                if (Time.time > effectEnd)
                {
                    distorsion = distortionEffect.NONE;
                    wavyDistortion.enabled = false;
                    source.Stop();
                    effectEnd = Time.time + Random.Range(4f, 8f);
                }
                break;

            case distortionEffect.TREMBLE:

                KeepPlaying();

                if (Time.time < effectEnd)
                {
                    if (Time.time > nextSubEnd)
                    {
                        rect = plainDistortion.uvRect;
                        rect.y = Mathf.Sin(Time.time * 150f) * 0.008f;
                        plainDistortion.uvRect = rect;
                        nextSubEnd = Time.time + 0.07f;
                    }
                }
                else
                {
                    distorsion = distortionEffect.NONE;
                    plainDistortion.enabled = false;
                    source.Stop();
                    effectEnd = Time.time + Random.Range(4f, 8f);
                }
                break;

            case distortionEffect.INCONSISTENCE:

                if(wavyDistortion.enabled)
                    KeepPlaying(false);
                else
                    KeepPlaying();

                if (Time.time < effectEnd)
                {
                    if (Time.time > nextSubEnd)
                    {
                        Color color;
                        if (Random.value > 0.65f)
                        {
                            color = alternativeEffectColor;
                            StartPlaying(false);
                        }
                        else
                        {
                            color = mainEffectColor;
                            StartPlaying();
                        }

                        RawImage image;
                        if (Random.value > 0.5f)
                        {
                            image = plainDistortion;
                            wavyDistortion.enabled = false;

                        }
                        else
                        {
                            image = wavyDistortion;
                            plainDistortion.enabled = false;
                        }

                        rect = image.uvRect;
                        rect.y = Random.Range(-0.5f, 0.5f);
                        image.uvRect = rect;

                        image.color = color;
                        image.enabled = true;

                        nextSubEnd = Time.time + ((effectEnd - effectBegin)/5);
                    }
                }
                else
                {
                    distorsion = distortionEffect.NONE;

                    plainDistortion.enabled = false;
                    plainDistortion.color = mainEffectColor;

                    wavyDistortion.enabled = false;
                    wavyDistortion.color = mainEffectColor;

                    source.Stop();
                    effectEnd = Time.time + Random.Range(4f, 8f);
                }
                break;

            case distortionEffect.PLAINMOVE:

                KeepPlaying();

                if (Time.time < effectEnd)
                {
                    rect = plainDistortion.uvRect;
                    rect.y += 0.5f * Time.deltaTime;
                    plainDistortion.uvRect = rect;
                }
                else
                {
                    distorsion = distortionEffect.NONE;
                    plainDistortion.enabled = false;
                    source.Stop();
                    effectEnd = Time.time + Random.Range(4f, 8f);
                }
                break;

            case distortionEffect.FLICKER:

                KeepPlaying();

                if (Time.time < effectEnd)
                {
                    if (Time.time > nextSubEnd)
                    {
                        if (!plainDistortion.enabled)
                        {
                            plainDistortion.enabled = true;
                            if (Random.value > 0.5f)
                            {
                                plainDistortion.color = mainEffectColor;
                                StartPlaying();
                            }
                            else
                            {
                                plainDistortion.color = alternativeEffectColor;
                                StartPlaying(false);
                            }
                        }
                        else
                        {
                            plainDistortion.enabled = false;
                            source.Stop();
                        }

                        nextSubEnd = Time.time + ((effectEnd - effectBegin) / 5);
                    }
                }
                else
                {
                    distorsion = distortionEffect.NONE;
                    plainDistortion.color = mainEffectColor;
                    plainDistortion.enabled = false;
                    source.Stop();
                    effectEnd = Time.time + Random.Range(4f, 8f);
                }
                break;

            case distortionEffect.RUN_AND_COLLAPSE:

                KeepPlaying();

                if (Time.time < effectEnd)
                {
                    percent = (Time.time - effectBegin) / (effectEnd - effectBegin);

                    rect = plainDistortion.uvRect;
                    rect.y += 2.2f * (1 - percent) * Time.deltaTime;
                    rect.height += 2.25f * percent * Time.deltaTime;
                    plainDistortion.uvRect = rect;

                    if (percent < 0.85f || (percent > 0.90f && percent < 0.95f))
                        plainDistortion.enabled = true;
                    else
                        plainDistortion.enabled = false;
                }
                else
                {
                    distorsion = distortionEffect.NONE;
                    rect = plainDistortion.uvRect;
                    rect.height = 1f;
                    plainDistortion.uvRect = rect;
                    plainDistortion.enabled = false;
                    source.Stop();
                    effectEnd = Time.time + Random.Range(4f, 8f);
                }
                break;
        }
	}

    private void StartPlaying(bool main = true)
    {
        if (main)
        {
            if (source == null || source.clip != whiteMain || !source.isPlaying)
            {
                source.Stop();
                source = SoundManager.instance.PlaySingle(whiteMain);
            }
        }
        else
        {
            if (source == null || source.clip != whiteAlternative || !source.isPlaying)
            {
                source.Stop();
                source = SoundManager.instance.PlaySingle(whiteAlternative);
            }
        }
    }

    private void KeepPlaying(bool main = true)
    {
        if (main)
        {
            if (source == null || source.clip != whiteMain || !source.isPlaying)
                source = SoundManager.instance.PlaySingle(whiteMain);
        }
        else
        {
            if (source == null || source.clip != whiteAlternative || !source.isPlaying)
                source = SoundManager.instance.PlaySingle(whiteAlternative);
        }
    }
}
