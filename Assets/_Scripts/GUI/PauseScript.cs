using UnityEngine;
using UnityEngine.UI;
using InControl;

public class PauseScript : MonoBehaviour {

	public GameObject playerPowers;

	public GameObject pauseMenu;
	public Button resumeButton;
	public Button restartButton;
	public Button menuButton;

	public void resumePress()
	{
		if (pauseMenu.activeInHierarchy)
		{
			playerPowers.SetActive (true);
			Time.timeScale = 1.0f;
            gameObject.SetActive(false);
		}
	}

	public void restartPress()
	{
		Time.timeScale = 1.0f;
        Loader.LoadScene("Level1");
	}

	public void menuPress()
	{
		Time.timeScale = 1.0f;
        Loader.LoadScene("menu");
	}

    public void optionPress(){

        pauseMenu.SetActive(false);
        resumeButton.enabled = false;
        restartButton.enabled = false;
        menuButton.enabled = false;

    }

    public void returnToMenu(){

        pauseMenu.SetActive(true);
        resumeButton.enabled = true;
        restartButton.enabled = true;
        menuButton.enabled = true;

        resumeButton.Select();

    }

    public void Pause()
    {
        playerPowers.SetActive(true);
        Time.timeScale = 0.0f;
        resumeButton.Select();
    }

    public bool Unpause()
    {
        if (pauseMenu.active)
        {
            Time.timeScale = 1.0f;
            playerPowers.SetActive(true);
            return true;
        }
        
        return false;
    }
}