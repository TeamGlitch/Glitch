using UnityEngine;
using System.Collections;

public class ScoreScene : MonoBehaviour {

    private bool loadingScene = false;
    private RectTransform characterRT;

	// Use this for initialization
	void Start () {
        GameObject glitch = transform.FindChild("Character").gameObject;
        characterRT = glitch.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {

        if (!loadingScene)
        {
            Vector3 position = characterRT.position;
            position.x += 2.5f;
            characterRT.position = position;

            if (position.x > Screen.width)
            {
                loadingScene = true;
                ReturnToMenu();
            }
        }

	}

    public void ReturnToMenu()
    {
        if (Loader.getLastScene() == "Level1")
            Loader.LoadScene("Boss Stage", false);
    }
}
