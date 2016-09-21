using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class GlitchWorldNoiseController : MonoBehaviour {

    private enum noisephase
    {
        GROW,
        WAIT_UP,
        NARROW,
        WAIT_DOWN
    }

    private NoiseAndGrain noiseAndGrain;
    private ScreenSpaceAmbientOcclusion occlusion;
    private BloomOptimized bloom;

    private noisephase phase = noisephase.WAIT_DOWN;
    private float nextPhase = 0;

    private int actualQuality;

	// Use this for initialization
	void Start () {
        noiseAndGrain = GetComponent<NoiseAndGrain>();
        occlusion = GetComponent<ScreenSpaceAmbientOcclusion>();
        bloom = GetComponent<BloomOptimized>();

        qualityChanges();
	}
	
	// Update is called once per frame
	void Update () {

        if (actualQuality != QualitySettings.GetQualityLevel())
            qualityChanges();

        switch(phase)
        {
            case noisephase.GROW:
                if (noiseAndGrain.intensityMultiplier < 1.5f)
                    noiseAndGrain.intensityMultiplier += 0.5f * Time.deltaTime;
                else
                {
                    nextPhase = Time.time + Random.Range(1.0f, 5.0f);
                    phase = noisephase.WAIT_UP;
                }
                break;

            case noisephase.WAIT_UP:
                if (Time.time >= nextPhase)
                    phase = noisephase.NARROW;
                break;

            case noisephase.NARROW:
                float value = noiseAndGrain.intensityMultiplier - (0.5f * Time.deltaTime);
                if (value > 0)
                    noiseAndGrain.intensityMultiplier = value;
                else
                {
                    nextPhase = Time.time + Random.Range(2.0f, 10.0f);
                    phase = noisephase.WAIT_DOWN;
                }
                break;

            case noisephase.WAIT_DOWN:
                if (Time.time >= nextPhase)
                    phase = noisephase.GROW;
                break;
        }
	}

    private void qualityChanges()
    {
        int quality = QualitySettings.GetQualityLevel();
        
        if (quality > 0)
            bloom.enabled = true;
        else
            bloom.enabled = false;

        if (quality == 0)
            occlusion.m_SampleCount = ScreenSpaceAmbientOcclusion.SSAOSamples.Low;
        else if (quality == 1)
            occlusion.m_SampleCount = ScreenSpaceAmbientOcclusion.SSAOSamples.Medium;
        else
            occlusion.m_SampleCount = ScreenSpaceAmbientOcclusion.SSAOSamples.High;

        actualQuality = quality;

    }
}
