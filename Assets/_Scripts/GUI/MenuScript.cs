using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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

    //SOUND//
    public AudioClip backSound;
    public AudioClip selectSound;
    public AudioClip confirmSound;

    //MISCELLANEA//
	private float lastTimeActive = 0;
	private bool onMainScreen = true;

    public TextAsset XMLAsset;

	void Start () 
    {

		quitMenu.enabled = false;
		levelSelectionMenu.enabled = false;
		startText.Select ();

		//Play the menu music and check this time as the last active
		lastTimeActive = Time.time;
		SoundManager.instance.musicSource.Play();

        options =  GetComponent<OptionsMenuScript>();
        SetTexts();

        Configuration.addLanguageListener(this);
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
        options.Disable();

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
