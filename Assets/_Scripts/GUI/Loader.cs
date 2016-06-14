using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using InControl;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {

    public static Loader instance = null;   //Singleton instance

    private static AsyncOperation async;    //Async operation
    private static string actualLevel;      //Currently loadingLevel
    private static bool loading = false;    //Is it loading?

    public Text text;                       //Screen text
    public Text percent;                    //Screen percent text

    private float nextLine = 0;                         //When the next line will be written
    private GameObject child;                           //Direct reference to the loading UI
    private bool waitingForPlayerContinue = false;      //Waiting for player input when "Press any key to continue"

    private string[] phrases = { "Loading enemies hostility", "Ensuring ragequit situations", "Rendering stereotypical hacker binary patterns",
                               "Sharpening enemies weapons", "Coordinating IA stupidity", "Leaking memory", "Compiling innecesary break commands",
                               "Making noise for no reason", "Retrieving nostalgia", "Making bad design decisions", "Pretending actual bugs are intended bugs",
                               "Loading loading screens", "Initializing recursive recursive recursive functions functions functions", 
                               "Making up false loading operations", "Looking at GitHub to see who made a code mistake"};
	
	// Update is called once per frame
    void Awake()
    {
        //Check if there is already an instance of Loader
        if (instance == null){
            //if not, set it to this.
            instance = this;
            child = transform.GetChild(0).gameObject;
        }
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        text.text = "*** Commondore 64 Basic V2 **** \n\n\n\n\n\n\n\n\n\n\n\n\n\n";
	}

    void Update(){

        if (loading){
			//Waiting for player input
			if (waitingForPlayerContinue) {
				if (InputManager.ActiveDevice.AnyButton.WasPressed) {
					async.allowSceneActivation = true;
					loading = false;
					child.SetActive(false);
				}
			//Loading ended
			} else if (async.progress >= 0.9f){
				waitingForPlayerContinue = true;
				percent.text = "Press any button to continue";
            }
			//Loading
            else {
                int percnt = (int) ((100 * async.progress) + (10 * (async.progress / 0.9f)));
                percent.text = "Now Loading: " + percnt + "%";

                if (Time.time > nextLine)
                {
                    text.text += "\n" + phrases[Random.Range(0, phrases.Length)] + ".";
                    nextLine = Time.time + Random.Range(0.5f, 2.0f);
                }
            }
        }

    }

    public static void LoadScene(string levelName)
    {
        if (!loading && actualLevel != levelName){
            loading = true;
            actualLevel = levelName;
            async = SceneManager.LoadSceneAsync(levelName);
			async.allowSceneActivation = false;
            instance.prepareLoading();
        }
    }

    public void prepareLoading()
    {
        child.SetActive(true);
        percent.text = "Now Loading: 0%";
		waitingForPlayerContinue = false;
    }
}
