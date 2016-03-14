using UnityEngine;

public class CheckPoint : MonoBehaviour {

	public bool active = false;
    public Renderer renderer;
    public Player player;

	void OnTriggerEnter(Collider coll){
		if((coll.gameObject.name == "Player") && !active)
        {
			setThisAsCheckPoint();
		}
	}

	protected void setThisAsCheckPoint()
    {
		player.lastCheckPoint = this;
		player.healCompletely();

        // Changes color of checkpoint
		renderer.material.color = new Color (1, 0, 0, renderer.material.color.a);
		active = true;
	}
}
