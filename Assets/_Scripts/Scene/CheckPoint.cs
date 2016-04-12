using UnityEngine;

public class CheckPoint : MonoBehaviour {

	public ParticleSystem particles;
	public bool active = false;
	private bool growing = false;

	void Start(){
		particles = gameObject.transform.FindChild ("Particles").gameObject.GetComponent<ParticleSystem>();
	}

	void OnTriggerEnter(Collider coll){
		if((coll.gameObject.name == "Player") && !active)
        {
			setThisAsCheckPoint(coll.gameObject);
			growing = true;
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

	void Update(){
		if (growing == true) {

			var so = new UnityEditor.SerializedObject(particles);

			so.FindProperty("ShapeModule.radius").floatValue += 0.4f * Time.deltaTime;
			//so.FindProperty("EmissionModule.rate").floatValue += 9.0f * Time.deltaTime;
			//so.FindProperty ("startSpeed").floatValue += 0.5f * Time.deltaTime;
			so.ApplyModifiedProperties();
		}
	}
}
