using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Everything is temporal!
public class EndPointScript : MonoBehaviour {
    public GameObject titlesGameObject;
    public Text title;
    public Text subtitle;
    public PlayerController player;

	private float endGame = -1;

    // Script is disabled until OnTriggerEnter detects the player
	void OnTriggerEnter(Collider coll){
		if(coll.gameObject.name == "Player"){
            titlesGameObject.SetActive(true);
			title.text = "Level Complete!";
			title.color = Color.red;
			subtitle.text = "Good work not screwing up";
			player.state = PlayerController.player_state.DEATH;
			endGame = Time.time + 3.0f;
            enabled = true;
		}
	}

	void Update(){
		if (Time.time >= endGame) {
			SceneManager.LoadScene("menu");
		}
	}
}
