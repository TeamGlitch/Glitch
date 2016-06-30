using UnityEngine;
using System.Collections;

public class Congratulations : MonoBehaviour {
    public void ReturnToMenu(){
        if (Loader.getLastLevel() == "Boss Stage")
        {
            Loader.LoadScene("menu", false, true, true);
        }
        else
        {
            Loader.LoadScene("Boss Stage");
        }
    }
}
