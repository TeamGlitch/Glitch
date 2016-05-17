using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public AudioSource musicSource;
    public AudioSource efxSource1;
    public AudioSource efxSource2;
    public AudioSource efxSource3;
    public AudioSource efxSource4;
    public AudioSource efxSource5;
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
        if(!efxSource1.isPlaying)
        {
            //Set the clip of our efxSource audio source to the clip passed in as a parameter.
            efxSource1.clip = clip;
            //Play the clip.
            efxSource1.Play();
        }
        else if(!efxSource2.isPlaying)
        {
            //Set the clip of our efxSource audio source to the clip passed in as a parameter.
            efxSource2.clip = clip;
            //Play the clip.
            efxSource2.Play();
        }
        else if (!efxSource3.isPlaying)
        {
            //Set the clip of our efxSource audio source to the clip passed in as a parameter.
            efxSource3.clip = clip;
            //Play the clip.
            efxSource3.Play();
        }
        else if (!efxSource4.isPlaying)
        {
            //Set the clip of our efxSource audio source to the clip passed in as a parameter.
            efxSource4.clip = clip;
            //Play the clip.
            efxSource4.Play();
        }
        else if (!efxSource5.isPlaying)
        {
            //Set the clip of our efxSource audio source to the clip passed in as a parameter.
            efxSource5.clip = clip;
            //Play the clip.
            efxSource5.Play();
        }
        else
        {
            Debug.Log("We need more sound sources bitches");
        }
    }

    public void ChangeMusicSpeed(float speed)
    {
        musicSource.pitch = speed;
    }
}
