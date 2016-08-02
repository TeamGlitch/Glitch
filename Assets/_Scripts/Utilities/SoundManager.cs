﻿using UnityEngine;
using System.Collections;
using System.Xml;
using System.Globalization;

public class SoundManager : MonoBehaviour
{

    public AudioSource musicSource;
    public AudioSource[] efxSources;
    private int currentAudioSource = 0;
    public static SoundManager instance = null;

    private bool allowNewSounds = true;

    private bool configurationLoaded = false;

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

    public void LoadConfiguration(){

        if(!configurationLoaded)
        {
            configurationLoaded = true;

            if(!tryToLoadConfiguration())
            {

                //If the file doesn't exist o there is a problem, restores the default values
                System.IO.File.WriteAllLines("config.xml", new string[] {
                "<!--?xml version=”1.0” encoding=”UTF-8”?-->",
                "<!--?xml version=”1.0” encoding=”UTF-8”?-->",
                "<confg>",
                "<music>0.5</music>",
                "<sfx>0.5</sfx>",
                "<pan>0</pan>",
                "<mode>1</mode>",
                "</confg>"
                });

                tryToLoadConfiguration();
            }
        }
    }


    public void SaveConfiguration(){

        AudioConfiguration config = AudioSettings.GetConfiguration();

        System.IO.File.WriteAllLines("config.xml", new string[] {
            "<!--?xml version=”1.0” encoding=”UTF-8”?-->",
            "<!--?xml version=”1.0” encoding=”UTF-8”?-->",
            "<confg>",
            "<music>" + musicSource.volume + "</music>",
            "<sfx>" + efxSources[0].volume + "</sfx>",
            "<pan>" + musicSource.panStereo + "</pan>",
            "<mode>" + config.speakerMode + "</mode>",
            "</confg>"
            });
    }

    private bool tryToLoadConfiguration(){

        if (!System.IO.File.Exists("config.xml"))
            return false;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc = new XmlDocument();

        string txt = System.IO.File.ReadAllText("config.xml");
        xmlDoc.LoadXml(txt);

        XmlNode node = xmlDoc.SelectSingleNode("/confg/music");
        if (node == null)
            return false;
        float value;
        if (!float.TryParse(node.InnerText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat, out value))
            return false;
        if(value < 0 || value > 1)
            return false;
        setMusicVolume(value);

        node = xmlDoc.SelectSingleNode("/confg/sfx");
        if (node == null)
            return false;
        if (!float.TryParse(node.InnerText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat, out value))
            return false;
        if (value < 0 || value > 1)
            return false;
        setSoundVolume(value);

        node = xmlDoc.SelectSingleNode("/confg/pan");
        if (node == null)
            return false;
        if (!float.TryParse(node.InnerText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat, out value))
            return false;
        if (value < -1 || value > 1)
            return false;
        setPan(value);

        node = xmlDoc.SelectSingleNode("/confg/mode");
        if (node == null)
            return false;
        int valueI;
        if (!int.TryParse(node.InnerText, out valueI))
            return false;
        if (valueI < 0 || valueI > 6)
            return false;
        setMode(valueI);

        return true;

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
