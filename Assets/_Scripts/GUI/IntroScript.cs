using UnityEngine;
using System.Collections;
using InControl;

[RequireComponent (typeof (AudioSource))]

public class IntroScript : MonoBehaviour {
    public enum introPhases
    {
        ALLBLACK,
        DISK_INSERTION,
        POSITIONING_LOGO_SCREEN,
        PRE_LOGOSCREEN,
        LOGOSCREEN,
        COLLAPSE_SCREEEN,
        BLACK_PRE_INTRO,
        INTROMOVIE,
    };

	public MovieTexture movie;
    public AudioClip amigaSound;
    public Transform logoscreen;

    private AudioSource audio;
    private RectTransform logoMenu;

    private introPhases phase = introPhases.ALLBLACK;
    private float phaseStart = 0;

    private float timeToEnd;

	void Start(){
		audio = GetComponent<AudioSource>();

		GetComponent<Renderer>().material.mainTexture = movie as MovieTexture;
		audio.clip = movie.audioClip;

        logoMenu = logoscreen.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        logoMenu.position = new Vector3(3000,0,-1);

        
	}

	// Update is called once per frame
	void Update () {

        if (phase == introPhases.PRE_LOGOSCREEN || phase == introPhases.LOGOSCREEN){
            if (Random.value > 0.2)
            {
                logoMenu.position = new Vector3((-Random.Range(-2, 2)), 0, -1);
            }
            else if (logoMenu.position.x != 0)
            {
                logoMenu.position = new Vector3(0, 0, -1);
            }
        }

        switch (phase){
        
            case introPhases.ALLBLACK:
                if (Time.time > phaseStart + 2.1f){
                    phase = introPhases.DISK_INSERTION;
                    phaseStart = Time.time;
                    SoundManager.instance.PlaySingle(amigaSound);
                    timeToEnd = Time.time + amigaSound.length;
                }

                break;

            case introPhases.DISK_INSERTION:

                if (Time.time > phaseStart + 1.9f)
                {
                    phase = introPhases.POSITIONING_LOGO_SCREEN;
                    phaseStart = Time.time;
                }

                break;

            case introPhases.POSITIONING_LOGO_SCREEN:

                if (Time.time < phaseStart + 0.7f)
                {
                    logoMenu.position = new Vector3((1066 - Random.Range(-20, 20)), 0, -1);
                }
                else {
                    logoMenu.position = new Vector3(0, 0, -1);
                    phase = introPhases.PRE_LOGOSCREEN;
                }
                break;

            case introPhases.PRE_LOGOSCREEN:

                if (Time.time > phaseStart + 0.3f)
                {
                    logoMenu.GetChild(1).gameObject.SetActive(true);
                    logoMenu.GetChild(1).GetComponent<Animation>().Play();
                    phase = introPhases.LOGOSCREEN;
                    phaseStart = Time.time;
                }

                break;

            case introPhases.LOGOSCREEN:

                if (Time.time >= timeToEnd - 0.2f){
                    phase = introPhases.COLLAPSE_SCREEEN;
                    phaseStart = Time.time;
                }

                break;

            case introPhases.COLLAPSE_SCREEEN:

                if (Time.time < timeToEnd)
                {
                    float percent = (Time.time - phaseStart) / (timeToEnd - phaseStart);
                    
                    Vector3 newPosition = new Vector3(1066 * 0.25f * percent, 450 * 0.25f * percent, -1);

                    if (Random.value >= 0.25)
                        newPosition.x *= -1;

                    if (Random.value >= 0.9)
                        newPosition.y *= -1;

                    logoMenu.position = newPosition;

                }
                else {
                    logoMenu.gameObject.SetActive(false);
                    phaseStart = Time.time;
                    phase = introPhases.BLACK_PRE_INTRO;
                }

                break;

            case introPhases.BLACK_PRE_INTRO:

                if (Time.time > phaseStart + 2.5f){
                    logoscreen.gameObject.SetActive(false);
                    movie.Play();
                    audio.Play();
                    timeToEnd = Time.time + movie.duration;
                    phase = introPhases.INTROMOVIE;
                }

                break;

            case introPhases.INTROMOVIE:

                if (Time.time > timeToEnd || InputManager.ActiveDevice.AnyButton.WasPressed)
                {
                    Loader.LoadScene("menu");
                }
                else if (Camera.current == Camera.main)
                {
                    float height = Camera.current.orthographicSize * 2;
                    float width = Camera.current.aspect * height;
                    transform.localScale = new Vector3(width, height, 0.1f);
                }

                break;
        }
	}
}
