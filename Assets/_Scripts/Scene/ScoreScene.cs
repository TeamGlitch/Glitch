using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreScene : MonoBehaviour {

    public enum scoreState
    {
        SCORE_SHOWING,
        GLITCH_WALKING,
        LOADING_LEVEL
    };

    public Button continueButton;
    public Text text;

    private scoreState state;
    private RectTransform characterRT;
    private GameObject ScoreUI;

	// Use this for initialization
	void Start () {

        ScoreUI = transform.FindChild("Score").gameObject;

        GameObject glitch = transform.FindChild("Character").gameObject;
        characterRT = glitch.GetComponent<RectTransform>();

        state = scoreState.SCORE_SHOWING;
        continueButton.Select();

        text.text = ScoreManager.instance.calculatePoints();

	}
	
	// Update is called once per frame
	void Update () {

        switch(state)
        {
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
}
