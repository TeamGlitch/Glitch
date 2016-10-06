using UnityEngine;
using UnityEngine.UI;
using InControl;
using System.Xml;
using UnityEngine.EventSystems;

public class PauseScript : MonoBehaviour, LanguageListener {

	public Player playerPowers;

    public AudioClip selectSound;
    public AudioClip confirmSound;

	public GameObject pauseMenu;
	public Button resumeButton;
	public Button restartButton;
	public Button menuButton;

    public TextAsset XMLAsset;

    void Start()
    {
        SetTexts();
        Configuration.addLanguageListener(this);

        ColorBlock cb;
        Color darkBrown;
        Color lightBrown;
        Color white;

        ColorUtility.TryParseHtmlString("#9F9F9FFF", out darkBrown);
        ColorUtility.TryParseHtmlString("#C8C8C8FF", out lightBrown);
        ColorUtility.TryParseHtmlString("#FFFFFFFF", out white);

        Toggle[] toggles = GetComponentsInChildren<Toggle>();
        for (int i = 0; i < toggles.Length; i++)
        {
            cb = toggles[i].colors;
            cb.normalColor = darkBrown;
            cb.highlightedColor = white;
            toggles[i].colors = cb;
        }

        Dropdown[] dropdowns = GetComponentsInChildren<Dropdown>();
        for (int i = 0; i < dropdowns.Length; i++)
        {
            cb = dropdowns[i].colors;
            cb.normalColor = darkBrown;
            cb.highlightedColor = white;
            dropdowns[i].colors = cb;

            Toggle toggle = dropdowns[i].transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<Toggle>();
            cb = toggle.colors;
            cb.normalColor = darkBrown;
            cb.highlightedColor = lightBrown;
            toggle.colors = cb;
        }



    }

    void OnDestroy()
    {
        Configuration.removeLanguageListener(this);
    }

    public void MakeSelectSound()
    {
        SoundManager.instance.PlaySingle(selectSound);
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
            SoundManager.instance.PlaySingle(confirmSound);
			playerPowers.enabled = true;
			Time.timeScale = 1.0f;
            gameObject.SetActive(false);
		}
	}

	public void restartPress()
	{
        SoundManager.instance.PlaySingle(confirmSound);
        Loader.ReloadScene();
	}

	public void menuPress()
	{
        SoundManager.instance.PlaySingle(confirmSound);
        Loader.LoadScene("menu", false, false, true, true);
    }

    public void optionPress(){

        SoundManager.instance.PlaySingle(confirmSound);
        pauseMenu.SetActive(false);
        resumeButton.enabled = false;
        restartButton.enabled = false;
        menuButton.enabled = false;

    }

    public void returnToMenu(){

        SoundManager.instance.PlaySingle(confirmSound);
        pauseMenu.SetActive(true);
        resumeButton.enabled = true;
        restartButton.enabled = true;
        menuButton.enabled = true;

        resumeButton.Select();

    }

    public void Pause()
    {
        playerPowers.enabled = false;
        Time.timeScale = 0.0f;
        for (int i = 0; i < SoundManager.instance.efxSources.Length; i++)
            SoundManager.instance.efxSources[i].Pause();
        resumeButton.Select();
    }

    public bool Unpause()
    {
        if (pauseMenu.activeSelf)
        {
            Time.timeScale = 1.0f;
            playerPowers.enabled = true;
            for (int i = 0; i < SoundManager.instance.efxSources.Length; i++)
                SoundManager.instance.efxSources[i].UnPause();
            return true;
        }
        
        return false;
    }
}