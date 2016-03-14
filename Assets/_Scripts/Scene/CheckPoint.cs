using UnityEngine;

public class CheckPoint : MonoBehaviour {

	public bool active = false;

	void OnTriggerEnter(Collider coll){
		if((coll.gameObject.name == "Player") && !active)
        {
			setThisAsCheckPoint(coll.gameObject);
		}
	}

	protected void setThisAsCheckPoint(GameObject playerRef)
    {
		Player player = playerRef.GetComponent<Player> ();
		player.lastCheckPoint = this;
		player.healCompletely();

        // Changes color of checkpoint
		Renderer renderer = GetComponent<Renderer>();
		renderer.material.color = new Color (1, 0, 0, renderer.material.color.a);

		active = true;
	}
}
