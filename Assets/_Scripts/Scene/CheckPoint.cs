using UnityEngine;

public class CheckPoint : MonoBehaviour {

	public ParticleSystem particles;
	public bool active = false;

	//Animation
	private float growStart = -1;
	private float growEnd;

	private GameObject InnerPortal = null; 
	private GameObject MedPortal = null; 
	private GameObject OuterPortal = null;
	public Renderer cylinderRenderer = null;
	private float speed = 9.0f;

	void Start(){
		//Find the particle system
		particles = gameObject.transform.FindChild("Particles").gameObject.GetComponent<ParticleSystem>();
		InnerPortal = gameObject.transform.FindChild("InnerPortal").gameObject;
		MedPortal = gameObject.transform.FindChild("MedPortal").gameObject;
		OuterPortal = gameObject.transform.FindChild("OuterPortal").gameObject;
		cylinderRenderer = gameObject.transform.FindChild ("Cylinder").gameObject.GetComponent<Renderer>();
		//cylinderRenderer.gameObject.SetActive(false);
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

		if (InnerPortal != null){
			InnerPortal.transform.Rotate(new Vector3(0,0,speed * Time.deltaTime));
			MedPortal.transform.Rotate(new Vector3(0,0,-speed * 1.33f * Time.deltaTime));
			OuterPortal.transform.Rotate(new Vector3(0,0,16.0f * 1.77f * Time.deltaTime));
		}

		//If there's a growing animation
		if (growStart > -1) {

			//Get the actual time
			float actualTime = Time.time;

			//If it's larger than growEnd, it will be growEnd
			if (actualTime >= growEnd) {
				actualTime = growEnd;
			}

			float animationFrame = (actualTime - growStart) / (growEnd - growStart);

			//Increase the radius
			ParticleSystem.ShapeModule shapeModule = particles.shape;
			shapeModule.radius = Mathf.Lerp(0.1f, 0.5f, animationFrame);

			//Increase the emission rate
			ParticleSystem.EmissionModule emission = particles.emission;
			ParticleSystem.MinMaxCurve rate = emission.rate;
			rate.constantMax = Mathf.Lerp(1.0f, 10.0f, animationFrame);
			emission.rate = rate;

			//Increase the startSpeed
			particles.startSpeed = Mathf.Lerp(0.5f, 1.0f, animationFrame);

			//Increases portal size & speed
			float scale = Mathf.Lerp(0.3f, 1.0f, animationFrame);
			InnerPortal.transform.localScale = new Vector3 (scale, scale, 0);

			scale = Mathf.Lerp(0.3f, 0.85f, animationFrame);
			MedPortal.transform.localScale = new Vector3 (scale, scale, 0);

			scale = Mathf.Lerp(0.3f, 0.7f, animationFrame);
			OuterPortal.transform.localScale = new Vector3 (scale, scale, 0);

			speed = Mathf.Lerp (9.0f, 36.0f, animationFrame);
		

			var cilinderFrame = Mathf.Pow (animationFrame, 0.28f);

			//Vector2 actualOffset = cylinderRenderer.material.GetTextureOffset("_MainTex");
			//actualOffset.x += Mathf.Lerp(
			//cylinderRenderer.material.SetTextureOffset("_MainTex", actualOffset);

			float cilinderScale = Mathf.Lerp(0.25f,1.0f,cilinderFrame);
			cylinderRenderer.gameObject.transform.localScale = new Vector3 (cilinderScale, cilinderScale * 2, cilinderScale);
			Vector3 cilinderPosition = cylinderRenderer.gameObject.transform.localPosition;
			cilinderPosition.y = (cilinderScale*2) - 1;
			cylinderRenderer.gameObject.transform.localPosition = cilinderPosition;

			//After all the calculations, do growStart = -1 to stop doing the animation
			if (actualTime >= growEnd) {
				growStart = -1;
			}
		}
	}
}
