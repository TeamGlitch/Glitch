using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdvanceBarEnemies : MonoBehaviour {

    public enum endtimeState
    {
        NOT_MOVING,
        MOVING,
        POWER_DOWNING,
        SHOWING_MESSAGE,
        DEATH
    };

    private endtimeState state;

    private const float maxTime = 0.3f * 60f;
    private float time = 0.0f;
    private float stateChange = 0f;

    public Player player;

    public Slider slider;
    public Canvas powerDown;
    public Image blackScreen;
    public Text levelCompleteText;

    public AudioClip powerDownSound;

    void Start()
    {
        slider.maxValue = maxTime;
        slider.minValue = 0.0f;
        state = endtimeState.MOVING;
    }

    void Update()
    {
        switch(state){

            case endtimeState.MOVING:
                //TODO: Se deberia ralentizar con slowfps?
                //TODO: Funciona durante el scroll inicial del nivel. Probablemente también en pantallas de game over y alguna más. Usar NO_MOVING
                //TODO: Aviso cuando queda menos de 30 segundos?

                if (slider.value < maxTime)
                {
                    slider.value += Time.deltaTime;
                }
                else
                {
                    state = endtimeState.POWER_DOWNING;
                    SoundManager.instance.musicSource.Pause();
                    SoundManager.instance.PlaySingle(powerDownSound);
                    powerDown.enabled = true;
                    blackScreen.color = new Color(0, 0, 0, 0);
                    levelCompleteText.enabled = false;
                    stateChange = Time.time;
                }
                break;

            case endtimeState.POWER_DOWNING:

                //TODO: Congelar personaje? Hacerle inmortal?

                float percent = (Time.time - stateChange) / 2.5f;

                if (percent > 1) { 
                    percent = 1;
                    state = endtimeState.SHOWING_MESSAGE;
                    levelCompleteText.enabled = true;
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
