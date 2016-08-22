using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
        SCORE_SHOWING,
        GLITCH_WALKING,
        LOADING_LEVEL
    };

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

    //BUTTON REFERENCES
    public Button continueButton;

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
                    else actualValue += 4f;

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
                    else actualValue += 0.04f;

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
                timeOut(1.0f, scoreState.HEARTS_REMOVED);
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
