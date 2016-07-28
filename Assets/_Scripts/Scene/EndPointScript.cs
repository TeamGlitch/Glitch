using UnityEngine;
using UnityEngine.UI;

//Everything is temporal!
public class EndPointScript : MonoBehaviour {

    public GameObject titlesGameObject;
    public Text title;
    public Text subtitle;

	private float endGame = -1;

    // Script is disabled until OnTriggerEnter detects the player
	void OnTriggerEnter(Collider coll){

		if(coll.gameObject.CompareTag("Player")){
            titlesGameObject.SetActive(true);
            title.text = "Level 1... complete?";
			title.color = Color.red;
			subtitle.text = "Time for a boss";
			coll.transform.gameObject.GetComponent<PlayerController>().allowMovement = false;
			endGame = Time.time + 3.0f;
            enabled = true;
		}
	}

	void Update(){

		if (endGame != -1 && Time.time >= endGame) {
            Loader.LoadScene("Congratulations", true, false, true, true);
		}

	}

}
