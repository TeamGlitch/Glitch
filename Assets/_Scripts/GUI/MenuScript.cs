using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InControl;

public class MenuScript : MonoBehaviour {

    //References by screen//
    public Button startText;
    public Image pointer;

    public Text loadingText;

    public Canvas levelSelectionMenu;
    public Button levelSelectText;
    public Button firstLevelSelectButton;

    public Canvas optionsMenu;
    public Button OptionsText;
    public Button ControlsButton;

    public Canvas helpMenu;
    public Button firstHelpButton;
    public Button keyboardButton;
    public Button gamepadButton;
    public Button backButtonInHelp;
    public Image keyboardImage;
    public Image gamepadImage;

    public Canvas graphicsMenu;
    public Dropdown qualityLevel;
    public Dropdown resolutions;
    public Toggle fullscreen;
    public Button applyButton;

    public Canvas audioMenu;
    public Slider musicSlider;
    public Slider soundSlider;
    public Slider panSlider;
    public Dropdown speakersMode;

    public Canvas creditsMenu;
    public Button firstCreditsButton;

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

	void Start () 
    {
        optionsMenu.enabled = false;
		quitMenu.enabled = false;
		levelSelectionMenu.enabled = false;
		helpMenu.enabled = false;
        graphicsMenu.enabled = false;
        audioMenu.enabled = false;
		creditsMenu.enabled = false;
		startText.Select ();

		//Play the menu music and check this time as the last active
		lastTimeActive = Time.time;
		SoundManager.instance.musicSource.Play();
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
                Loader.LoadScene("Intro");
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
        Loader.LoadScene("Level1");
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
        Loader.LoadScene("Level1");
    }

    public void OptionsPress(){
        SoundManager.instance.PlaySingle(confirmSound);
        optionsMenu.enabled = true;

		startText.enabled = false;
		levelSelectText.enabled = false;
		exitText.enabled = false;
        OptionsText.enabled = false;

		onMainScreen = false;

		ControlsButton.Select ();
    }

    public void ReturnToOptions()
    {
        SoundManager.instance.PlaySingle(backSound);

        optionsMenu.enabled = true;
        helpMenu.enabled = false;
        graphicsMenu.enabled = false;
        audioMenu.enabled = false;
        creditsMenu.enabled = false;

        ControlsButton.Select();
    }

	public void HelpPress()
	{
        SoundManager.instance.PlaySingle(confirmSound);
        helpMenu.enabled = true;
        optionsMenu.enabled = false;

		keyboardButton.enabled = false;
		gamepadButton.enabled = true;

		keyboardImage.enabled = true;
		gamepadImage.enabled = false;

		Navigation customNav = new Navigation();
		customNav.mode = Navigation.Mode.Explicit;
		customNav.selectOnUp = gamepadButton;
		backButtonInHelp.navigation = customNav;

		firstHelpButton.Select ();
	}

    public void GraphicsPress()
    {

        SoundManager.instance.PlaySingle(confirmSound);
        graphicsMenu.enabled = true;
        optionsMenu.enabled = false;

        qualityLevel.options.Clear();
        string[] names = QualitySettings.names;
        for (int i = 0; i < names.Length; i++)
        {
            Dropdown.OptionData entry = new Dropdown.OptionData();
            entry.text = names[i];
            qualityLevel.options.Add(entry);
        }
        qualityLevel.value = QualitySettings.GetQualityLevel();

        resolutions.options.Clear();
        Resolution[] resolutionList = Screen.resolutions;
        for(int i = 0; i < resolutionList.Length; i++)
        {
            Dropdown.OptionData entry = new Dropdown.OptionData();
            entry.text = resolutionList[i].width + "x" + resolutionList[i].height;
            resolutions.options.Add(entry);

            if (resolutionList[i].width == Screen.width &&
                resolutionList[i].height == Screen.height)
            {
                resolutions.value = i;
            }
        }

        if (Screen.fullScreen)
            fullscreen.isOn = true;
        else
            fullscreen.isOn = false;

        resolutions.Select();

    }


    public void ChangeResolutionFullscreen()
    {
        Resolution newRes = Screen.resolutions[resolutions.value];
        Screen.SetResolution(newRes.width, newRes.height, fullscreen.isOn);
    }

    public void ChangeQuality()
    {
        QualitySettings.SetQualityLevel(qualityLevel.value);
    }

    public void AudioPress() {
        SoundManager.instance.PlaySingle(confirmSound);
        audioMenu.enabled = true;
        optionsMenu.enabled = false;

        musicSlider.value = SoundManager.instance.musicSource.volume;
        soundSlider.value = SoundManager.instance.efxSource1.volume;
        panSlider.value = SoundManager.instance.musicSource.panStereo;

        if (AudioSettings.speakerMode == AudioSpeakerMode.Mono)
            speakersMode.value = 0;
        else if(AudioSettings.speakerMode == AudioSpeakerMode.Stereo)
            speakersMode.value = 1;
        else if(AudioSettings.speakerMode == AudioSpeakerMode.Quad)
            speakersMode.value = 2;
        else if(AudioSettings.speakerMode == AudioSpeakerMode.Surround)
            speakersMode.value = 3;
        else if(AudioSettings.speakerMode == AudioSpeakerMode.Mode5point1)
            speakersMode.value = 4;
        else if(AudioSettings.speakerMode == AudioSpeakerMode.Mode7point1)
            speakersMode.value = 5;
        else if(AudioSettings.speakerMode == AudioSpeakerMode.Prologic)
            speakersMode.value = 6;

        musicSlider.Select();
    }

    public void ChangeMusicVolume(){
        SoundManager.instance.musicSource.volume = musicSlider.value;
    }

    public void ChangeSoundVolume()
    {
        SoundManager.instance.efxSource1.volume = soundSlider.value;
        SoundManager.instance.efxSource2.volume = soundSlider.value;
        SoundManager.instance.efxSource3.volume = soundSlider.value;
        SoundManager.instance.efxSource4.volume = soundSlider.value;
        SoundManager.instance.efxSource5.volume = soundSlider.value;

        SoundManager.instance.PlaySingle(confirmSound);
    }

    public void ChangePan(){

        SoundManager.instance.musicSource.panStereo = panSlider.value;

        SoundManager.instance.efxSource1.panStereo = panSlider.value;
        SoundManager.instance.efxSource2.panStereo = panSlider.value;
        SoundManager.instance.efxSource3.panStereo = panSlider.value;
        SoundManager.instance.efxSource4.panStereo = panSlider.value;
        SoundManager.instance.efxSource5.panStereo = panSlider.value;

    }

    public void ChangeSpeakerMode(){

        float musicTime = -1;

        if (SoundManager.instance.musicSource.isPlaying)
        {
            musicTime = SoundManager.instance.musicSource.time;
        }

        AudioConfiguration config = AudioSettings.GetConfiguration();

        switch (speakersMode.value){
            
            case 0: config.speakerMode = AudioSpeakerMode.Mono; break;
            case 1: config.speakerMode = AudioSpeakerMode.Stereo; break;
            case 2: config.speakerMode = AudioSpeakerMode.Quad; break;
            case 3: config.speakerMode = AudioSpeakerMode.Surround; break;
            case 4: config.speakerMode = AudioSpeakerMode.Mode5point1; break;
            case 5: config.speakerMode = AudioSpeakerMode.Mode7point1; break;
            case 6: config.speakerMode = AudioSpeakerMode.Prologic; break;

        }

        AudioSettings.Reset(config);

        if (musicTime != -1)
        {
            SoundManager.instance.musicSource.Play();
            SoundManager.instance.musicSource.time = musicTime;
        }

    }

    public void CreditsMenu()
    {
        SoundManager.instance.PlaySingle(confirmSound);
        creditsMenu.enabled = true;
        optionsMenu.enabled = false;

        firstCreditsButton.Select();
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

	public void KeyboardPress()
	{
        SoundManager.instance.PlaySingle(confirmSound);
        keyboardButton.enabled = false;
		gamepadButton.enabled = true;
		keyboardImage.enabled = true;
		gamepadImage.enabled = false;

		Navigation customNav = new Navigation();
		customNav.mode = Navigation.Mode.Explicit;
		customNav.selectOnUp = gamepadButton;
		backButtonInHelp.navigation = customNav;

		gamepadButton.Select ();
	}

	public void GamepadPress()
	{
        SoundManager.instance.PlaySingle(confirmSound);
        keyboardButton.enabled = true;
		gamepadButton.enabled = false;
		keyboardImage.enabled = false;
		gamepadImage.enabled = true;

		Navigation customNav = new Navigation();
		customNav.mode = Navigation.Mode.Explicit;
		customNav.selectOnUp = keyboardButton;
		backButtonInHelp.navigation = customNav;

		keyboardButton.Select ();
	}

	public void NoPress()
	{
        SoundManager.instance.PlaySingle(backSound);
        quitMenu.enabled = false;
		levelSelectionMenu.enabled = false;
		optionsMenu.enabled = false;
		creditsMenu.enabled = false;

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
