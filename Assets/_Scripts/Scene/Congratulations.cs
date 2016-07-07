using UnityEngine;
using System.Collections;

public class Congratulations : MonoBehaviour {
    public void ReturnToMenu(){
        if (Loader.getLastScene() == "Boss Stage")
        {
            Loader.LoadScene("menu", false, false, true, true);
        }
        else
        {
            Loader.LoadScene("Boss Stage", false);
        }
    }
}
