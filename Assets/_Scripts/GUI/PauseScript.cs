using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using InControl;

public class PauseScript : MonoBehaviour {

	public Canvas pauseMenu;
	public Button resumeButton;
	public Button restartButton;
	public Button menuButton;


	void Start () 
    {
		pauseMenu.enabled = false;
	}
	
	void Update () 
    {
		if (InputManager.ActiveDevice.MenuWasPressed && !pauseMenu.enabled) 
        {
			Time.timeScale = 0.0f;
			pauseMenu.enabled = true;
			resumeButton.Select ();
		} else if (InputManager.ActiveDevice.MenuWasPressed) {
			Time.timeScale = 1.0f;
			pauseMenu.enabled = false;
		}
	}

	public void resumePress()
	{
		Time.timeScale = 1.0f;
		pauseMenu.enabled = false;	
	}

	public void restartPress()
	{
		Time.timeScale = 1.0f;
		SceneManager.LoadScene ("scene");
	}

	public void menuPress()
	{
		Time.timeScale = 1.0f;
		SceneManager.LoadScene ("menu");
	}
}