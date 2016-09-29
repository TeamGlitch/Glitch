using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using InControl;
using System.Xml;

public class MenuScript : MonoBehaviour, LanguageListener {

    //References by screen//
    public Button startText;
    public Image pointer;

    public Text loadingText;

    public Canvas levelSelectionMenu;
    public Button levelSelectText;
    public Button firstLevelSelectButton;

    private OptionsMenuScript options;
    public Button OptionsText;

	public Canvas quitMenu;
    public Button exitText;
    public Button firstExitButton;

    public Canvas highscoreMenu;
    public Text highscoreLevelName;
    public Text highscoreNames;
    public Text highscorePoints;
    public Button highscoreReturnButton;
    public Text highscoreLeft;
    public Text highscoreRight;
    private int actualHighscore = -1;

    //SOUND//
    public AudioClip backSound;
    public AudioClip selectSound;
    public AudioClip confirmSound;

    //MISCELLANEA//
	private float lastTimeActive = 0;
	private bool onMainScreen = true;

    public TextAsset XMLAsset;

    private bool leftPadRested = true;

	void Start () 
    {

		quitMenu.enabled = false;
		levelSelectionMenu.enabled = false;
        highscoreMenu.enabled = false;
		startText.Select ();

		//Play the menu music and check this time as the last active
		lastTimeActive = Time.time;
		SoundManager.instance.musicSource.Play();

        options =  GetComponent<OptionsMenuScript>();
        SetTexts();

        Configuration.addLanguageListener(this);


        //TODO: Repeated with PauseScript Start with minor differences
        ColorBlock cb;
        Color darkBrown;
        Color lightBrown;
        Color white;

        ColorUtility.TryParseHtmlString("#9F9F9FFF", out darkBrown);
        ColorUtility.TryParseHtmlString("#C8C8C8FF", out lightBrown);
        ColorUtility.TryParseHtmlString("#FFFFFFFF", out white);

        Toggle[] toggles = transform.parent.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < toggles.Length; i++)
        {
            cb = toggles[i].colors;
            cb.normalColor = darkBrown;
            cb.highlightedColor = white;
            toggles[i].colors = cb;
        }

        Dropdown[] dropdowns = transform.parent.GetComponentsInChildren<Dropdown>();
        for (int i = 0; i < dropdowns.Length; i++)
        {
            cb = dropdowns[i].colors;
            cb.normalColor = darkBrown;
            cb.highlightedColor = white;
            dropdowns[i].colors = cb;

            Toggle toggle = dropdowns[i].transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<Toggle>();
            cb = toggle.colors;
            cb.normalColor = darkBrown;
            cb.highlightedColor = lightBrown;
            toggle.colors = cb;
        }

	}

    void OnDestroy()
    {
        Configuration.removeLanguageListener(this);
    }

	void Update(){

		//If it's in the main screen
		if (onMainScreen) {
			//If the player pushes any button, update the last time active
			if (InputManager.ActiveDevice.AnyButton.IsPressed) {
				lastTimeActive = Time.time;
				//If a given time has passed without input, play the intro
			} else if (Time.time > lastTimeActive + 60f) {
				SoundManager.instance.musicSource.Stop();
                Loader.LoadScene("Intro", false, false, true, true);
			}
		}
        else if (highscoreMenu.enabled)
        {
            if (InputManager.ActiveDevice.LeftStick.X < -0.5f && highscoreLeft.enabled == true && leftPadRested)
            {
                actualHighscore--;
                readHighscoreList();
                leftPadRested = false;
            }
            else if (InputManager.ActiveDevice.LeftStick.X > 0.5f && highscoreRight.enabled == true && leftPadRested)
            {
                actualHighscore++;
                readHighscoreList();
                leftPadRested = false;
            }
            else if (!leftPadRested && InputManager.ActiveDevice.LeftStick.X > -0.5f && InputManager.ActiveDevice.LeftStick.X < 0.5f)
            {
                leftPadRested = true;
            }
        }
	}

    public void SetTexts()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(XMLAsset.text);

        XmlNodeList texts = xmlDoc.SelectNodes("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Group[@id = \"Main_Menu\"]/UI");

        string menuName;
        Transform menu, element;
        Text elementText;
        for (int i = 0; i < texts.Count; i++)
        {
            menuName = texts[i].Attributes["id"].Value;
            menu = transform.parent.FindChild(menuName);

            if (menu != null)
            {
                for (int z = 0; z < texts[i].ChildNodes.Count; z++)
                {
                    element = menu.FindChild(texts[i].ChildNodes[z].Attributes["id"].Value);
                    if (element != null)
                    {
                        elementText = element.GetComponent<Text>();
                        if (elementText != null)
                        {
                            elementText.text = texts[i].ChildNodes[z].InnerText;
                        }
                        else
                        {
                            print(texts[i].ChildNodes[z].Attributes["id"].Value + " on " + texts[i].Attributes["id"].Value + " doesn't have a Text.");
                        }
                    }
                    else
                    {
                        print(texts[i].ChildNodes[z].Attributes["id"].Value + " not found on " + texts[i].Attributes["id"].Value + ".");
                    }
                } // ENDFOR
            }
            else
            {
                print("Menu " + texts[i].Attributes["id"].Value + " not found.");
            }
        }


    }

	public void ContinuePress()
	{
        SoundManager.instance.PlaySingle(confirmSound);
        levelSelectionMenu.enabled = false;
        startText.gameObject.SetActive(false);
        levelSelectText.gameObject.SetActive(false);
        exitText.gameObject.SetActive(false);
        OptionsText.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(true);
		SoundManager.instance.musicSource.Stop();
		onMainScreen = false;

        if (Loader.getLastLevel() == "None")
        {
            Loader.LoadScene("Level1", true);
        }
        else
        {
            Loader.LoadScene(Loader.getLastLevel(), true);
        }
        
    }

	public void LevelSelectPress()
	{
        SoundManager.instance.PlaySingle(confirmSound);
        levelSelectionMenu.enabled = true;
        firstLevelSelectButton.Select();

		onMainScreen = false;
	}

    public void Level1Press()
    {
        SoundManager.instance.PlaySingle(confirmSound);
        levelSelectionMenu.enabled = false;
        startText.gameObject.SetActive(false);
        levelSelectText.gameObject.SetActive(false);
        exitText.gameObject.SetActive(false);
        OptionsText.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(true);
        SoundManager.instance.musicSource.Stop();
        onMainScreen = false;
        Loader.LoadScene("Level1", true);
    }

    public void LevelBossPress()
    {
        SoundManager.instance.PlaySingle(confirmSound);
        levelSelectionMenu.enabled = false;
        startText.gameObject.SetActive(false);
        levelSelectText.gameObject.SetActive(false);
        exitText.gameObject.SetActive(false);
        OptionsText.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(true);
        SoundManager.instance.musicSource.Stop();
        onMainScreen = false;
        Loader.LoadScene("BossStage", true);
    }

    public void HighscorePress()
    {
        SoundManager.instance.PlaySingle(confirmSound);

        highscoreMenu.enabled = true;

        startText.enabled = false;
        levelSelectText.enabled = false;
        exitText.enabled = false;
        OptionsText.enabled = false;

        onMainScreen = false;

        actualHighscore = 0;
        readHighscoreList();
        highscoreReturnButton.Select();
    }

    private void readHighscoreList()
    {
        List<ScoreManager.HiscoreList> hs = ScoreManager.instance.getHighScores();

        if (hs.Count == 0)
        {
            highscoreLevelName.text = "No highscores";

            highscoreNames.text = "";
            for (int i = 0; i < 12; i++)
                highscoreNames.text += "------------\n";

            highscorePoints.text = "";
            for (int i = 0; i < 12; i++)
                highscorePoints.text += "------------\n";

            highscoreLeft.enabled = false;
            highscoreRight.enabled = false;
        }
        else
        {
            highscoreLevelName.text = hs[actualHighscore].name;

            highscoreNames.text = "";
            highscorePoints.text = "";
            for (int i = 0; i < 10; i++)
            {
                if (hs[actualHighscore].list[i] == null)
                {
                    highscoreNames.text += "------------\n";
                    highscorePoints.text += "------------\n";
                }
                else
                {
                    highscoreNames.text += hs[actualHighscore].list[i].name + "\n";
                    highscorePoints.text += hs[actualHighscore].list[i].points + "\n";
                }
            }

            if (actualHighscore == 0)
                highscoreLeft.enabled = false;
            else
                highscoreLeft.enabled = true;

            if (actualHighscore == hs.Count - 1)
                highscoreRight.enabled = false;
            else
                highscoreRight.enabled = true;
        }
    }

    public void OptionsPress(){

        SoundManager.instance.PlaySingle(confirmSound);

		startText.enabled = false;
		levelSelectText.enabled = false;
		exitText.enabled = false;
        OptionsText.enabled = false;

		onMainScreen = false;

        options.Enable();
    }

	public void QuitPress()
	{
        SoundManager.instance.PlaySingle(confirmSound);
        quitMenu.enabled = true;

		startText.enabled = false;
		levelSelectText.enabled = false;
        OptionsText.enabled = false;
		exitText.enabled = false;

		onMainScreen = false;

		firstExitButton.Select ();
	}

	public void NoPress()
	{
        SoundManager.instance.PlaySingle(backSound);
        quitMenu.enabled = false;
		levelSelectionMenu.enabled = false;
        highscoreMenu.enabled = false;
        options.Disable();

        actualHighscore = -1;


		startText.enabled = true;
		levelSelectText.enabled = true;
		exitText.enabled = true;
        OptionsText.enabled = true;

		onMainScreen = true;
		lastTimeActive = Time.time;

		startText.Select ();
	}

	public void ExitGame()
	{
        Application.Quit();
	}

    public void MakeSelectSound(RectTransform textPosition)
    {
        SoundManager.instance.PlaySingle(selectSound);

        if (textPosition != null) { 
            Vector3 newPosition = pointer.rectTransform.anchoredPosition;
            newPosition.y = textPosition.anchoredPosition.y + 13.5f;
            pointer.rectTransform.anchoredPosition = newPosition;
        }
    }

    public void MakeSelectSound()
    {
        MakeSelectSound(null);
    }
}
