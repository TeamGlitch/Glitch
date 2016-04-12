using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeadMenuScript : MonoBehaviour {

	private Canvas deadMenu;
	private Button restartButton;
	private Button menuButton;

	public GameObject playerPowers;
	public GlitchOffsetCamera glitchedCameraScript;

	private float timeDead;

	// Use this for initialization
	void Awake () {

		deadMenu = GetComponent<Canvas>();
		restartButton = transform.FindChild("Restart Game").GetComponent<Button> ();
		menuButton = transform.FindChild("Main Menu").GetComponent<Button>();
		gameObject.SetActive(false);
		playerPowers.SetActive (false);
		restartButton.Select ();
	}

	// Update is called once per frame
	void Update () {
		if (deadMenu.enabled == false) {
			timeDead += Time.deltaTime;
			if (timeDead >= 2.0f && timeDead < 4.0f) {
				if(glitchedCameraScript.divisions < 50)
					glitchedCameraScript.divisions += 1;
				glitchedCameraScript.intensity += 0.05f;
				glitchedCameraScript.frequency += 0.005f;
				glitchedCameraScript.inestability += 0.005f;
			} else if (timeDead >= 6.0f) {
				deadMenu.enabled = true;
				glitchedCameraScript.enabled = false;
			}
		}
	}

	public void RestartPress()
	{
		SceneManager.LoadScene ("testDesign");
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