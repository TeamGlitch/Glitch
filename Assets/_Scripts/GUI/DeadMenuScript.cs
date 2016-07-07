using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeadMenuScript : MonoBehaviour {

	private Canvas deadMenu;
	private Button restartButton;
	private Button menuButton;
    private Button continueButton;
    private Image respawnObject;

    public Player player;

    private int timesDead = 0;

	public GameObject playerPowers;
	public GlitchOffsetCamera glitchedCameraScript;

    private float timeDead;

    //Saved pre-death camera values
    private int divisions;
    private float intensity;
    private float frequency;
    private float inestability;

    public AdvanceBarScript glitchBar;
    public AdvanceBarEnemies enemyBar;

	// Use this for initialization
	void Awake () {

		deadMenu = GetComponent<Canvas>();
		restartButton = transform.FindChild("Restart Game").GetComponent<Button> ();
		menuButton = transform.FindChild("Main Menu").GetComponent<Button>();
        continueButton = transform.FindChild("Continue").GetComponent<Button>();
        respawnObject = transform.FindChild("Respawn_object").GetComponent<Image>();
		gameObject.SetActive(false);
		playerPowers.SetActive (false);
	}

	// Update is called once per frame
	void Update () {
		if (deadMenu.enabled == false) {

			timeDead += Time.deltaTime;
			if (timeDead >= 2.0f && timeDead < 4.0f) {

                //TODO: Make time-oriented

				if(glitchedCameraScript.divisions < 50)
					glitchedCameraScript.divisions += 1;
				glitchedCameraScript.intensity += 0.05f;
				glitchedCameraScript.frequency += 0.005f;
				glitchedCameraScript.inestability += 0.005f;

			} else if (timeDead >= 6.0f) {

                timesDead++;

                int fibonacciValue = fib(timesDead + 1);
                fibonacciValue *= 10;

                continueButton.gameObject.GetComponent<Text>().text = "Continue (    x " + fibonacciValue + " )";

                Vector3 nPosition = respawnObject.rectTransform.anchoredPosition;
                int digits = (int) Mathf.Floor(Mathf.Log10(fibonacciValue) + 1);
                nPosition.x = (-27.25f * digits) + 150.5f;
                respawnObject.rectTransform.anchoredPosition = nPosition;

                if (player.items >= fibonacciValue)
                {
                    continueButton.Select();
                }
                else
                {
                    restartButton.Select();
                    continueButton.interactable = false;

                    Navigation nav = menuButton.navigation;
                    nav.selectOnUp = null;
                    nav.selectOnDown = null;
                    menuButton.navigation = nav;

                    nav = restartButton.navigation;
                    nav.selectOnUp = null;
                    nav.selectOnDown = null;
                    restartButton.navigation = nav;
                }
                
				deadMenu.enabled = true;
				glitchedCameraScript.enabled = false;

			}
		}
	}

	public void RestartPress()
	{
        Loader.ReloadScene();
	}

	public void MenuPress()
	{
        Loader.LoadScene("menu", false, true, true);
	}

    public void ContinuePress(){

        glitchedCameraScript.divisions  = divisions;
        glitchedCameraScript.intensity  = intensity;
        glitchedCameraScript.frequency  = frequency;
        glitchedCameraScript.inestability  = inestability;

        Vector3 checkpointPosition = player.lastCheckPoint.gameObject.transform.position;
        float begin = glitchBar.slider.minValue;
        float end = glitchBar.slider.maxValue;
        float percent = (checkpointPosition.x - begin) / (end - begin);

        enemyBar.Reanimated(percent);

        player.ContinueAfterDeath(fib(timesDead + 1) * 10);
    }

	public void PlayerDead()
	{
		glitchedCameraScript.enabled = true;
		deadMenu.enabled = false;
		timeDead = 0.0f;
        divisions = glitchedCameraScript.divisions;
        intensity = glitchedCameraScript.intensity;
        frequency = glitchedCameraScript.frequency;
        inestability = glitchedCameraScript.inestability;
	}

    static int fib(int n)
    {
        int fib0 = 0, fib1 = 1;
        for (int i = 2; i <= n; i++)
        {
            int tmp = fib0;
            fib0 = fib1;
            fib1 = tmp + fib1;
        }
        return (n > 0 ? fib1 : 0);
    }

}