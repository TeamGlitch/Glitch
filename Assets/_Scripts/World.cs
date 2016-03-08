﻿using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
    public bool slow = false;
    public PlayerController player;
    public Camera mainCamera;
    public GameObject powers;
    public GameObject gui;

    void Start()
    {
		// We begin the game activating camera and movements of player
        mainCamera.gameObject.SetActive(true);
        gui.SetActive(true);
        powers.SetActive(true);
        player.enabled = true;
    }
}
