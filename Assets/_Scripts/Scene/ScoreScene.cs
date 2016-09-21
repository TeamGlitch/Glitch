using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml;
using InControl;

public class ScoreScene : MonoBehaviour, LanguageListener {

    //TODO: HOURGLASS PARTICLES
    //TODO: SOUND

    public enum scoreState
    {
        START,
        POINTS_SHOW_MULTIPLIER_1,
        POINTS_SHOW_MULTIPLIER_2,
        POINTS_INCREASING,
        POINTS_DONE,
        TIME_SHOW_MULTIPLIER_1,
        TIME_SHOW_MULTIPLIER_2,
        TIME_INCREASING,
        TIME_DONE,
        HEARTS_SHOW,
        HEARTS_REMOVE_1,
        HEARTS_REMOVE_2,
        HEARTS_REMOVE_3,
        HEARTS_REMOVED,
        ITEM_SHOW,
        ITEM_REMOVE_1,
        ITEM_REMOVE_2,
        ITEM_REMOVE_3,
        ITEMS_REMOVED,
        MEDAL_UP,
        MEDAL_DOWN,
        MEDALS_SHOW,
        NEW_RECORD,
        SCORE_SHOWING,
        GLITCH_WALKING,
        LOADING_LEVEL
    };

    public TextAsset XMLInterface;
    public TextAsset XMLAchievements;

    private bool skip;

    //UI REFERENCES
    public Text finalPoints;
    private int goalPoints = 0;
    private int actualPoints = 0;

    public Text pointsMultiplier;
    public Text pointsValue;

    public Text timeMultiplier;
    public Text timeValue;
    public RectTransform sandMask;
    private float maxTotal;
    public RectTransform corruptionMask;
    private float maxConsumed;

    public Image firstHeart;
    public Image secondHeart;
    public Image thirdHeart;
    private Sprite emptyHeart;
    public Text heartText;

    public Image firstItem;
    public Image secondItem;
    public Image thirdItem;
    private Sprite fullItem;
    public Text itemText;

    public GameObject MedalPanel;
    public GameObject AchievementMedal;
    private List<RectTransform> medals;

    public Text newHiscore;
    public InputField nameInput;

    //BUTTON REFERENCES
    public Button continueButton;
    public Button retryButton;
    public Button menuButton;

    private scoreState state;
    private RectTransform characterRT;
    private GameObject ScoreUI;

    private float timeLastState;
    private float desiredValue;
    private float actualValue;

    private AudioSource countingSource;
    public AudioClip attributeSound;
    public AudioClip countingSound;
    public AudioClip[] extraSound = new AudioClip[3];
    public AudioClip medalSound;
    public AudioClip penaltySound;
    public AudioClip newRecordSound;
    public AudioClip acceptSound;
    public AudioClip refuseSound;

	// Use this for initialization
	void Start () {

        ScoreUI = transform.FindChild("Score").gameObject;

        GameObject glitch = transform.FindChild("Character").gameObject;
        characterRT = glitch.GetComponent<RectTransform>();

        //Sets
        finalPoints.text = "0";

        pointsMultiplier.text = "";
        pointsValue.text = "";

        //Time

        timeMultiplier.text = "";
        timeValue.text = "";

        maxTotal = sandMask.offsetMax.y;
        maxConsumed = corruptionMask.offsetMax.y;

        float percentSpent = ScoreManager.instance.getTimeSpent() / ScoreManager.instance.getTotalTime();
        float percentLeft = 1 - percentSpent;

        sandMask.offsetMax = new Vector2(sandMask.offsetMax.x, maxTotal * percentLeft);
        corruptionMask.offsetMax = new Vector2(corruptionMask.offsetMax.x, maxConsumed - (maxConsumed * percentSpent));

        //-Time

        firstHeart.enabled = false;
        secondHeart.enabled = false;
        thirdHeart.enabled = false;
        heartText.text = "";

        firstItem.enabled = false;
        secondItem.enabled = false;
        thirdItem.enabled = false;
        itemText.text = "";

        AchievementMedal.SetActive(false);
        medals = new List<RectTransform>();

        state = scoreState.START;

        SetTexts();
        Configuration.addLanguageListener(this);

        continueButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);

        timeLastState = Time.time;

	}

    void OnDestroy()
    {
        Configuration.removeLanguageListener(this);
    }

    public void SetTexts()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(XMLInterface.text);

        continueButton.gameObject.GetComponent<Text>().text = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Group[@id = \"Score\"]/UI[@id = \"Score\"]/I[@id = \"Continue\"]").InnerText;
        retryButton.gameObject.GetComponent<Text>().text = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Group[@id = \"Score\"]/UI[@id = \"Score\"]/I[@id = \"Retry\"]").InnerText;
        menuButton.gameObject.GetComponent<Text>().text = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Group[@id = \"Score\"]/UI[@id = \"Score\"]/I[@id = \"Return\"]").InnerText;
        newHiscore.text = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Group[@id = \"Score\"]/UI[@id = \"Score\"]/I[@id = \"NewHiscore\"]").InnerText;
        nameInput.transform.GetChild(0).GetComponent<Text>().text = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Group[@id = \"Score\"]/UI[@id = \"Score\"]/I[@id = \"YourName\"]").InnerText;
    }

	// Update is called once per frame
	void Update () {

        if (InputManager.ActiveDevice.AnyButton.WasPressed &&
            state != scoreState.NEW_RECORD &&
            state != scoreState.SCORE_SHOWING &&
            state != scoreState.GLITCH_WALKING && 
            state != scoreState.LOADING_LEVEL)
        {
            skip = true;
        }
        else
        {
            skip = false;
        }

        if (actualPoints < goalPoints)
        {
            float increase = (goalPoints - actualPoints) * Time.deltaTime;

            if (increase < 200 * Time.deltaTime)
                increase = 200 * Time.deltaTime;

            actualPoints += (int)increase;

            if (actualPoints > goalPoints)
                actualPoints = goalPoints;

            finalPoints.text = string.Format("{0:#,###0.#}", Mathf.Round(actualPoints));
        }
        else if (actualPoints > goalPoints)
        {
            float decrease = (actualPoints - goalPoints) * Time.deltaTime;

            if (decrease < 200 * Time.deltaTime)
                decrease = 200 * Time.deltaTime;

            actualPoints -= (int)decrease;

            if (actualPoints < goalPoints)
                actualPoints = goalPoints;

            finalPoints.text = string.Format("{0:#,###0.#}", Mathf.Round(actualPoints));
        }

        do
        {

        switch(state)
        {
            case scoreState.START:
                if(timeOut(0.5f, scoreState.POINTS_SHOW_MULTIPLIER_1)){
                    pointsMultiplier.text = ScoreManager.instance.getBasePoints().ToString();
                    SoundManager.instance.PlaySingle(attributeSound);
                }
                break;

            case scoreState.POINTS_SHOW_MULTIPLIER_1:
                if (timeOut(0.5f, scoreState.POINTS_SHOW_MULTIPLIER_2))
                {
                    pointsMultiplier.text = ScoreManager.instance.getBasePoints().ToString() + " <color=#FFC300FF>x " + ScoreManager.instance.calculatePoints() + "</color>";
                    SoundManager.instance.PlaySingle(attributeSound);
                }
                break;

            case scoreState.POINTS_SHOW_MULTIPLIER_2:
                if (timeOut(0.5f, scoreState.POINTS_INCREASING))
                {
                    pointsValue.text = "0";
                    desiredValue = ScoreManager.instance.getPoints();
                    actualValue = 0;
                    goalPoints = (int)desiredValue;
                }
                break;

            case scoreState.POINTS_INCREASING:

                playCountingSound();

                if (actualValue < desiredValue)
                {

                    if (actualValue == 0) actualValue = 1;
                    else actualValue += 400f * Time.deltaTime;

                    if (actualValue > desiredValue)
                    {
                        actualValue = desiredValue;
                    }

                    pointsValue.text = (int)actualValue + "";

                }
                else
                {
                    timeOut(0, scoreState.POINTS_DONE); 
                }
                break;

            case scoreState.POINTS_DONE:
                if (timeOut(0.5f, scoreState.TIME_SHOW_MULTIPLIER_1))
                {
                    timeMultiplier.text = " <color=#FFC300FF>/ " + ScoreManager.instance.getTotalTime() + "</color>";
                    SoundManager.instance.PlaySingle(attributeSound);
                }
                break;

            case scoreState.TIME_SHOW_MULTIPLIER_1:
                if (timeOut(0.5f, scoreState.TIME_SHOW_MULTIPLIER_2))
                {
                    float rest = ScoreManager.instance.getTotalTime() - ScoreManager.instance.getTimeSpent();
                    timeMultiplier.text = (ScoreManager.instance.getTotalTime() - ScoreManager.instance.getTimeSpent()).ToString() + timeMultiplier.text;
                    SoundManager.instance.PlaySingle(attributeSound);
                }
                break;

            case scoreState.TIME_SHOW_MULTIPLIER_2:
                if (timeOut(0.5f, scoreState.TIME_INCREASING))
                {
                    timeValue.text = "x 0";
                    desiredValue = ScoreManager.instance.calculatePoints();
                    actualValue = 0;
                    goalPoints = (int)ScoreManager.instance.getPoints();
                }
                break;

            case scoreState.TIME_INCREASING:

                playCountingSound();

                if (actualValue < desiredValue)
                {

                    if (actualValue == 0) actualValue = 1;
                    else actualValue += 1.5f * Time.deltaTime;

                    if (actualValue > desiredValue)
                    {
                        actualValue = desiredValue;
                    }

                    timeValue.text = "x " + roundToTwo(actualValue);

                    float percentSpent = ScoreManager.instance.getTimeSpent() / ScoreManager.instance.getTotalTime();
                    float percentLeft = 1 - percentSpent;
                    percentSpent += ((actualValue / desiredValue) * percentLeft);
                    percentLeft = 1 - percentSpent;

                    sandMask.offsetMax = new Vector2(sandMask.offsetMax.x, maxTotal * percentLeft);
                    corruptionMask.offsetMax = new Vector2(corruptionMask.offsetMax.x, maxConsumed - (maxConsumed * percentSpent));

                }
                else
                {
                    timeOut(0, scoreState.TIME_DONE);
                }
                break;

            case scoreState.TIME_DONE:
                if (timeOut(0.7f, scoreState.HEARTS_SHOW))
                {
                    firstHeart.enabled = true;
                    secondHeart.enabled = true;
                    thirdHeart.enabled = true;

                    SoundManager.instance.PlaySingle(attributeSound);

                    if (ScoreManager.instance.getRemainingLives() < 2)
                    {
                        secondHeart.sprite = thirdHeart.sprite;
                    }
                    else if (ScoreManager.instance.getRemainingLives() > 2)
                    {
                        emptyHeart = thirdHeart.sprite;
                        thirdHeart.sprite = secondHeart.sprite;
                    }
                }
                break;

            case scoreState.HEARTS_SHOW:
                if (timeOut(0.7f, scoreState.HEARTS_REMOVE_1))
                {
                    ScoreManager.instance.calculatePoints();
                    goalPoints = (int)ScoreManager.instance.getPoints();

                    if (ScoreManager.instance.getRemainingLives() == 3)
                    {
                        thirdHeart.sprite = emptyHeart;
                        SoundManager.instance.PlaySingle(extraSound[0]);
                    }
                    else if (ScoreManager.instance.getRemainingLives() == 2)
                    {
                        secondHeart.sprite = thirdHeart.sprite;
                        SoundManager.instance.PlaySingle(extraSound[0]);
                    }
                    else
                    {
                        firstHeart.sprite = thirdHeart.sprite;
                        SoundManager.instance.PlaySingle(extraSound[0]);
                    }

                    heartText.text = "x 1.0";
                    heartText.fontSize = 20;
                }
                break;

            case scoreState.HEARTS_REMOVE_1:
                if (timeOut(0.7f, scoreState.HEARTS_REMOVED))
                {
                    if (ScoreManager.instance.getRemainingLives() == 3){
                        secondHeart.sprite = emptyHeart;
                        heartText.text = "x 1.25";
                        heartText.fontSize = 27;
                        state = scoreState.HEARTS_REMOVE_2;
                        SoundManager.instance.PlaySingle(extraSound[1]);
                    }
                    else if (ScoreManager.instance.getRemainingLives() == 2)
                    {
                        firstHeart.sprite = thirdHeart.sprite;
                        heartText.text = "x 1.25";
                        heartText.fontSize = 27;
                        state = scoreState.HEARTS_REMOVE_2;
                        SoundManager.instance.PlaySingle(extraSound[1]);
                    }
                    
                }
                break;

            case scoreState.HEARTS_REMOVE_2:
                if (timeOut(0.7f, scoreState.HEARTS_REMOVED))
                {
                    if (ScoreManager.instance.getRemainingLives() == 3)
                    {
                        firstHeart.sprite = emptyHeart;
                        heartText.text = "x 1.50";
                        heartText.fontSize = 40;
                        state = scoreState.HEARTS_REMOVE_3;
                        SoundManager.instance.PlaySingle(extraSound[2]);
                    }
                }
                break;

            case scoreState.HEARTS_REMOVE_3:
                timeOut(0.7f, scoreState.HEARTS_REMOVED);
                break;

            case scoreState.HEARTS_REMOVED:
                if (timeOut(0.0f, scoreState.ITEM_SHOW))
                {
                    firstItem.enabled = true;
                    secondItem.enabled = true;
                    thirdItem.enabled = true;

                    emptyHeart = thirdItem.sprite;
                    fullItem = firstItem.sprite;

                    SoundManager.instance.PlaySingle(attributeSound);

                    if (ScoreManager.instance.getColectionablesTaken() < 1)
                        firstItem.sprite = emptyHeart;
                    if (ScoreManager.instance.getColectionablesTaken() < 2)
                        secondItem.sprite = emptyHeart;
                    if (ScoreManager.instance.getColectionablesTaken() > 2)
                        thirdItem.sprite = fullItem;

                    itemText.text = "x 1.0";
                    itemText.fontSize = 15;
                }

                break;

            case scoreState.ITEM_SHOW:
                if (timeOut(0.7f, scoreState.ITEM_REMOVE_1))
                {
                    ScoreManager.instance.calculatePoints();
                    goalPoints = (int)ScoreManager.instance.getPoints();

                    if (ScoreManager.instance.getColectionablesTaken() > 0)
                    {
                        itemText.text = "x 1.5";
                        itemText.fontSize = 20;

                        SoundManager.instance.PlaySingle(extraSound[0]);

                        if (ScoreManager.instance.getColectionablesTaken() == 3)
                            thirdItem.sprite = emptyHeart;
                        else if (ScoreManager.instance.getColectionablesTaken() == 2)
                            secondItem.sprite = emptyHeart;
                        else if (ScoreManager.instance.getColectionablesTaken() == 1)
                            firstItem.sprite = emptyHeart;
                    }
                    else
                    {
                        state = scoreState.ITEMS_REMOVED;
                    }
                }
                break;

            case scoreState.ITEM_REMOVE_1:
                if (timeOut(0.7f, scoreState.ITEM_REMOVE_2))
                {
                    if (ScoreManager.instance.getColectionablesTaken() > 1)
                    {
                        itemText.text = "x 2.5";
                        itemText.fontSize = 27;

                        SoundManager.instance.PlaySingle(extraSound[1]);

                        if (ScoreManager.instance.getColectionablesTaken() == 3)
                            secondItem.sprite = emptyHeart;
                        else if (ScoreManager.instance.getColectionablesTaken() == 2)
                            firstItem.sprite = emptyHeart;
                    }
                    else
                    {
                        state = scoreState.ITEMS_REMOVED;
                    }
                }
                break;

            case scoreState.ITEM_REMOVE_2:
                if (timeOut(0.7f, scoreState.ITEM_REMOVE_3))
                {
                    if (ScoreManager.instance.getColectionablesTaken() > 2)
                    {
                        SoundManager.instance.PlaySingle(extraSound[2]);

                        itemText.text = "x 4.0";
                        itemText.fontSize = 40;
                        firstItem.sprite = emptyHeart;
                    }
                    else
                    {
                        state = scoreState.ITEMS_REMOVED;
                    }
                }
                break;

            case scoreState.ITEM_REMOVE_3:
                timeOut(0.7f, scoreState.ITEMS_REMOVED);
                break;

            case scoreState.ITEMS_REMOVED:
                if (timeOut(0.0f, scoreState.MEDALS_SHOW))
                {
                    float multiplier = 0;
                    float medalNumber = 1;

                    while (multiplier == 0 && ScoreManager.instance.phase != ScoreManager.pointsCalculationPhases.PENALTY)
                    {
                        multiplier = ScoreManager.instance.calculatePoints();
                    }

                    //If we won a medal
                    if (multiplier != 0)
                    {
                        state = scoreState.MEDAL_UP;
                        goalPoints = (int)ScoreManager.instance.getPoints();

                        if (ScoreManager.instance.phase == ScoreManager.pointsCalculationPhases.PENALTY)
                        {
                            medalNumber = ScoreManager.instance.getTimesRetry();
                        }

                        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/GUI/medallas");
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(XMLAchievements.text);
                        string title, descr;
                        Medal medal;
                        float size = MedalPanel.GetComponent<RectTransform>().rect.height * 0.90f;

                        for (int i = 0; i < medalNumber; i++)
                        { 
                            //Declaration
                            GameObject newMedal = Object.Instantiate<GameObject>(AchievementMedal);
                            medals.Add(newMedal.GetComponent<RectTransform>());

                            newMedal.SetActive(true);
                            newMedal.transform.SetParent(MedalPanel.transform);

                            //Connections
                            if (medals.Count > 1)
                            {
                                Button lastMedalButton = medals[medals.Count - 2].gameObject.GetComponent<Button>();
                                Button thisMedalButton = medals[medals.Count - 1].gameObject.GetComponent<Button>();

                                Navigation navigator = lastMedalButton.navigation;
                                navigator.selectOnRight = thisMedalButton;
                                lastMedalButton.navigation = navigator;

                                navigator = thisMedalButton.navigation;
                                navigator.selectOnLeft = lastMedalButton;
                                thisMedalButton.navigation = navigator;
                            }
                            
                            //Content
                            medal = newMedal.GetComponent<Medal>();
                            
                            switch (ScoreManager.instance.phase)
                            {
                                case ScoreManager.pointsCalculationPhases.PACIFIST:
                                    title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Pacifist\"]/Name").InnerText;
                                    descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Pacifist\"]/Descr").InnerText;
                                    medal.setImageAndText(sprites[1], title, descr, "x" + roundToTwo(multiplier));
                                    break;

                                case ScoreManager.pointsCalculationPhases.GENOCIDE:
                                    title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Genocide\"]/Name").InnerText;
                                    descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Genocide\"]/Descr").InnerText;
                                    medal.setImageAndText(sprites[2], title, descr, "x" + roundToTwo(multiplier));
                                    break;

                                case ScoreManager.pointsCalculationPhases.PERMADEATH:
                                    title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Permadeath\"]/Name").InnerText;
                                    descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Permadeath\"]/Descr").InnerText;
                                    medal.setImageAndText(sprites[3], title, descr, "x" + roundToTwo(multiplier));
                                    break;

                                case ScoreManager.pointsCalculationPhases.GODMODE:
                                    title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"GodMode\"]/Name").InnerText;
                                    descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"GodMode\"]/Descr").InnerText;
                                    medal.setImageAndText(sprites[4], title, descr, "x" + roundToTwo(multiplier));
                                    break;

                                case ScoreManager.pointsCalculationPhases.COMBO:
                                    title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Combo\"]/Name").InnerText;
                                    descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Combo\"]/Descr").InnerText;
                                    medal.setImageAndText(sprites[5], title, descr, "x" + roundToTwo(multiplier));
                                    break;

                                case ScoreManager.pointsCalculationPhases.JINXED:
                                    title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Jinxed\"]/Name").InnerText;
                                    descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Jinxed\"]/Descr").InnerText;
                                    medal.setImageAndText(sprites[6], title, descr, "x" + roundToTwo(multiplier));
                                    break;

                                case ScoreManager.pointsCalculationPhases.LAG:
                                    title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Lag\"]/Name").InnerText;
                                    descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Lag\"]/Descr").InnerText;
                                    medal.setImageAndText(sprites[7], title, descr, "x" + roundToTwo(multiplier));
                                    break;

                                case ScoreManager.pointsCalculationPhases.PENALTY:
                                    title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Penalty\"]/Name").InnerText;
                                    descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Penalty\"]/Descr").InnerText;
                                    medal.setImageAndText(sprites[16], title, descr, "<color=#FF0000FF>/2</color>");
                                    break;
                            }

                            if (ScoreManager.instance.phase == ScoreManager.pointsCalculationPhases.PENALTY)
                                SoundManager.instance.PlaySingle(penaltySound);
                            else
                                SoundManager.instance.PlaySingle(medalSound);

                            //Size
                            medals[medals.Count - 1].sizeDelta = new Vector2(size, size);
                        }

                        for (int i = 0; i < medals.Count; i++)
                        {

                            float posX = size * (i + 0.5f + (medals.Count * -0.5f));    //Precalculated function

                            if (i < medals.Count - 1)
                                medals[i].anchoredPosition = new Vector2(posX, 0);
                            else
                                medals[i].anchoredPosition = new Vector2(posX, -20);
                        }
                    }

                    //If there's no more medals to calculate
                    else if (ScoreManager.instance.phase == ScoreManager.pointsCalculationPhases.PENALTY)
                    {
                        if (medals.Count > 0)
                        {
                            Button firstMedal = medals[0].gameObject.GetComponent<Button>();

                            Navigation navigator = continueButton.navigation;
                            navigator.selectOnUp = firstMedal;
                            navigator.selectOnDown = firstMedal;
                            continueButton.navigation = navigator;

                            navigator = retryButton.navigation;
                            navigator.selectOnUp = firstMedal;
                            navigator.selectOnDown = firstMedal;
                            retryButton.navigation = navigator;

                            navigator = menuButton.navigation;
                            navigator.selectOnUp = firstMedal;
                            navigator.selectOnDown = firstMedal;
                            menuButton.navigation = navigator;

                            //Connect the last and first buttons

                            Button lastMedal = medals[medals.Count - 1].gameObject.GetComponent<Button>();

                            navigator = firstMedal.navigation;
                            navigator.selectOnLeft = lastMedal;
                            firstMedal.navigation = navigator;

                            navigator = lastMedal.navigation;
                            navigator.selectOnRight = firstMedal;
                            lastMedal.navigation = navigator;
                        }
                    }

                    
                }

                break;

            case scoreState.MEDAL_UP:

                Vector2 pos = medals[medals.Count - 1].anchoredPosition;
                pos.y += 700f * Time.deltaTime;

                if (pos.y >= 40)
                {
                    pos.y = 40;
                    timeOut(0.0f, scoreState.MEDAL_DOWN);
                }

                medals[medals.Count - 1].anchoredPosition = pos;

                break;

            case scoreState.MEDAL_DOWN:

                Vector2 nPos = medals[medals.Count - 1].anchoredPosition;
                nPos.y -= 700f * Time.deltaTime;

                if (nPos.y <= 0)
                {
                    nPos.y = 0;
                    timeOut(0.75f, scoreState.ITEMS_REMOVED);
                }

                medals[medals.Count - 1].anchoredPosition = nPos;

                break;

            case scoreState.MEDALS_SHOW:
                if (timeOut(0.7f, scoreState.SCORE_SHOWING))
                {
                    ScoreManager.instance.calculatePoints();

                    //If it's a new record
                    if (ScoreManager.instance.CheckHiscore(Loader.getLastLevel(), goalPoints))
                    {
                        newHiscore.transform.parent.gameObject.SetActive(true);
                        nameInput.Select();
                        state = scoreState.NEW_RECORD;
                        SoundManager.instance.PlaySingle(newRecordSound);
                    }
                    else
                    {
                        HighlightMenu();
                    }

                    if (skip)
                    {
                        actualPoints = goalPoints;
                        finalPoints.text = string.Format("{0:#,###0.#}", Mathf.Round(actualPoints));
                    }

                    skip = false;
                }
                break;

            //case scoreState.SCORE_SHOWING:

            //    break;

            case scoreState.GLITCH_WALKING:
                Vector3 position = characterRT.position;
                position.x += 375f * Time.deltaTime;
                characterRT.position = position;

                if (position.x > Screen.width)
                {
                    state = scoreState.LOADING_LEVEL;
                    NextLevel();
                }
                break;

            //case scoreState.LOADING_LEVEL:
            //    break;
        }

        } while (skip);

	}

    private void playCountingSound()
    {
        if (countingSource != null)
        {
            if (!countingSource.isPlaying || countingSource.time > countingSource.clip.length * 0.5)
            {
                countingSource.Stop();
                countingSource.Play();
            }
        }
        else
        {
            countingSource = SoundManager.instance.PlaySingle(countingSound);
        }
    }

    private bool timeOut(float time, scoreState newState){
        if (skip || Time.time > timeLastState + time)
        {
            timeLastState = Time.time;
            state = newState;
            return true;
        }
        return false;
    }

    private void HighlightMenu()
    {
        continueButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
        continueButton.Select();
    }

    public void EndHighScoreNameInsertion()
    {
        if (nameInput.text != "")
        {
            ScoreManager.HiscoreEntry entry = new ScoreManager.HiscoreEntry();
            entry.name = nameInput.text;
            entry.points = goalPoints;
            ScoreManager.instance.NewHiscore(Loader.getLastLevel(), entry);
            SoundManager.instance.PlaySingle(acceptSound);
        }
        else
            SoundManager.instance.PlaySingle(refuseSound);

        newHiscore.transform.parent.gameObject.SetActive(false);
        HighlightMenu();
        state = scoreState.SCORE_SHOWING;
    }

    public void NextLevel()
    {
        if (Loader.getLastLevel() == "Level1")
            Loader.LoadScene("BossStage", true);
    }

    public void ContinuePress()
    {
        ScoreUI.SetActive(false);
        state = scoreState.GLITCH_WALKING;
    }

    public void RetryPress()
    {
        if (Loader.getLastLevel() == "None")
        {
            Loader.LoadScene("Level1", true);
        }
        else
        {
            Loader.LoadScene(Loader.getLastLevel(), true);
        }
    }

    public void MenuPress()
    {
        ScoreUI.SetActive(false);
        Loader.LoadScene("menu", false, false, true, true);
    }
    private float roundToTwo(float num)
    {
        return Mathf.Floor(num * 100) / 100;
    }
}
