﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ToFinalScene : MonoBehaviour
{

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Boss Stage");
        }
    }
}
