using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeadMenuScript : MonoBehaviour {

	public Canvas deadMenu;
	public Button restartButton;
	public Button menuButton;

	public GlitchOffsetCamera glitchedCameraScript;

	private float timeDead;
	private bool dead;

	// Use this for initialization
	void Start () {
		deadMenu = deadMenu.GetComponent<Canvas> ();

		restartButton = restartButton.GetComponent<Button> ();
		menuButton = menuButton.GetComponent<Button> ();

		deadMenu.enabled = false;

		dead = false;

		timeDead = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (dead) {
			timeDead += Time.deltaTime;
			if (timeDead >= 2.0f && timeDead < 4.0f) {
				if(glitchedCameraScript.divisions < 50)
					glitchedCameraScript.divisions += 1;
				glitchedCameraScript.intensity += 0.05f;
				glitchedCameraScript.frequency += 0.005f;
				glitchedCameraScript.inestability += 0.005f;
			} else if (timeDead >= 4.0f && timeDead < 6.0f) {
			} else if (timeDead >= 6.0f) {
				deadMenu.enabled = true;
				glitchedCameraScript.enabled = false;
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
		timeDead = 0.0f;
		dead = true;
	}

}