using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Congratulations : MonoBehaviour {

    private bool bossStage;
    private bool loadingScene = false;
    private RectTransform characterRT;

    void Start()
    {
        GameObject endgameImage = transform.FindChild("EndgameImage").gameObject;
        GameObject glitch = transform.FindChild("Character").gameObject;
        characterRT = glitch.GetComponent<RectTransform>();

        if (Loader.getLastScene() == "Boss Stage")
        {
            bossStage = true;
            glitch.SetActive(false);
            GetComponent<Animation>().Play();
        }
        else
        {
            bossStage = false;
            endgameImage.SetActive(false);
        }
    }

    void Update()
    {
        if (!bossStage && !loadingScene)
        {
            Vector3 position = characterRT.position;
            position.x += 2.5f;
            characterRT.position = position;

            if(position.x > Screen.width)
            {
                loadingScene = true;
                ReturnToMenu();
            }
        }
    }

    public void ReturnToMenu(){
        if (bossStage)
        {
            Loader.LoadScene("menu", false, false, true, true);
        }
        else
        {
            Loader.LoadScene("Boss Stage", false);
        }
    }
}
