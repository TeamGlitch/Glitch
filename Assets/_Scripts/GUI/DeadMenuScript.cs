using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeadMenuScript : MonoBehaviour {

	public Canvas deadMenu;
	public Button restartButton;
	public Button menuButton;
	public GlitchOffsetCamera glitchedCameraScript;

	private float timeDead;

	void Awake () 
    {
		gameObject.SetActive(false);
	}
	
    // The screen is glitched and appears a blue screen
	void Update () 
    {
		if (deadMenu.enabled == false) {
			timeDead += Time.deltaTime;
			if (timeDead >= 2.0f && timeDead < 4.0f) 
            {
                if (glitchedCameraScript.divisions < 50)
                {
                    glitchedCameraScript.divisions += 1;
                }
				glitchedCameraScript.intensity += 0.05f;
				glitchedCameraScript.frequency += 0.005f;
				glitchedCameraScript.inestability += 0.005f;
			} else if (timeDead >= 6.0f) {
				deadMenu.enabled = true;
				glitchedCameraScript.enabled = false;
				restartButton.Select ();
			}
		}
	}

	public void RestartPress()
	{
		SceneManager.LoadScene ("scene");
	}

	public void MenuPress()
	{
		SceneManager.LoadScene ("menu");
	}

	public void PlayerDead()
	{
		glitchedCameraScript.enabled = true;
		deadMenu.enabled = false;
		timeDead = 0.0f;
	}

}