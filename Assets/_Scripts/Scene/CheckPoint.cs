using UnityEngine;

public class CheckPoint : MonoBehaviour {

	public ParticleSystem particles;
	public bool active = false;

	//Animation
	private float growStart = -1;
	private float growEnd;

	void Start(){
		//Find the particle system
		particles = gameObject.transform.FindChild ("Particles").gameObject.GetComponent<ParticleSystem>();
	}

	void OnTriggerEnter(Collider coll){
		
		//If there's a collision with the player
		if((coll.gameObject.name == "Player") && !active)
        {
			//Sets this point as the checkpoint
			setThisAsCheckPoint(coll.gameObject);

			//Activates the grow animation
			growStart = Time.time;
			growEnd = growStart + 2.5f;
		}

	}

	//Sets this as a checkpoint
	protected void setThisAsCheckPoint(GameObject playerRef)
    {
		Player player = playerRef.GetComponent<Player>();
		player.lastCheckPoint = this;
		player.healCompletely();

        // Changes color of checkpoint
		Renderer renderer = GetComponent<Renderer>();
		renderer.material.color = new Color (1, 0, 0, renderer.material.color.a);

		active = true;
	}
		
	void Update(){

		//If there's a growing animation
		if (growStart > -1) {

			//Get the actual time
			float actualTime = Time.time;

			//If it's larger than growEnd, it will be growEnd
			if (actualTime >= growEnd) {
				actualTime = growEnd;
			}

			//Variables for the function calculation
			float negElapsed = growStart - actualTime;
			float remaining = growEnd - actualTime;
			float totalTime = growEnd - growStart;

			//Increase the radius
			ParticleSystem.ShapeModule shapeModule = particles.shape;
			shapeModule.radius = -((0.5f * negElapsed) - (0.1f * remaining)) / totalTime;

			//Increase the emission rate
			ParticleSystem.EmissionModule emission = particles.emission;
			ParticleSystem.MinMaxCurve rate = emission.rate;
			rate.constantMax = -((10.0f * negElapsed) - (1.0f * remaining)) / totalTime;
			emission.rate = rate;

			//Increase the startSpeed
			particles.startSpeed = -((1.0f * negElapsed) - (0.5f * remaining)) / totalTime;

			//After all the calculations, do growStart = -1 to stop doing the animation
			if (actualTime >= growEnd) {
				growStart = -1;
			}
		}
	}
}
