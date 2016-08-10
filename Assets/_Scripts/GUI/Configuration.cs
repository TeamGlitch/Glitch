using UnityEngine;
using System.Collections;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;

public interface LanguageListener
{
    void SetTexts();
}

public class Configuration : MonoBehaviour {

    private static bool configurationLoaded = false;

    private static string lang = "English";
    private static List<LanguageListener> languageListeners = new List<LanguageListener>();

    public static void LoadConfiguration()
    {

        if (!configurationLoaded)
        {
            configurationLoaded = true;

            if (!tryToLoadConfiguration())
            {

                //If the file doesn't exist o there is a problem, restores the default values
                System.IO.File.WriteAllLines("config.xml", new string[] {
                "<!--?xml version=”1.0” encoding=”UTF-8”?-->",
                "<!--?xml version=”1.0” encoding=”UTF-8”?-->",
                "<confg>",
                "<lang>English</lang>",
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

    public static void SaveConfiguration()
    {

        AudioConfiguration config = AudioSettings.GetConfiguration();

        System.IO.File.WriteAllLines("config.xml", new string[] {
            "<!--?xml version=”1.0” encoding=”UTF-8”?-->",
            "<!--?xml version=”1.0” encoding=”UTF-8”?-->",
            "<confg>",
            "<lang>" + lang + "</lang>",
            "<music>" + SoundManager.instance.musicSource.volume + "</music>",
            "<sfx>" + SoundManager.instance.efxSources[0].volume + "</sfx>",
            "<pan>" + SoundManager.instance.musicSource.panStereo + "</pan>",
            "<mode>" + config.speakerMode + "</mode>",
            "</confg>"
            });
    }

    private static bool tryToLoadConfiguration()
    {

        if (!System.IO.File.Exists("config.xml"))
            return false;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc = new XmlDocument();

        string txt = System.IO.File.ReadAllText("config.xml");
        xmlDoc.LoadXml(txt);

        XmlNode node = xmlDoc.SelectSingleNode("/confg/lang");
        if (node == null)
            return false;
        if (node.InnerText != "English"
            && node.InnerText != "Spanish"
            && node.InnerText != "Catalan")
            return false;
        setLanguage(node.InnerText);

        node = xmlDoc.SelectSingleNode("/confg/music");
        if (node == null)
            return false;
        float value;
        if (!float.TryParse(node.InnerText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat, out value))
            return false;
        if (value < 0 || value > 1)
            return false;
        SoundManager.instance.setMusicVolume(value);

        node = xmlDoc.SelectSingleNode("/confg/sfx");
        if (node == null)
            return false;
        if (!float.TryParse(node.InnerText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat, out value))
            return false;
        if (value < 0 || value > 1)
            return false;
        SoundManager.instance.setSoundVolume(value);

        node = xmlDoc.SelectSingleNode("/confg/pan");
        if (node == null)
            return false;
        if (!float.TryParse(node.InnerText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat, out value))
            return false;
        if (value < -1 || value > 1)
            return false;
        SoundManager.instance.setPan(value);

        node = xmlDoc.SelectSingleNode("/confg/mode");
        if (node == null)
            return false;
        int valueI;
        if (!int.TryParse(node.InnerText, out valueI))
            return false;
        if (valueI < 0 || valueI > 6)
            return false;
        SoundManager.instance.setMode(valueI);

        return true;

    }


    //LANGUAGE
    public static string getLanguage()
    {
        return lang;
    }

    public static void setLanguage(string newLang)
    {
        lang = newLang;
        warnLanguageListeners();
    }

    public static void addLanguageListener(LanguageListener listener){
        languageListeners.Add(listener);
    }

    public static void removeLanguageListener(LanguageListener listener)
    {
        for(int i = languageListeners.Count - 1; i >= 0; i--)
        {
            if (languageListeners[i] == listener)
            {
                languageListeners.RemoveAt(i);
            }
        }
    }

    private static void warnLanguageListeners()
    {
        for (int i = languageListeners.Count - 1; i >= 0; i--)
        {
            languageListeners[i].SetTexts();
        }
    }




}
