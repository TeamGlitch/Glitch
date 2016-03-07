using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	//External references
	public HUDLives guiLife;
	public HUDCollects guiItem;

	public CheckPoint lastCheckPoint;
	public DeadMenuScript deadScript;

	//Internal references
	private PlayerController playerController;

	//Prefabs
	public GameObject glitchPart;

	//List
	private ObjectPool glitchPartPool;

	//Properties
	public int lives;
	public int items = 0;						// Items collected

	// Use this for initialization
	void Start () {
		glitchPartPool = new ObjectPool(glitchPart);
		playerController = GetComponent<PlayerController>();
		lives = 3;
	}

	void OnTriggerEnter(Collider coll){
		
		if(coll.CompareTag("Death")){
			lives -= 1;
			guiLife.UpdateLifeUI();
			playerController.state = PlayerController.player_state.DEATH;
			playerController.vSpeed = 0;

			bool lastLife = (lives > 0) ? false : true;

			for (int i = 0; i < 100; i++) {
				
				GameObject part = glitchPartPool.getObject();
				part.transform.position = transform.position;

				if (i == 0) {
					part.GetComponent<glitchFragment> ().restart (lastCheckPoint.gameObject.transform.position, lastLife, this);
				} else {
					part.GetComponent<glitchFragment> ().restart (lastCheckPoint.gameObject.transform.position, lastLife);
				}

			}

			if(lastLife){
				deadScript.gameObject.SetActive(true);
				deadScript.PlayerDead();
			}

			GetComponent<SpriteRenderer>().enabled = false;

		}
	}

	public void Resurrected(){
		GetComponent<SpriteRenderer>().enabled = true;
		playerController.state = PlayerController.player_state.JUMPING;
		transform.position = lastCheckPoint.gameObject.transform.position;
	}

	public void healCompletely(){
		lives = 3;
		guiLife.UpdateLifeUI();
	}
}
