using UnityEngine;

public class CheckPoint : MonoBehaviour {

	public bool active = false;
    public Renderer renderer;
    public Player player;

	void OnTriggerEnter(Collider coll){
		if((coll.gameObject.name == "Player") && !active)
        {
			setThisAsCheckPoint(player);
		}
	}

	protected void setThisAsCheckPoint(Player player)
    {
		Player script = player.GetComponent<Player> ();
		script.lastCheckPoint = this;
		script.healCompletely();

        // Changes color of checkpoint
		renderer.material.color = new Color (1, 0, 0, renderer.material.color.a);
		active = true;
	}
}
