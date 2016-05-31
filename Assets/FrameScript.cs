using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FrameScript : MonoBehaviour {

    public GameObject[] go;

    void FrameTrigger()
    {
        for (int i = 0; i < go.Length; i++)
        {
            go[i].gameObject.SetActive(true);
        }
    } 
}
