using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using InControl;

public class PauseScript : MonoBehaviour {

	public GameObject playerPowers;

	public Canvas pauseMenu;
	public Button resumeButton;
	public Button restartButton;
	public Button menuButton;

	public void resumePress()
	{
		if (pauseMenu.enabled)
		{
			playerPowers.SetActive (true);
			Time.timeScale = 1.0f;
            pauseMenu.gameObject.SetActive(false);
		}
	}

	public void restartPress()
	{
		Time.timeScale = 1.0f;
		SceneManager.LoadScene ("Level1");
	}

	public void menuPress()
	{
		Time.timeScale = 1.0f;
		SceneManager.LoadScene ("menu");
	}


    public void Pause()
    {
        resumeButton.Select();
        Time.timeScale = 0.0f;
        playerPowers.SetActive(true);
    }

    public void Unpause()
    {
        Time.timeScale = 1.0f;
        playerPowers.SetActive(true);
    }
}