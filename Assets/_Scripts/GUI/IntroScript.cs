using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine.UI;
using System.Xml;
using System.Globalization;

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

    private struct subsEntry
    {
        public string content;
        public float begin;
        public float end;
        public bool secundary;
        public bool corrupted;
    }

	public MovieTexture movie;
    public AudioClip amigaSound;
    public Transform logoscreen;

    private RectTransform logoMenu;

    private introPhases phase = introPhases.ALLBLACK;
    private float phaseStart = 0;

    private float timeToEnd;
    private float started;

    public TextAsset XMLAsset;
    public Text Subtitles;
    private List<subsEntry> subtitleEntries;
    private List<subsEntry> actualSubs = new List<subsEntry>();
    private string MainMessage = "";
    private string SecondMessage = "";
    private float glitchStart = 0;
    private float glitchEnd = 0;

    private char[] randomLetters = { '~', '€', '@', '¬', '#', '|', '/', '$', '·', '!', '?' };

	void Start(){

        Configuration.LoadConfiguration();

		GetComponent<Renderer>().material.mainTexture = movie as MovieTexture;

        logoMenu = logoscreen.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        logoMenu.position = new Vector3(3000,0,-1);

        loadSubtitles();
        updateSubtiltes();

        Loader.LoadScene("menu", false, false, true, false);
	}

    private void loadSubtitles()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(XMLAsset.text);

        XmlNode texts = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]");

        subtitleEntries = new List<subsEntry>();
        for (int i = 0; i < texts.ChildNodes.Count; i++)
        {
            XmlNode child = texts.ChildNodes[i];
            subsEntry entry = new subsEntry();

            entry.content = child.InnerText;
            entry.begin = getTime(child.Attributes["begin"].Value);
            entry.end = getTime(child.Attributes["end"].Value);

            if (child.Name == "MainCorr")
            {
                entry.secundary = false;
                entry.corrupted = true;
            }
            else if (child.Name == "Secund")
            {
                entry.secundary = true;
                entry.corrupted = false;
            }
            else
            {
                entry.secundary = false;
                entry.corrupted = false;
            }

            subtitleEntries.Add(entry);

        }

        subtitleEntries.Sort(delegate(subsEntry a, subsEntry b)
        {
            return (a.begin).CompareTo(b.begin);
        });

    }

    private float getTime(string formattedTime)
    {
        float time = 0;
        string rest = formattedTime;

        time += float.Parse(rest.Split(':')[0], CultureInfo.InvariantCulture.NumberFormat) * 60 * 60;
        time += float.Parse(rest.Split(':')[1], CultureInfo.InvariantCulture.NumberFormat) * 60;
        time += float.Parse(rest.Split(':')[2], CultureInfo.InvariantCulture.NumberFormat);

        return time;
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
                    started = Time.time;
                    phase = introPhases.INTROMOVIE;
                }

                break;

            case introPhases.INTROMOVIE:

                float passed = Time.time - started;

                bool glitching = false;
                for (int i = actualSubs.Count - 1; i >= 0; i--)
                {
                    if (passed >= actualSubs[i].end)
                    {

                        if (actualSubs[i].secundary)
                            SecondMessage = "";
                        else
                            MainMessage = "";

                        updateSubtiltes();
                        actualSubs.RemoveAt(i);
                    }
                    else
                    {
                        if (actualSubs[i].corrupted)
                            glitching = true;
                    }
                }

                for (int i = 0; i < subtitleEntries.Count; i++)
                {
                    if (passed >= subtitleEntries[i].begin)
                    {
                        subsEntry entry = subtitleEntries[i];

                        if (entry.secundary)
                            SecondMessage = entry.content + "\n";
                        else
                            MainMessage = entry.content;

                        if (!glitching && entry.corrupted)
                        {
                            glitchStart = Time.time + 0.2f;
                            glitchStart = Time.time + 0.3f;
                        }

                        updateSubtiltes();
                        actualSubs.Add(subtitleEntries[i]);
                        subtitleEntries.RemoveAt(i);
                        i--;
                    }
                    else
                        break;
                }

                if (glitching)
                {
                    if (Time.time > glitchStart)
                    {
                        if (Time.time > glitchEnd)
                        {
                            glitchStart = Time.time + 0.2f + Random.Range(0, 0.1f);
                            glitchEnd = Time.time + 0.3f + Random.Range(0, 0.1f);
                            updateSubtiltes();
                        }
                        else
                        {
                            char[] corruption = MainMessage.ToCharArray();

                            int numCorruptions = Random.Range(5, 10);
                            for (int i = 0; i < numCorruptions; i++)
                            {
                                corruption[Random.Range(0, corruption.Length - 1)] = randomLetters[Random.Range(0, randomLetters.Length - 1)];
                            }
                            
                            updateSubtiltes(new string(corruption));
                        }
                        
                    }
                }

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

    private void updateSubtiltes(string subs = "")
    {
        if (subs == "")
            Subtitles.text = SecondMessage + MainMessage;
        else
            Subtitles.text = SecondMessage + subs;
    }

    private void adjustCamera(){
        float height = Camera.current.orthographicSize * 2;
        float width = Camera.current.aspect * height;
        transform.localScale = new Vector3(width, height, 0.1f);
    }
}