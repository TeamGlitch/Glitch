using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {

	public bool active = false;

	void OnTriggerEnter(Collider coll){

		if(coll.gameObject.name == "Player" && !active){
			setThisAsCheckPoint(coll.gameObject);
		}
	}

	protected void setThisAsCheckPoint(GameObject player){
		Player script = player.GetComponent<Player> ();
		script.lastCheckPoint = this;
		script.healCompletely();

		Renderer renderer = GetComponent<Renderer>();
		renderer.material.color = new Color (1, 0, 0, renderer.material.color.a);

		active = true;
	}
}
