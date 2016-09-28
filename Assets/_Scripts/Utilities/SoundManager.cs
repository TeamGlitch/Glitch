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
    public AudioSource PlaySingle(AudioClip clip)
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

            return efxSources[currentAudioSource];

        }

        return null;

    }

    public void setMusicVolume(float volume)
    {
        if (volume < 0)
            volume = 0;
        else if (volume > 1)
            volume = 1;

        musicSource.volume = volume;
    }

    public void ChangeMusicSpeed(float speed)
    {
        musicSource.pitch = speed;
    }

    public void setSoundVolume(float volume)
    {
        if (volume < 0)
            volume = 0;
        else if (volume > 1)
            volume = 1;

        for (int i = 0; i < efxSources.Length; ++i)
            efxSources[i].volume = volume;
    }

    public void setPan(float pan)
    {
        if (pan < -1)
            pan = -1;
        else if (pan > 1)
            pan = 1;

        musicSource.panStereo = pan;

        for (int i = 0; i < efxSources.Length; ++i)
            efxSources[i].panStereo = pan;
    }

    public void setMode(int mode)
    {
        float musicTime = -1;

        if (musicSource.isPlaying)
        {
            musicTime = musicSource.time;
        }

        AudioConfiguration config = AudioSettings.GetConfiguration();

        switch (mode)
        {

            case 0: config.speakerMode = AudioSpeakerMode.Mono; break;
            case 1: config.speakerMode = AudioSpeakerMode.Stereo; break;
            case 2: config.speakerMode = AudioSpeakerMode.Quad; break;
            case 3: config.speakerMode = AudioSpeakerMode.Surround; break;
            case 4: config.speakerMode = AudioSpeakerMode.Mode5point1; break;
            case 5: config.speakerMode = AudioSpeakerMode.Mode7point1; break;
            case 6: config.speakerMode = AudioSpeakerMode.Prologic; break;

        }

        AudioSettings.Reset(config);

        if (musicTime != -1)
        {
            musicSource.Play();
            musicSource.time = musicTime;
        }
    }

    public void Restart()
    {
        musicSource.Stop();
        musicSource.time = 0f;

        for (int i = 0; i < efxSources.Length; i++)
        {
            efxSources[i].Stop();
            efxSources[i].time = 0f;
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
