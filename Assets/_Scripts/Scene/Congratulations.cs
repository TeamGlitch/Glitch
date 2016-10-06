using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using InControl;

public class Congratulations : MonoBehaviour {
    public enum congratulationsPhase
    {
        INTRO,
        PARTY,
        FIRST_NOISE,
        SECOND_PARTY,
        FINAL_NOISE,
        LOADING
    };

    private bool skip = false;
    private congratulationsPhase phase = congratulationsPhase.INTRO;

    public AudioClip music;
    public Animator glitch;
    public Image black;
    public Image congratulations;
    public Image light;
    public Image[] lights;
    public Image[] roundLights;

    public Sprite[] noiseImgs;
    private int actualNoiseImg = 0;
    public Image noise;
    public AudioClip shortNoise;
    public AudioClip longNoise;
    private AudioSource whiteNoiseSource;

    private int nextChange = 0;
    private int actualDance = -1;

    private float startTime = 0;
    private float lightsOn = 7.890f;
    private float firstDist = 83.757f;
    private float distFin = 91.835f;
    private float[] changes = new float[]{
        11.754f,
        15.815f,
        19.862f,
        23.962f,
        27.857f,
        32.022f,
        35.852f,
        39.914f,
        44.102f,
        49.989f,
        52.877f,
        55.884f,
        59.882f,
        63.816f,
        67.877f,
        71.832f,
        75.858f,
        79.870f,
        83.882f,
    };

    void Start()
    {
        congratulations.enabled = false;
        light.enabled = false;

        SoundManager.instance.musicSource.clip = music;
        SoundManager.instance.musicSource.Play();
        startTime = Time.time;

        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].enabled = false;
        }

        for (int i = 0; i < roundLights.Length; i++)
        {
            roundLights[i].enabled = false;
        }

    }

    void Update()
    {

        if (!skip && InputManager.ActiveDevice.AnyButton.WasPressed)
        {
            skip = true;
            phase = congratulationsPhase.SECOND_PARTY;
        }

        switch (phase)
        {
        
            case congratulationsPhase.INTRO:

                float percentDone = (Time.time - startTime)/lightsOn;

                if(percentDone < 1)
                {
                    float alpha = (255 - (71 * percentDone)) / 255;
                    black.color = new Color(0,0,0,alpha);
                    
                }
                else
                {
                    black.color = new Color32(0,0,0,0);

                    congratulations.enabled = true;
                    light.enabled = true;
                    for (int i = 0; i < lights.Length; i++)
                    {
                        lights[i].enabled = true;
                    }

                    for (int i = 0; i < roundLights.Length; i++)
                    {
                        roundLights[i].enabled = true;
                    }



                    PickADance();
                    phase = congratulationsPhase.PARTY;
                }

                break;


            case congratulationsPhase.PARTY:

                if (Time.time - startTime >= firstDist)
                {
                    noise.enabled = true;
                    SoundManager.instance.musicSource.Pause();
                    whiteNoiseSource = SoundManager.instance.PlaySingle(shortNoise);
                    phase = congratulationsPhase.FIRST_NOISE;
                }

                break;

            case congratulationsPhase.FIRST_NOISE:

                if (!whiteNoiseSource.isPlaying)
                {
                    noise.enabled = false;
                    SoundManager.instance.musicSource.time = Time.time - startTime;
                    SoundManager.instance.musicSource.Play();
                    phase = congratulationsPhase.SECOND_PARTY;
                }

                break;

            case congratulationsPhase.SECOND_PARTY:

                if (skip || Time.time - startTime >= distFin)
                {
                    noise.enabled = true;
                    SoundManager.instance.musicSource.Pause();
                    whiteNoiseSource = SoundManager.instance.PlaySingle(longNoise);
                    black.color = new Color(0, 0, 0, 0);
                    phase = congratulationsPhase.FINAL_NOISE;
                }

                break;

            case congratulationsPhase.FINAL_NOISE:

                if (!whiteNoiseSource.isPlaying){
                    black.color = new Color(0, 0, 0, 1);
                    phase = congratulationsPhase.LOADING;
                    ReturnToMenu();
                }
                else if (whiteNoiseSource.time > 3.5f)
                {
                    float alpha = (whiteNoiseSource.time - 3.5f) / (6F - 3.5f);
                    black.color = new Color(0, 0, 0, alpha);
                }

                break;
        
        }

        if (nextChange < changes.Length && Time.time - startTime >= changes[nextChange] - 0.1f)
        {
            nextChange++;
            PickADance();
        }

        if (phase == congratulationsPhase.FIRST_NOISE || phase == congratulationsPhase.FINAL_NOISE)
        {
            actualNoiseImg++;

            if(actualNoiseImg > noiseImgs.Length - 1)
                actualNoiseImg = 0;

            noise.sprite = noiseImgs[actualNoiseImg];
        }

    }

    private void PickADance()
    {
        int newDance;
        bool search = true;
        while (search)
        {
            newDance = Random.Range(0, 4);
            if (actualDance != newDance)
            {
                actualDance = newDance;
                search = false;

                switch (newDance)
                {
                    case 0:
                        glitch.Play("The_Wave");
                        break;
                    case 1:
                        glitch.Play("The_Champion");
                        break;
                    case 2:
                        glitch.Play("The_Dance");
                        break;
                    case 3:
                        glitch.Play("The_Macarena");
                        break;
                }
            }
        }

        for (int i = 0; i < lights.Length; i+= 2)
        {
            //lights[i].color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0, 1f), lights[i].color.a);

            Color color = pickColor(lights[i].color.a);

            lights[i].color = color;
            lights[i + 1].color = color;

        }

        for (int i = 0; i < roundLights.Length; i++)
        {
            roundLights[i].color = pickColor(roundLights[i].color.a);
        }

    }

    private Color pickColor(float alpha)
    {
        Color color;

        switch (Random.Range(0,6))
        {
            case 0: color = Color.blue; break;
            case 1: color = Color.cyan; break;
            case 2: color = Color.green; break;
            case 3: color = Color.magenta; break;
            case 4: color = Color.red; break;
            default: color = Color.yellow; break;
        }

        color.a = alpha;

        return color;
    }

    public void ReturnToMenu()
    {
        Loader.LoadScene("menu", false, false, true, true);
    }
}
