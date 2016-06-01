using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FrameScript : MonoBehaviour {

    public GameObject[] go;
    public GameObject powers;
    public PlayerController player;

    void FrameTrigger()
    {
        int max = go.Length;
        for (int i = 0; i < max; i++)
        {
            go[i].SetActive(true);
        }

        powers.SetActive(true);
        player.enabled = true;
    } 
}
