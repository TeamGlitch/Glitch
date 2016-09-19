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

    private noisephase phase = noisephase.WAIT_DOWN;
    private float nextPhase;

	// Use this for initialization
	void Start () {
        noiseAndGrain = GetComponent<NoiseAndGrain>();
	}
	
	// Update is called once per frame
	void Update () {
        switch(phase)
        {
            case noisephase.GROW:
                break;

            case noisephase.WAIT_UP:
                break;

            case noisephase.NARROW:
                break;

            case noisephase.WAIT_DOWN:
                break;
        }
	}
}
