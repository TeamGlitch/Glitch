﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdvanceBarEnemies : MonoBehaviour {

    private const float maxTime = 300.0f;

    public Slider slider;

    private float time = 0.0f;

    void Start()
    {
        slider.maxValue = maxTime;
        slider.minValue = 0.0f;
    }

    void Update()
    {
        slider.value += Time.deltaTime;
        if (slider.value >= maxTime)
        {
            SceneManager.LoadScene("menu");
        }
    }
}
