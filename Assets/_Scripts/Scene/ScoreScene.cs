using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml;

public class ScoreScene : MonoBehaviour {

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
        SCORE_SHOWING,
        GLITCH_WALKING,
        LOADING_LEVEL
    };

    public TextAsset XMLInterface;
    public TextAsset XMLAchievements;

    //UI REFERENCES
    public Text finalPoints;

    public Text pointsMultiplier;
    public Text pointsValue;

    public Text timeMultiplier;
    public Text timeValue;

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

	// Use this for initialization
	void Start () {

        ScoreUI = transform.FindChild("Score").gameObject;

        GameObject glitch = transform.FindChild("Character").gameObject;
        characterRT = glitch.GetComponent<RectTransform>();

        //Sets
        finalPoints.text = "0";

        pointsMultiplier.text = "";
        pointsValue.text = "";

        timeMultiplier.text = "";
        timeValue.text = "";

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
        continueButton.Select();


        timeLastState = Time.time;

	}
	
	// Update is called once per frame
	void Update () {

        switch(state)
        {
            case scoreState.START:
                if(timeOut(0.5f, scoreState.POINTS_SHOW_MULTIPLIER_1)){
                    pointsMultiplier.text = ScoreManager.instance.getBasePoints().ToString();
                }
                break;

            case scoreState.POINTS_SHOW_MULTIPLIER_1:
                if (timeOut(0.5f, scoreState.POINTS_SHOW_MULTIPLIER_2))
                {
                    pointsMultiplier.text = ScoreManager.instance.getBasePoints().ToString() + " <color=#FFC300FF>x " + ScoreManager.instance.calculatePoints() + "</color>";
                }
                break;

            case scoreState.POINTS_SHOW_MULTIPLIER_2:
                if (timeOut(0.5f, scoreState.POINTS_INCREASING))
                {
                    pointsValue.text = "0";
                    desiredValue = ScoreManager.instance.getPoints();
                    actualValue = 0;
                }
                break;

            case scoreState.POINTS_INCREASING:

                if (actualValue < desiredValue)
                {

                    if (actualValue == 0) actualValue = 1;
                    else actualValue += 400f * Time.deltaTime;

                    if (actualValue > desiredValue)
                    {
                        actualValue = desiredValue;
                    }

                    pointsValue.text = (int)actualValue + "";
                    finalPoints.text = pointsValue.text;

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
                }
                break;

            case scoreState.TIME_SHOW_MULTIPLIER_1:
                if (timeOut(0.5f, scoreState.TIME_SHOW_MULTIPLIER_2))
                {
                    timeMultiplier.text = (ScoreManager.instance.getTotalTime() - ScoreManager.instance.getTimeSpent()).ToString() + timeMultiplier.text;
                }
                break;

            case scoreState.TIME_SHOW_MULTIPLIER_2:
                if (timeOut(0.5f, scoreState.TIME_INCREASING))
                {
                    timeValue.text = "x 0";
                    desiredValue = ScoreManager.instance.calculatePoints();
                    actualValue = 0;
                }
                break;

            case scoreState.TIME_INCREASING:

                if (actualValue < desiredValue)
                {

                    if (actualValue == 0) actualValue = 1;
                    else actualValue += 1.5f * Time.deltaTime;

                    if (actualValue > desiredValue)
                    {
                        actualValue = desiredValue;
                    }

                    timeValue.text = "x " + roundToTwo(actualValue);
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

                    if (ScoreManager.instance.getRemainingLives() == 3)
                        thirdHeart.sprite = emptyHeart;
                    else if (ScoreManager.instance.getRemainingLives() == 2)
                        secondHeart.sprite = thirdHeart.sprite;
                    else
                        firstHeart.sprite = thirdHeart.sprite;

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
                    }
                    else if (ScoreManager.instance.getRemainingLives() == 2)
                    {
                        firstHeart.sprite = thirdHeart.sprite;
                        heartText.text = "x 1.25";
                        heartText.fontSize = 27;
                        state = scoreState.HEARTS_REMOVE_2;
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

                    if (ScoreManager.instance.getColectionablesTaken() > 0)
                    {
                        itemText.text = "x 1.5";
                        itemText.fontSize = 20;

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

                    while (multiplier == 0 && ScoreManager.instance.phase != ScoreManager.pointsCalculationPhases.LAG)
                    {
                        multiplier = ScoreManager.instance.calculatePoints();
                    }

                    //If we won a medal
                    if (multiplier != 0)
                    {
                        state = scoreState.MEDAL_UP;

                        GameObject newMedal = Object.Instantiate<GameObject>(AchievementMedal);
                        medals.Add(newMedal.GetComponent<RectTransform>());

                        newMedal.SetActive(true);
                        newMedal.transform.SetParent(MedalPanel.transform);

                        //Set the last button to this && viceversa
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

                        float size = MedalPanel.GetComponent<RectTransform>().rect.height * 0.90f;
                        medals[medals.Count - 1].sizeDelta = new Vector2(size, size);

                        for (int i = 0; i < medals.Count; i++)
                        {

                            float posX = size * (i + 0.5f + (medals.Count * -0.5f));    //Precalculated function

                            if (i < medals.Count - 1)
                                medals[i].anchoredPosition = new Vector2(posX, 0);
                            else
                                medals[i].anchoredPosition = new Vector2(posX, -20);
                        }

                        Medal medal = newMedal.GetComponent<Medal>();
                        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/GUI/medallas");
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(XMLAchievements.text);

                        string title, descr;

                        switch (ScoreManager.instance.phase)
                        {
                            case ScoreManager.pointsCalculationPhases.PACIFIST:
                                title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Pacifist\"]/Name").InnerText;
                                descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Pacifist\"]/Descr").InnerText;
                                medal.setImageAndText(sprites[1], title, descr, roundToTwo(multiplier));
                                break;

                            case ScoreManager.pointsCalculationPhases.GENOCIDE:
                                title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Genocide\"]/Name").InnerText;
                                descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Genocide\"]/Descr").InnerText;
                                medal.setImageAndText(sprites[2], title, descr, roundToTwo(multiplier));
                                break;

                            case ScoreManager.pointsCalculationPhases.PERMADEATH:
                                title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Permadeath\"]/Name").InnerText;
                                descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Permadeath\"]/Descr").InnerText;
                                medal.setImageAndText(sprites[3], title, descr, roundToTwo(multiplier));
                                break;

                            case ScoreManager.pointsCalculationPhases.GODMODE:
                                title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"GodMode\"]/Name").InnerText;
                                descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"GodMode\"]/Descr").InnerText;
                                medal.setImageAndText(sprites[4], title, descr, roundToTwo(multiplier));
                                break;

                            case ScoreManager.pointsCalculationPhases.COMBO:
                                title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Combo\"]/Name").InnerText;
                                descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Combo\"]/Descr").InnerText;
                                medal.setImageAndText(sprites[5], title, descr, roundToTwo(multiplier));
                                break;

                            case ScoreManager.pointsCalculationPhases.JINXED:
                                title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Jinxed\"]/Name").InnerText;
                                descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Jinxed\"]/Descr").InnerText;
                                medal.setImageAndText(sprites[6], title, descr, roundToTwo(multiplier));
                                break;

                            case ScoreManager.pointsCalculationPhases.LAG:
                                title = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Lag\"]/Name").InnerText;
                                descr = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Achievement[@id = \"Lag\"]/Descr").InnerText;
                                medal.setImageAndText(sprites[7], title, descr, roundToTwo(multiplier));
                                break;
                        }
                    }

                    //If there's no more medals to calculate
                    else if (ScoreManager.instance.phase == ScoreManager.pointsCalculationPhases.LAG)
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
                    
                }
                break;

            //case scoreState.SCORE_SHOWING:

            //    break;

            case scoreState.GLITCH_WALKING:
                Vector3 position = characterRT.position;
                position.x += 2.5f;
                characterRT.position = position;

                if (position.x > Screen.width)
                {
                    state = scoreState.LOADING_LEVEL;
                    ReturnToMenu();
                }
                break;

            //case scoreState.LOADING_LEVEL:
            //    break;
        }

	}

    private bool timeOut(float time, scoreState newState){
        if (Time.time > timeLastState + time)
        {
            timeLastState = Time.time;
            state = newState;
            return true;
        }
        return false;
    }

    public void ReturnToMenu()
    {
        if (Loader.getLastScene() == "Level1")
            Loader.LoadScene("Boss Stage", true);
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
