﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;

public class DeadMenuScript : MonoBehaviour, LanguageListener {

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

    public TextAsset XMLAsset;
    private string continueText;

	// Use this for initialization
	void Awake () {

		deadMenu = GetComponent<Canvas>();
		restartButton = transform.FindChild("Restart Game").GetComponent<Button> ();
		menuButton = transform.FindChild("Main Menu").GetComponent<Button>();

        Transform continueT = transform.FindChild("Continue");
        continueButton = continueT.GetComponent<Button>();
        respawnObject = continueT.FindChild("Respawn_object").GetComponent<Image>();

		gameObject.SetActive(false);
		playerPowers.SetActive (false);
	}

    void Start()
    {
        SetTexts();
        Configuration.addLanguageListener(this);
    }
    void OnDestroy()
    {
        Configuration.removeLanguageListener(this);
    }

    public void SetTexts()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(XMLAsset.text);

        XmlNodeList texts = xmlDoc.SelectNodes("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Group[@id = \"GameOver\"]/UI");

        string menuName;
        Transform menu, element;
        Text elementText;
        for (int i = 0; i < texts.Count; i++)
        {
            menuName = texts[i].Attributes["id"].Value;
            menu = transform.parent.FindChild(menuName);

            if (menu != null)
            {
                for (int z = 0; z < texts[i].ChildNodes.Count; z++)
                {
                    element = menu.FindChild(texts[i].ChildNodes[z].Attributes["id"].Value);
                    if (element != null)
                    {
                        elementText = element.GetComponent<Text>();
                        if (elementText != null)
                        {
                            elementText.text = texts[i].ChildNodes[z].InnerText;
                        }
                        else
                        {
                            print(texts[i].ChildNodes[z].Attributes["id"].Value + " on " + texts[i].Attributes["id"].Value + " doesn't have a Text.");
                        }
                    }
                    else
                    {
                        print(texts[i].ChildNodes[z].Attributes["id"].Value + " not found on " + texts[i].Attributes["id"].Value + ".");
                    }
                } // ENDFOR
            }
            else
            {
                print("Menu " + texts[i].Attributes["id"].Value + " not found.");
            }
        }

        continueText = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Group[@id = \"GameOver\"]/UI[@id = \"Dead Menu\"]/I[@id = \"Continue\"]").InnerText;


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

                continueButton.gameObject.GetComponent<Text>().text = continueText + " (    x " + fibonacciValue + " )";

                Vector3 nPosition = respawnObject.rectTransform.anchoredPosition;
                int digits = (int) Mathf.Floor(Mathf.Log10(fibonacciValue) + 1);
                nPosition.x = (-27.25f * digits) + 150.5f;
                respawnObject.rectTransform.anchoredPosition = nPosition;

                if (player.items >= fibonacciValue)
                {
                    continueButton.interactable = true;
                    continueButton.Select();
                }
                else
                {
                    restartButton.Select();

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
        Loader.LoadScene("menu", false, false, true, true);
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
        continueButton.interactable = false;
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