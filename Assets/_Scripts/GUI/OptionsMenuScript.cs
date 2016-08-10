using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Xml;

public class OptionsMenuScript : MonoBehaviour, LanguageListener {

    public GameObject optionsMenu;
    public Button ControlsButton;

    public GameObject helpMenu;
    public Button firstHelpButton;
    public Button keyboardButton;
    public Button gamepadButton;
    public Button backButtonInHelp;
    public Image keyboardImage;
    public Image gamepadImage;

    public GameObject graphicsMenu;
    public Dropdown qualityLevel;
    public Dropdown resolutions;
    public Toggle fullscreen;
    public Dropdown language;

    public GameObject audioMenu;
    public Slider musicSlider;
    public Slider soundSlider;
    public Slider panSlider;
    public Dropdown speakersMode;

    public GameObject creditsMenu;
    public Button firstCreditsButton;

    //SOUND//
    public AudioClip backSound;
    public AudioClip selectSound;
    public AudioClip confirmSound;

    public TextAsset XMLAsset;
    public Transform menusContainer;

	void Start () {
        optionsMenu.SetActive(false);
        helpMenu.SetActive(false);
        graphicsMenu.SetActive(false);
        audioMenu.SetActive(false);
        creditsMenu.SetActive(false);
        SetTexts();
        Configuration.addLanguageListener(this);
	}

    void OnDestroy()
    {
        Configuration.removeLanguageListener(this);
    }

    public void Enable(){
        optionsMenu.SetActive(true);
        ControlsButton.Select();
    }

    public void Disable(){
        optionsMenu.SetActive(false);
    }

    public void SetTexts()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(XMLAsset.text);

        XmlNodeList texts = xmlDoc.SelectNodes("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Group[@id = \"Options\"]/UI");

        string menuName;
        Transform menu, element;
        Text elementText;
        for (int i = 0; i < texts.Count; i++)
        {
            menuName = texts[i].Attributes["id"].Value;
            menu = menusContainer.FindChild(menuName);

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

    public void ReturnToOptions()
    {
        SoundManager.instance.PlaySingle(backSound);

        optionsMenu.SetActive(true);
        helpMenu.SetActive(false);
        graphicsMenu.SetActive(false);
        audioMenu.SetActive(false);
        creditsMenu.SetActive(false);

        ControlsButton.Select();
    }

    public void HelpPress()
    {
        SoundManager.instance.PlaySingle(confirmSound);
        helpMenu.SetActive(true);
        optionsMenu.SetActive(false);

        keyboardButton.enabled = false;
        gamepadButton.enabled = true;

        keyboardImage.enabled = true;
        gamepadImage.enabled = false;

        Navigation customNav = new Navigation();
        customNav.mode = Navigation.Mode.Explicit;
        customNav.selectOnUp = gamepadButton;
        backButtonInHelp.navigation = customNav;

        firstHelpButton.Select();
    }

    public void KeyboardPress()
    {
        SoundManager.instance.PlaySingle(confirmSound);
        keyboardButton.enabled = false;
        gamepadButton.enabled = true;
        keyboardImage.enabled = true;
        gamepadImage.enabled = false;

        Navigation customNav = new Navigation();
        customNav.mode = Navigation.Mode.Explicit;
        customNav.selectOnUp = gamepadButton;
        backButtonInHelp.navigation = customNav;

        gamepadButton.Select();
    }

    public void GamepadPress()
    {
        SoundManager.instance.PlaySingle(confirmSound);
        keyboardButton.enabled = true;
        gamepadButton.enabled = false;
        keyboardImage.enabled = false;
        gamepadImage.enabled = true;

        Navigation customNav = new Navigation();
        customNav.mode = Navigation.Mode.Explicit;
        customNav.selectOnUp = keyboardButton;
        backButtonInHelp.navigation = customNav;

        keyboardButton.Select();
    }

    public void GraphicsPress()
    {

        SoundManager.instance.PlaySingle(confirmSound);
        graphicsMenu.SetActive(true);
        optionsMenu.SetActive(false);

        qualityLevel.options.Clear();
        string[] names = QualitySettings.names;
        for (int i = 0; i < names.Length; i++)
        {
            Dropdown.OptionData entry = new Dropdown.OptionData();
            entry.text = names[i];
            qualityLevel.options.Add(entry);
        }
        qualityLevel.value = QualitySettings.GetQualityLevel();

        resolutions.options.Clear();
        Resolution[] resolutionList = Screen.resolutions;
        for (int i = 0; i < resolutionList.Length; i++)
        {
            Dropdown.OptionData entry = new Dropdown.OptionData();
            entry.text = resolutionList[i].width + "x" + resolutionList[i].height;
            resolutions.options.Add(entry);

            if (resolutionList[i].width == Screen.width &&
                resolutionList[i].height == Screen.height)
            {
                resolutions.value = i;
            }
        }

        if (Screen.fullScreen)
            fullscreen.isOn = true;
        else
            fullscreen.isOn = false;

        resolutions.Select();

        if (Configuration.getLanguage() == "Spanish") language.value = 1;
        else language.value = 0;
        //TODO: HERE FOR MORE LANGS

    }


    public void ChangeResolutionFullscreen()
    {
        Resolution newRes = Screen.resolutions[resolutions.value];
        Screen.SetResolution(newRes.width, newRes.height, fullscreen.isOn);
    }

    public void ChangeQuality()
    {
        QualitySettings.SetQualityLevel(qualityLevel.value);
    }

    public void ChangeLanguage()
    {
        string newLang;

        switch (language.value)
        {
            case 1: newLang = "Spanish"; break;
            default: newLang = "English"; break;
            //TODO: HERE FOR MORE LANGS
        }

        if (newLang != Configuration.getLanguage()){
            Configuration.setLanguage(newLang);
            Configuration.SaveConfiguration();
        }

    }

    public void AudioPress()
    {
        SoundManager.instance.PlaySingle(confirmSound);
        audioMenu.SetActive(true);
        optionsMenu.SetActive(false);

        musicSlider.value = SoundManager.instance.musicSource.volume;
        soundSlider.value = SoundManager.instance.efxSources[0].volume;
        panSlider.value = SoundManager.instance.musicSource.panStereo;

        if (AudioSettings.speakerMode == AudioSpeakerMode.Mono)
            speakersMode.value = 0;
        else if (AudioSettings.speakerMode == AudioSpeakerMode.Stereo)
            speakersMode.value = 1;
        else if (AudioSettings.speakerMode == AudioSpeakerMode.Quad)
            speakersMode.value = 2;
        else if (AudioSettings.speakerMode == AudioSpeakerMode.Surround)
            speakersMode.value = 3;
        else if (AudioSettings.speakerMode == AudioSpeakerMode.Mode5point1)
            speakersMode.value = 4;
        else if (AudioSettings.speakerMode == AudioSpeakerMode.Mode7point1)
            speakersMode.value = 5;
        else if (AudioSettings.speakerMode == AudioSpeakerMode.Prologic)
            speakersMode.value = 6;

        musicSlider.Select();
    }

    public void ChangeMusicVolume()
    {
        SoundManager.instance.setMusicVolume(musicSlider.value);
    }

    public void ChangeSoundVolume()
    {
        SoundManager.instance.setSoundVolume(soundSlider.value);
        SoundManager.instance.PlaySingle(confirmSound);
    }

    public void ChangePan()
    {
        SoundManager.instance.setPan(panSlider.value);
    }

    public void ChangeSpeakerMode()
    {
        SoundManager.instance.setMode(speakersMode.value);
    }

    public void SaveChangesOnAudio(){
        Configuration.SaveConfiguration();
    }

    public void CreditsMenu()
    {
        SoundManager.instance.PlaySingle(confirmSound);
        creditsMenu.SetActive(true);
        optionsMenu.SetActive(false);

        firstCreditsButton.Select();
    }

    public void MakeSelectSound()
    {
        SoundManager.instance.PlaySingle(selectSound);
    }
}
