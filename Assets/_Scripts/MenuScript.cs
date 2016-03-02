using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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

	public float origW = 1980.0f;
	public float origH = 1020.0f;

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

		quitMenu.enabled = false;
		levelSelectionMenu.enabled = false;
		helpMenu.enabled = false;
		creditsMenu.enabled = false;
	
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
	}


	public void QuitPress()
	{
		quitMenu.enabled = true;

		startText.enabled = false;
		levelSelectText.enabled = false;
		HelpText.enabled = false;
		exitText.enabled = false;
		creditsText.enabled = false;
	}

	public void KeyboardPress()
	{
		keyboardButton.enabled = false;
		gamepadButton.enabled = true;
		keyboardImage.enabled = true;
		gamepadImage.enabled = false;
	}

	public void GamepadPress()
	{
		keyboardButton.enabled = true;
		gamepadButton.enabled = false;
		keyboardImage.enabled = false;
		gamepadImage.enabled = true;
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
	}

	public void CreditsMenu()
	{
		creditsMenu.enabled = true;

		startText.enabled = false;
		levelSelectText.enabled = false;
		HelpText.enabled = false;
		exitText.enabled = false;
		creditsText.enabled = false;
	}

	public void ExitGame()
	{
		Application.Quit ();
	}

}
