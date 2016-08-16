using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Xml;

public class Loader : MonoBehaviour, LanguageListener {

    public static Loader instance = null;       //Singleton instance
    public TextAsset XMLAsset;

    private static AsyncOperation async;        //Async operation
    private static string actualScene;          //Current scene
    private static string lastScene;            //Last scene name
    private static string lastLevel = "None";   //Last level name

    //States
    private static bool loading = false;                //Is it loading?
    private static bool loaded = false;                 //It has finished loading
    private static bool allowLoadingToFinish = false;   //Allow the player to continue

    //Configuration
    private static bool interfaceLoad = false;          //It's the interface needed for loading?
    private static bool automaticLoad = false;          //Does it need player input to continue after finishing?

    //UI
    private GameObject loadingUI;           //Direct reference to the loading UI
    public Text text;                       //Screen text
    public Text percent;                    //Screen percent text
    public AudioClip confirmSound;          //Confirm sound

    private float nextLine = 0;             //When the next line will be written

    private string[] phrases = {"Phases not loaded."};
    private string Tloading = "";
    private string TpressAnyButton = "";

    void Awake()
    {
        //Check if there is already an instance of Loader
        if (instance == null){
            //if not, set it to this.
            instance = this;
            loadingUI = transform.GetChild(0).gameObject;
            DontDestroyOnLoad(gameObject);
            text.text = "*** Commondore 64 Basic V2 **** \n\n\n\n\n\n\n\n\n\n\n\n\n\n";
            actualScene = SceneManager.GetActiveScene().name;
        }
        //If instance already exists:
        else if (instance != this)
        {
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);
        }

        Configuration.addLanguageListener(this);

	}

    void OnDestroy()
    {
        Configuration.removeLanguageListener(this);
    }

    void Update(){

        if (loading){

            if (interfaceLoad){

                //Interface load

                //If it has finished loading
                if (loaded){

                    //If we're now in the next scene
                    if (async.isDone){
                        Time.timeScale = 1.0f;
                        loading = false;
                        loadingUI.SetActive(false);
                    
                    }
                    //If it is allowed to end loading
                    else if (allowLoadingToFinish) {

                        //If it's automatic or it isn't but the player is pressing a button
                        if (automaticLoad || (!automaticLoad && InputManager.ActiveDevice.AnyButton.WasPressed))
                        {
                            //Allow to load
                            async.allowSceneActivation = true;
                            SoundManager.instance.setAllowNewSounds(true);
                            SoundManager.instance.PlaySingle(confirmSound);
                        }

                    }

                }
                //If loading hasn't ended
                else if (async.progress >= 0.9f)
                {
                    //On 90%
                    loaded = true;
                    percent.text = TpressAnyButton;
                }
                //Lower than 90%
                else
                {
                    int percnt = (int)((100 * async.progress) + (10 * (async.progress / 0.9f)));
                    percent.text = Tloading + ": " + percnt + "%";

                    if (Time.unscaledTime > nextLine)
                    {
                        text.text += "\n" + phrases[Random.Range(0, phrases.Length)] + ".";
                        nextLine = Time.unscaledTime + Random.Range(0.5f, 2.0f);
                    }
                }


            } else {

                //Non-interface load

                //If it has finished loading
                if (loaded)
                {

                    //If it is allowed to end loading
                    if (allowLoadingToFinish)
                    {

                        //If it's automatic or it isn't but the player is pressing a button
                        if (automaticLoad || (!automaticLoad && InputManager.ActiveDevice.AnyButton.WasPressed))
                        {
                            //If it's automatic
                            async.allowSceneActivation = true;
                            SoundManager.instance.MuteAll();
                            loading = false;
                        }

                    }

                }
                //If loading hasn't ended
                else if (async.progress >= 0.9f)
                {
                    //On 90%
                    loaded = true;
                }
            
            }
        }

    }

    public void SetTexts()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(XMLAsset.text);

        XmlNodeList texts = xmlDoc.SelectNodes("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/T");

        phrases = new string[texts.Count];

        //We add the lines to the message list 
        for (int i = 0; i < texts.Count; i++)
        {
            phrases[i] = texts[i].InnerText;
        }

        Tloading = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/I[@id=\"loading\"]").InnerText;
        TpressAnyButton = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/I[@id=\"continue\"]").InnerText;
    }

    //Loads a given scene.
    //Level = The scene we're GOING TO is a level
    //Use interface = If active, the loading screen will be visible.
    //Is automatic = If inactive, the player will need to push any button to continue after it has finished loading to go to the next scene.
    //Allowed = If inactive, the next scene will load but an outside allowToFinish() call will be needed to go to the next scene.
    public static void LoadScene(string levelName, bool level, bool useInterface = true, bool isAutomatic = false, bool allowed = true)
    {
        if (!loading && actualScene != levelName){

            if (level)
            {
                lastLevel = levelName;
                ScoreManager.instance.RestartValues();
            }

            sceneLoading(levelName, useInterface, isAutomatic, allowed);
        }
    }

    public static void ReloadScene(bool useInterface = true, bool isAutomatic = false, bool allowed = true)
    {
        if (!loading){
            ScoreManager.instance.RestartValues();
            sceneLoading(actualScene, useInterface, isAutomatic, allowed);
        }
    }

    private static void sceneLoading(string levelName, bool useInterface = true, bool isAutomatic = false, bool allowed = true)
    {
        EventSystem.current.SetSelectedGameObject(null);

        lastScene = actualScene;
        actualScene = levelName;
        async = SceneManager.LoadSceneAsync(levelName);

        loading = true;
        loaded = false;
        async.allowSceneActivation = false;

        if (useInterface)
        {
            interfaceLoad = true;
            Time.timeScale = 0.0f;
            SoundManager.instance.MuteAll();
            SoundManager.instance.setAllowNewSounds(false);
        }
        else
        {
            interfaceLoad = false;
        }

        if (isAutomatic)
        {
            automaticLoad = true;
        }
        else
        {
            automaticLoad = false;
        }

        if (allowed)
        {
            allowLoadingToFinish = true;
        }
        else
        {
            allowLoadingToFinish = false;
        }

        instance.prepareLoading();

    }

    public void prepareLoading()
    {

        if (interfaceLoad){
            loadingUI.SetActive(true);
            percent.text = Tloading + ": 0%";
        }

    }

    public static void allowToFinish(){
        allowLoadingToFinish = true;
    }

    public static string getLastScene(){
        return lastScene;
    }
    public static string getLastLevel(){
        return lastLevel;
    }
}
