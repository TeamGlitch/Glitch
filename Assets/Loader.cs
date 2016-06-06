using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Loader : MonoBehaviour {

    public static Loader instance = null;

    public Text text;
    public Text percent;

    private static AsyncOperation async;
    private static Transform loaderTransform;
    private static bool loading = false;

    private float nextLine = 0;

    private string[] phrases = { "Loading enemies hostility", "Ensuring ragequit situations", "Rendering stereotypical hacker binary patterns" };
	
	// Update is called once per frame
    void Awake()
    {
        //Check if there is already an instance of Loader
        if (instance == null){
            //if not, set it to this.
            instance = this;
            loaderTransform = transform;
        }
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
	}

    void Update(){

        if (loading){
            if (async.isDone){
                loading = false;
                loaderTransform.GetChild(0).gameObject.SetActive(false);
            }
            else {
                int percnt = (int) ((100 * async.progress) + (10 * (async.progress / 0.9f)));
                percent.text = "Now Loading: " + percnt + "%";

                if (Time.time > nextLine)
                {
                    text.text += "\n" + phrases[Random.Range(0, phrases.Length)];
                    nextLine = Time.time + Random.Range(3.0f * 0, 9.0f * 0);
                }
            }
        }

    }

    public static void LoadScene(string levelName){
        if (!loading){
            loading = true;
            async = Application.LoadLevelAsync(levelName);
            loaderTransform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
