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
    public Dropdown resolutions;
    public Toggle fullscreen;
    public Button applyButton;

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

    public void GraphicsPress(){
        SoundManager.instance.PlaySingle(confirmSound);
        graphicsMenu.enabled = true;
        optionsMenu.enabled = false;

        resolutions.options.Clear();
        Resolution[] resolutionList = Screen.resolutions;
        foreach (Resolution res in resolutionList)
        {
            Dropdown.OptionData entry = new Dropdown.OptionData();
            entry.text = res.width + "x" + res.height;
            resolutions.options.Add(entry);
        }

        if (Screen.fullScreen)
            fullscreen.isOn = true;
        else
            fullscreen.isOn = false;

        resolutions.Select();
    }

    public void ChangesApplicable(){
        if (Screen.fullScreen != fullscreen.isOn ||
            Screen.resolutions[resolutions.value].height != Screen.currentResolution.height ||
            Screen.resolutions[resolutions.value].width != Screen.currentResolution.width)
        {
                applyButton.interactable = true;
        } else
        {
                applyButton.interactable = false;
        }
    }

    public void ApplyChanges(){
        Resolution newRes = Screen.resolutions[resolutions.value];
        Screen.SetResolution(newRes.width, newRes.height, fullscreen.isOn);
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
