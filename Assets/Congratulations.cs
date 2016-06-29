using UnityEngine;
using System.Collections;

public class Congratulations : MonoBehaviour {
    public GlobalVariables global;

    private float endGame = 5.0f;

    void Update()
    {
        endGame -= Time.deltaTime;
        if (endGame <= 0.0f)
        {
            LoadScene();
        }

    }

    public void LoadScene(){
        if (GlobalVariables.GetName() == "Boss Stage")
        {
            Loader.LoadScene("menu");
        }
        else
        {
            Loader.LoadScene("Boss Stage");
        }
    }
}
