using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using InControl;

public class MenuScript : MonoBehaviour {

	public Canvas quitMenu;
	public Canvas levelSelectionMenu;
	public Canvas helpMenu;
	public Canvas creditsMenu;
	public Button startText;
	public Button exitText;
	public Button levelSelectText;
	public Button creditsText;
	public Button HelpText;
	public Button keyboardButton;
	public Button gamepadButton;
	public Image keyboardImage;
	public Image gamepadImage;
	public Button backButtonInHelp;
	public Button firstLevelSelectButton;
	public Button firstHelpButton;
	public Button firstExitButton;
	public Button firstCreditsButton;
    public Text loadingText;

    public AudioClip backSound;
    public AudioClip selectSound;
    public AudioClip confirmSound;

	private float lastTimeActive = 0;
	private bool onMainScreen = true;

	void Start () 
    {
		quitMenu.enabled = false;
		levelSelectionMenu.enabled = false;
		helpMenu.enabled = false;
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
				SceneManager.LoadScene ("Intro");
			}
		}
	}

	public void ContinuePress()
	{
        SoundManager.instance.PlaySingle(confirmSound);
        levelSelectionMenu.enabled = false;
        startText.gameObject.SetActive(false);
        levelSelectText.gameObject.SetActive(false);
        HelpText.gameObject.SetActive(false);
        exitText.gameObject.SetActive(false);
        creditsText.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(true);
		SoundManager.instance.musicSource.Stop();
		onMainScreen = false;
        SceneManager.LoadScene("Level1");
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
        HelpText.gameObject.SetActive(false);
        exitText.gameObject.SetActive(false);
        creditsText.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(true);
        SoundManager.instance.musicSource.Stop();
        onMainScreen = false;
        SceneManager.LoadScene("Level1");
    }

	public void HelpPress()
	{
        SoundManager.instance.PlaySingle(confirmSound);
        helpMenu.enabled = true;

		startText.enabled = false;
		levelSelectText.enabled = false;
		HelpText.enabled = false;
		exitText.enabled = false;
		creditsText.enabled = false;

		keyboardButton.enabled = false;
		gamepadButton.enabled = true;

		keyboardImage.enabled = true;
		gamepadImage.enabled = false;

		Navigation customNav = new Navigation();
		customNav.mode = Navigation.Mode.Explicit;
		customNav.selectOnUp = gamepadButton;
		backButtonInHelp.navigation = customNav;

		onMainScreen = false;

		firstHelpButton.Select ();
	}


	public void QuitPress()
	{
        SoundManager.instance.PlaySingle(confirmSound);
        quitMenu.enabled = true;

		startText.enabled = false;
		levelSelectText.enabled = false;
		HelpText.enabled = false;
		exitText.enabled = false;
		creditsText.enabled = false;

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
		helpMenu.enabled = false;
		creditsMenu.enabled = false;

		startText.enabled = true;
		levelSelectText.enabled = true;
		HelpText.enabled = true;
		exitText.enabled = true;
		creditsText.enabled = true;

		onMainScreen = true;
		lastTimeActive = Time.time;

		startText.Select ();
	}

	public void CreditsMenu()
	{
        SoundManager.instance.PlaySingle(confirmSound);
        creditsMenu.enabled = true;

		startText.enabled = false;
		levelSelectText.enabled = false;
		HelpText.enabled = false;
		exitText.enabled = false;
		creditsText.enabled = false;

		onMainScreen = false;

		firstCreditsButton.Select ();
	}

	public void ExitGame()
	{
        Application.Quit();
	}

    public void MakeSelectSound()
    {
        SoundManager.instance.PlaySingle(selectSound);
    }


}
