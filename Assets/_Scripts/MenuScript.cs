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
	public Button firstLevelSelectButton;
	public Button firstHelpButton;
	public Button firstExitButton;
	public Button firstCreditsButton;


	// Use this for initialization
	void Start () {

		quitMenu = quitMenu.GetComponent<Canvas> ();
		levelSelectionMenu = levelSelectionMenu.GetComponent<Canvas> ();
		helpMenu = helpMenu.GetComponent<Canvas> ();
		creditsMenu = creditsMenu.GetComponent<Canvas> ();

		startText = startText.GetComponent<Button> ();
		exitText = exitText.GetComponent<Button> ();
		levelSelectText = levelSelectText.GetComponent<Button> ();
		HelpText = HelpText.GetComponent<Button> ();
		creditsText = creditsText.GetComponent<Button> ();

		keyboardButton = keyboardButton.GetComponent<Button> (); 
		gamepadButton = gamepadButton.GetComponent<Button> (); 

		keyboardImage = keyboardImage.GetComponent<Image> (); 
		gamepadImage = gamepadImage.GetComponent<Image> (); 

		firstLevelSelectButton = firstLevelSelectButton.GetComponent<Button> ();
		firstHelpButton = firstHelpButton.GetComponent<Button> ();
		firstExitButton = firstExitButton.GetComponent<Button> ();
		firstCreditsButton = firstCreditsButton.GetComponent<Button> ();

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

		gamepadButton.Select ();
	}

	public void GamepadPress()
	{
		keyboardButton.enabled = true;
		gamepadButton.enabled = false;
		keyboardImage.enabled = false;
		gamepadImage.enabled = true;

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
