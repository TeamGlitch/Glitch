using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Xml;

public class AdvanceBarEnemies : MonoBehaviour, LanguageListener {

    public enum endtimeState
    {
        NOT_MOVING,
        MOVING,
        POWER_DOWNING,
        SHOWING_MESSAGE,
        DEATH,
        LEVEL_COMPLETE
    };

    private endtimeState state;
    private endtimeState lastStateBeforePause = endtimeState.MOVING;

    public float maxTime = 300.0f;
    private float time = 0.0f;
    private float stateChange = 0f;

    public Player player;
    public PlayerController playerController;

    public Slider slider;
    public Canvas powerDown;
    public Image blackScreen;
    public Text levelCompleteText;

    public AudioClip powerDownSound;
    public TextAsset XMLAsset;

    public EndPointScript endPoint;

    void Start()
    {
        slider.maxValue = maxTime;
        slider.minValue = 0.0f;
        state = endtimeState.NOT_MOVING;
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

        XmlNode texts = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Group[@id = \"LevelComplete\"]/UI[@id = \"LevelComplete\"]/I[@id = \"LevelCompleteText\"]");
        levelCompleteText.text = texts.InnerText;

    }

    public void Pause (bool pause)
    {
        if (pause)
        {
            if (state != endtimeState.NOT_MOVING)
                lastStateBeforePause = state;
            state = endtimeState.NOT_MOVING;
        }
        else
        {
            state = lastStateBeforePause;
        }
    }

    void Update()
    {
        if (state != endtimeState.LEVEL_COMPLETE && endPoint.enabled)
        {
            ScoreManager.instance.SetBasePoints(player.items);
            ScoreManager.instance.SetTimes(maxTime, time);
            ScoreManager.instance.SetRemaniningLives(player.lives);
            state = endtimeState.LEVEL_COMPLETE;
        }

        switch(state){

            case endtimeState.MOVING:

                if (slider.value < maxTime)
                {
                    slider.value += Time.deltaTime;
                }
                else
                {
                    if(player.lives > 0)
                    { 
                        state = endtimeState.POWER_DOWNING;
                        SoundManager.instance.musicSource.Pause();
                        SoundManager.instance.PlaySingle(powerDownSound);
                        powerDown.enabled = true;
                        blackScreen.color = new Color(0, 0, 0, 0);
                        levelCompleteText.enabled = false;
                        stateChange = Time.time;
                        playerController.allowMovement = false;
                    }
                }
                break;

            case endtimeState.POWER_DOWNING:

                float percent = (Time.time - stateChange) / 2.5f;

                if (percent > 1) { 
                    percent = 1;
                    state = endtimeState.SHOWING_MESSAGE;
                    levelCompleteText.enabled = true;
                    SetTexts();
                    stateChange = Time.time;
                }

                blackScreen.color = new Color(0, 0, 0, percent);
                break;

            case endtimeState.SHOWING_MESSAGE:

                float percent0 = (Time.time - stateChange) / 3f;

                if (percent0 > 1){
                    player.DecrementLives(3);
                    state = endtimeState.DEATH;
                }

                break;

        }
    }

    public void Reanimated(float percent){
        slider.value = slider.maxValue * percent;
        state = endtimeState.MOVING;
        powerDown.enabled = false;
        SoundManager.instance.musicSource.UnPause();
    }
}
