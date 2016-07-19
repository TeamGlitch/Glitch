using UnityEngine;
using System.Collections;
using InControl;

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

    private RectTransform logoMenu;

    private introPhases phase = introPhases.ALLBLACK;
    private float phaseStart = 0;

    private float timeToEnd;

	void Start(){

		GetComponent<Renderer>().material.mainTexture = movie as MovieTexture;

        logoMenu = logoscreen.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        logoMenu.position = new Vector3(3000,0,-1);

        Loader.LoadScene("menu", false, false, true, false);
	}

	// Update is called once per frame
	void Update () {

        if (InputManager.ActiveDevice.AnyButton.WasPressed){
            Loader.allowToFinish();
            return;
        }

        if (phase == introPhases.PRE_LOGOSCREEN || phase == introPhases.LOGOSCREEN){
			
            if (Random.value > 0.2)
            {
				float offset = Random.Range (-Screen.width * 0.002f, Screen.width * 0.002f);
				logoMenu.offsetMax = new Vector2(offset, 0);
				logoMenu.offsetMin = new Vector2(offset, 0);
            }
            else if (logoMenu.position.x != 0)
            {
				logoMenu.offsetMax = new Vector2(0, 0);
				logoMenu.offsetMin = new Vector2(0, 0);
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
					float offset = Random.Range (-Screen.width * 0.02f, Screen.width * 0.02f);
					logoMenu.offsetMax = new Vector2((Screen.width * 0.75f) + offset, 0);
					logoMenu.offsetMin = new Vector2((Screen.width * 0.75f) + offset, 0);
                }
                else {
					logoMenu.offsetMax = new Vector2(0, 0);
					logoMenu.offsetMin = new Vector2(0, 0);
                    phase = introPhases.PRE_LOGOSCREEN;
                }
                break;

            case introPhases.PRE_LOGOSCREEN:

                if (Time.time > phaseStart + 0.3f)
                {
					logoMenu.GetChild(0).GetChild(1).gameObject.SetActive(true);
					logoMenu.GetChild(0).GetChild(1).GetComponent<Animation>().Play();
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
                    
					float directionX = 1, directionY = 1;

					if (Random.value >= 0.25)
						directionX = -1;

					if (Random.value >= 0.85)
						directionY = -1;

					logoMenu.offsetMax = new Vector2(Screen.width * percent * directionX * 0.25f, Screen.height * percent * directionY * 0.25f);
					logoMenu.offsetMin = new Vector2(Screen.width * percent * directionX * 0.25f, Screen.height * percent * directionY * 0.25f);
                }
                else
				{
                    logoMenu.gameObject.SetActive(false);
                    phaseStart = Time.time;
                    phase = introPhases.BLACK_PRE_INTRO;
                }

                break;

            case introPhases.BLACK_PRE_INTRO:

                if (Time.time > phaseStart + 2.5f){
                    logoscreen.gameObject.SetActive(false);

                    adjustCamera();
                    movie.Play();
                    SoundManager.instance.PlaySingle(movie.audioClip);

                    timeToEnd = Time.time + movie.duration;
                    phase = introPhases.INTROMOVIE;
                }

                break;

            case introPhases.INTROMOVIE:

                if (Time.time > timeToEnd)
                {
                    Loader.allowToFinish();
                }
                else if (Camera.current == Camera.main)
                {
                    adjustCamera();
                }

                break;
        }
	}

    private void adjustCamera(){
        float height = Camera.current.orthographicSize * 2;
        float width = Camera.current.aspect * height;
        transform.localScale = new Vector3(width, height, 0.1f);
    }
}