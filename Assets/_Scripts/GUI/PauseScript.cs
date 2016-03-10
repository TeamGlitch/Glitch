using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseScript : MonoBehaviour {

	public Canvas pauseMenu;
	public Button resumeButton;
	public Button restartButton;
	public Button menuButton;

	// Use this for initialization
	void Start () {
		pauseMenu = pauseMenu.GetComponent<Canvas> ();

		resumeButton = resumeButton.GetComponent<Button> ();
		restartButton = restartButton.GetComponent<Button> ();
		menuButton = menuButton.GetComponent<Button> ();

		pauseMenu.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape) && !pauseMenu.enabled) {
			Time.timeScale = 0.0f;
			pauseMenu.enabled = true;
		} else if (Input.GetKeyDown (KeyCode.Escape)) {
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
