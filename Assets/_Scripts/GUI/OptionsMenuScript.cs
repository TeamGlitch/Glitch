using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsMenuScript : MonoBehaviour {

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

	void Start () {
        optionsMenu.SetActive(false);
        helpMenu.SetActive(false);
        graphicsMenu.SetActive(false);
        audioMenu.SetActive(false);
        creditsMenu.SetActive(false);
	}

    public void Enable(){
        optionsMenu.SetActive(true);
        ControlsButton.Select();
    }

    public void Disable(){
        optionsMenu.SetActive(false);
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
        SoundManager.instance.musicSource.volume = musicSlider.value;
    }

    public void ChangeSoundVolume()
    {
        for(int i=0; i < SoundManager.instance.efxSources.Length; ++i)
        {
            SoundManager.instance.efxSources[i].volume = soundSlider.value;
        }

        SoundManager.instance.PlaySingle(confirmSound);
    }

    public void ChangePan()
    {

        SoundManager.instance.musicSource.panStereo = panSlider.value;

        for (int i = 0; i < SoundManager.instance.efxSources.Length; ++i)
        {
            SoundManager.instance.efxSources[i].panStereo = panSlider.value;
        }
    }

    public void ChangeSpeakerMode()
    {

        float musicTime = -1;

        if (SoundManager.instance.musicSource.isPlaying)
        {
            musicTime = SoundManager.instance.musicSource.time;
        }

        AudioConfiguration config = AudioSettings.GetConfiguration();

        switch (speakersMode.value)
        {

            case 0: config.speakerMode = AudioSpeakerMode.Mono; break;
            case 1: config.speakerMode = AudioSpeakerMode.Stereo; break;
            case 2: config.speakerMode = AudioSpeakerMode.Quad; break;
            case 3: config.speakerMode = AudioSpeakerMode.Surround; break;
            case 4: config.speakerMode = AudioSpeakerMode.Mode5point1; break;
            case 5: config.speakerMode = AudioSpeakerMode.Mode7point1; break;
            case 6: config.speakerMode = AudioSpeakerMode.Prologic; break;

        }

        AudioSettings.Reset(config);

        if (musicTime != -1)
        {
            SoundManager.instance.musicSource.Play();
            SoundManager.instance.musicSource.time = musicTime;
        }

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
