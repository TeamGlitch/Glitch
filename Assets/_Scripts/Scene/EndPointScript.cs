using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Everything is temporal!
public class EndPointScript : MonoBehaviour {

	public GameObject Titles;
	private float endGame = -1;

	void OnTriggerEnter(Collider coll){
		if(coll.gameObject.name == "Player"){
			Titles.SetActive (true);
			Text title = Titles.transform.FindChild ("Title").GetComponent<Text>();
			title.text = "Level Complete!";
			title.color = Color.red;
			Titles.transform.FindChild("Subtitle").GetComponent<Text>().text = "Good work not screwing up";
			coll.gameObject.GetComponent<PlayerController> ().state = PlayerController.player_state.DEATH;
			endGame = Time.time + 3.0f;

		}
	}

	void Update(){
		if (endGame != -1 && Time.time >= endGame) {
			SceneManager.LoadScene("menu");
		}
	}
}
