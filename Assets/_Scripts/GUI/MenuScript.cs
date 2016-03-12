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


	void Start () 
    {
		quitMenu.enabled = false;
		levelSelectionMenu.enabled = false;
		helpMenu.enabled = false;
		creditsMenu.enabled = false;
		startText.Select ();
	}

	public void ContinuePress()
	{
		SceneManager.LoadScene ("scene");
	}

	public void LevelSelectPress()
	{
		levelSelectionMenu.enabled = true;

		startText.enabled = false;
		levelSelectText.enabled = false;
		HelpText.enabled = false;
		exitText.enabled = false;
		creditsText.enabled = false;

		firstLevelSelectButton.Select ();
	}

	public void HelpPress()
	{
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

		firstHelpButton.Select ();
	}


	public void QuitPress()
	{
		quitMenu.enabled = true;

		startText.enabled = false;
		levelSelectText.enabled = false;
		HelpText.enabled = false;
		exitText.enabled = false;
		creditsText.enabled = false;

		firstExitButton.Select ();
	}

	public void KeyboardPress()
	{
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
		quitMenu.enabled = false;
		levelSelectionMenu.enabled = false;
		helpMenu.enabled = false;
		creditsMenu.enabled = false;

		startText.enabled = true;
		levelSelectText.enabled = true;
		HelpText.enabled = true;
		exitText.enabled = true;
		creditsText.enabled = true;

		startText.Select ();
	}

	public void CreditsMenu()
	{
		creditsMenu.enabled = true;

		startText.enabled = false;
		levelSelectText.enabled = false;
		HelpText.enabled = false;
		exitText.enabled = false;
		creditsText.enabled = false;

		firstCreditsButton.Select ();
	}

	public void ExitGame()
	{
		Application.Quit ();
	}

}
