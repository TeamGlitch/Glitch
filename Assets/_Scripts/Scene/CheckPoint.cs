using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    public bool active = false;
    public AudioClip checkpointSound;
    private ParticleSystem particles;

    //Animation
    private float growStart = -1;
    private float growEnd;

    private GameObject InnerPortal = null;
    private GameObject MedPortal = null;
    private GameObject OuterPortal = null;
    private Renderer cylinderRenderer = null;
    private float speed = 9.0f;

    public bool startPoint = false;

    void Start()
    {
        //Find the particle system
        particles = gameObject.transform.FindChild("Particles").gameObject.GetComponent<ParticleSystem>();
        InnerPortal = gameObject.transform.FindChild("InnerPortal").gameObject;
        MedPortal = gameObject.transform.FindChild("MedPortal").gameObject;
        OuterPortal = gameObject.transform.FindChild("OuterPortal").gameObject;
        cylinderRenderer = gameObject.transform.FindChild("Cylinder").gameObject.GetComponent<Renderer>();
        cylinderRenderer.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider coll)
    {

        //If there's a collision with the player
        if ((coll.gameObject.CompareTag("Player")) && !active)
        {
            //Sets this point as the checkpoint
            setThisAsCheckPoint(coll.transform.gameObject, false);

            //Activates the grow animation
            growStart = Time.time;
            growEnd = growStart + 1.7f;
            cylinderRenderer.gameObject.SetActive(true);
            SoundManager.instance.PlaySingle(checkpointSound);
        }

    }

    //Sets this as a checkpoint
    protected void setThisAsCheckPoint(GameObject playerRef, bool start)
    {
        Player player = playerRef.GetComponent<Player>();
        player.lastCheckPoint = this;
        if (!start)
        {
            player.healCompletely();
        }

        // Changes color of checkpoint
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = new Color(1, 0, 0, renderer.material.color.a);

        active = true;
    }

    void Update()
    {

        if (InnerPortal != null)
        {
            InnerPortal.transform.Rotate(new Vector3(0, 0, speed * Time.deltaTime));
            MedPortal.transform.Rotate(new Vector3(0, 0, -speed * 1.33f * Time.deltaTime));
            OuterPortal.transform.Rotate(new Vector3(0, 0, 16.0f * 1.77f * Time.deltaTime));
        }

        //If there's a growing animation
        if (growStart > -1)
        {

            //Get the actual time
            float actualTime = Time.time;

            //If it's larger than growEnd, it will be growEnd
            if (actualTime >= growEnd)
            {
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
            InnerPortal.transform.localScale = new Vector3(scale, scale, 0);

            scale = Mathf.Lerp(0.3f, 0.85f, animationFrame);
            MedPortal.transform.localScale = new Vector3(scale, scale, 0);

            scale = Mathf.Lerp(0.3f, 0.7f, animationFrame);
            OuterPortal.transform.localScale = new Vector3(scale, scale, 0);

            speed = Mathf.Lerp(9.0f, 36.0f, animationFrame);


            ///////CILINDER///////

            ///ANIMACION 01///

            /*
            float cilinderFrame = Mathf.Pow(animationFrame, 0.28f);
            float cilinderHeight;
            float cilinderWidth;
            Vector2 actualTiling = cylinderRenderer.material.mainTextureScale;
            Vector2 actualOffset = cylinderRenderer.material.GetTextureOffset("_MainTex");

            if (cilinderFrame < 0.5) {

                cilinderFrame = cilinderFrame * 2;

                cilinderHeight = Mathf.Lerp(0.25f,2.0f,cilinderFrame);
                cilinderWidth = Mathf.Lerp (0.25f,1.0f, cilinderFrame);

                actualTiling.x = Mathf.Lerp(3f, 5f,cilinderFrame);
                actualTiling.y = Mathf.Lerp(2f, 5f,cilinderFrame);

                actualOffset.x += Mathf.Lerp (0.0f, 0.45f, cilinderFrame);

            } else {

                cilinderFrame = (cilinderFrame * 2) - 1;

                cilinderHeight = Mathf.SmoothStep(2.0f, 0.02f,cilinderFrame);
                cilinderWidth = Mathf.Lerp(1.0f, 6.0f, cilinderFrame);

                actualTiling.x = Mathf.Lerp(5f, 40f,cilinderFrame);
                actualTiling.y = Mathf.Lerp(5f, 1f,cilinderFrame);

                actualOffset.x += Mathf.Lerp (0.45f, 0.045f, cilinderFrame);

                Color color = cylinderRenderer.material.color;
                color.a = Mathf.Lerp(1.0f, 0.6f,cilinderFrame);
                cylinderRenderer.material.color = color;
			
            }
				
            cylinderRenderer.gameObject.transform.localScale = new Vector3(cilinderWidth, cilinderHeight, cilinderWidth);

            cylinderRenderer.material.mainTextureScale = actualTiling;
            cylinderRenderer.material.SetTextureOffset("_MainTex", actualOffset);

            Vector3 cilinderPosition = cylinderRenderer.gameObject.transform.localPosition;
            cilinderPosition.y = cilinderHeight - 1;
            cylinderRenderer.gameObject.transform.localPosition = cilinderPosition;


            */

            ///ANIMACION 02///


            float cilinderFrame = Mathf.Pow(animationFrame, 0.28f);
            float cilinderHeight;
            float cilinderWidth;
            Vector2 actualTiling = cylinderRenderer.material.mainTextureScale;
            Vector2 actualOffset = cylinderRenderer.material.GetTextureOffset("_MainTex");
            Color color = cylinderRenderer.material.color;

            if (cilinderFrame < 0.5)
            {

                cilinderFrame = cilinderFrame * 2;

                cilinderHeight = Mathf.Lerp(0.20f, 0.25f, cilinderFrame);
                cilinderWidth = Mathf.Lerp(0.20f, 1.5f, cilinderFrame);

                actualTiling.x = Mathf.Lerp(3.0f, 12f, cilinderFrame);
                actualTiling.y = Mathf.Lerp(3.0f, 2f, cilinderFrame);

                actualOffset.x += Mathf.Lerp(0.0f, 0.45f, cilinderFrame);

                color.a = Mathf.Lerp(0.0f, 1.0f, cilinderFrame);


            }
            else
            {

                cilinderFrame = (cilinderFrame * 2) - 1;

                cilinderHeight = Mathf.SmoothStep(0.25f, 10.0f, cilinderFrame);
                cilinderWidth = Mathf.Lerp(1.5f, 0.2f, cilinderFrame);

                actualTiling.x = Mathf.Lerp(12f, 2f, cilinderFrame);
                actualTiling.y = Mathf.Lerp(2f, 10f, cilinderFrame);

                actualOffset.x += Mathf.Lerp(0.45f, 0.045f, cilinderFrame);

                color.a = Mathf.SmoothStep(1.0f, 0.0f, cilinderFrame);

            }

            cylinderRenderer.gameObject.transform.localScale = new Vector3(cilinderWidth * 0.1f, cilinderHeight * 0.1f, cilinderWidth * 0.1f);

            cylinderRenderer.material.mainTextureScale = actualTiling;
            cylinderRenderer.material.SetTextureOffset("_MainTex", actualOffset);
            cylinderRenderer.material.color = color;

            //After all the calculations, do growStart = -1 to stop doing the animation
            if (actualTime >= growEnd)
            {
                growStart = -1;
                cylinderRenderer.gameObject.SetActive(false);
            }
        }
    }
}

