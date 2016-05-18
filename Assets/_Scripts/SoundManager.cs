using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public AudioSource musicSource;
    public AudioSource efxSource;
    public static SoundManager instance = null;        


	// Use this for initialization
	void Awake ()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy (gameObject);
            
        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad (gameObject);
	}
	
    //Used to play single sound clips.
    public void PlaySingle(AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        efxSource.clip = clip;
            
        //Play the clip.
        efxSource.Play ();
    }
}
