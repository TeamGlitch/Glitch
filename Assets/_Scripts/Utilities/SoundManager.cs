using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{

    public AudioSource musicSource;
    public AudioSource[] efxSources;
    private int currentAudioSource = 0;
    public static SoundManager instance = null;

    private bool allowNewSounds = true;

    // Use this for initialization
    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
        {
            instance.musicSource.clip = this.musicSource.clip;
            instance.musicSource.Stop();
            instance.musicSource.Play();
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);

    }

    //Used to play single sound clips.
    public void PlaySingle(AudioClip clip)
    {
        if (allowNewSounds){

            int firstAudioSource = currentAudioSource;

            do
            {
                ++currentAudioSource;
                if (currentAudioSource >= efxSources.Length)
                    currentAudioSource = 0;
            } while (efxSources[currentAudioSource].isPlaying && firstAudioSource != currentAudioSource);

            efxSources[currentAudioSource].clip = clip;
            efxSources[currentAudioSource].Play();

        }

    }

    public void ChangeMusicSpeed(float speed)
    {
        musicSource.pitch = speed;
    }

    public void MuteAll()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();


        for (int i = 0; i < efxSources.Length; i++)
        {
            if (efxSources[i].isPlaying)
            {
                efxSources[i].Stop();
            }


        }

    }

    public void setAllowNewSounds(bool value){
        allowNewSounds = value;
    }

    public bool getAllowNewSounds()
    {
        return allowNewSounds;
    }

}
