﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeadMenuScript : MonoBehaviour {

	private Canvas deadMenu;
	private Button restartButton;
	private Button menuButton;
    private Button continueButton;

    public Player player;

    private int timesDead = 0;

	public GameObject playerPowers;
	public GlitchOffsetCamera glitchedCameraScript;

	private float timeDead;

	// Use this for initialization
	void Awake () {

		deadMenu = GetComponent<Canvas>();
		restartButton = transform.FindChild("Restart Game").GetComponent<Button> ();
		menuButton = transform.FindChild("Main Menu").GetComponent<Button>();
        continueButton = transform.FindChild("Continue").GetComponent<Button>();
		gameObject.SetActive(false);
		playerPowers.SetActive (false);
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

                timesDead++;

                int fibonacciValue = fib(timesDead + 1);
                fibonacciValue *= 10;

                continueButton.gameObject.GetComponent<Text>().text = "Continue (   x " + fibonacciValue + " )"

                if (player.items >= fibonacciValue)
                {
                    continueButton.Select();
                }
                else
                {
                    restartButton.Select();
                    continueButton.interactable = false;
                }
                
				deadMenu.enabled = true;
				glitchedCameraScript.enabled = false;
			}
		}
	}

	public void RestartPress()
	{
        Loader.LoadScene("Level1");
	}

	public void MenuPress()
	{
        Loader.LoadScene("menu");
	}

	public void PlayerDead()
	{
		glitchedCameraScript.enabled = true;
		deadMenu.enabled = false;
		timeDead = 0.0f;
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