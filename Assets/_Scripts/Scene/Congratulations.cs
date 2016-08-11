using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Congratulations : MonoBehaviour {

    void Start()
    {
        GetComponent<Animation>().Play();
    }

    public void ReturnToMenu()
    {
        Loader.LoadScene("menu", false, false, true, true);
    }
}
