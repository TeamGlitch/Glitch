using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdvanceBarEnemies : MonoBehaviour {

    private const float maxTime = 20f * 60f;

    public Slider slider;

    private float time = 0.0f;

    void Start()
    {
        slider.maxValue = maxTime;
        slider.minValue = 0.0f;
    }

    void Update()
    {
        //TODO: Se deberia ralentizar con slowfps?
        //TODO: Funciona durante el scroll inicial del nivel. Probablemente también en pantallas de game over y alguna más.
        slider.value += Time.deltaTime;
        if (slider.value >= maxTime)
        {
            SceneManager.LoadScene("menu");
        }
    }
}
