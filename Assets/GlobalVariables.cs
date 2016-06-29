using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GlobalVariables : MonoBehaviour {

    private static string sceneName;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetName()
    {
        sceneName = SceneManager.GetActiveScene().name;
    }

    public static string GetName()
    {
        return sceneName;
    }
}
