using UnityEngine;
using UnityEngine.UI;
using InControl;
using System.Xml;

public class PauseScript : MonoBehaviour, LanguageListener {

	public GameObject playerPowers;

	public GameObject pauseMenu;
	public Button resumeButton;
	public Button restartButton;
	public Button menuButton;

    public TextAsset XMLAsset;

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

        XmlNodeList texts = xmlDoc.SelectNodes("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Group[@id = \"Pause\"]/UI");

        string menuName;
        Transform menu, element;
        Text elementText;
        for (int i = 0; i < texts.Count; i++)
        {
            menuName = texts[i].Attributes["id"].Value;
            menu = transform.FindChild(menuName);

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


    }

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
        Loader.ReloadScene();
	}

	public void menuPress()
	{
		Time.timeScale = 1.0f;
        Loader.LoadScene("menu", true, false, true, true);
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