using UnityEngine;
using InControl;

public class SlowFPS : MonoBehaviour {

	public World world;
	public ParticleSystem glitchParticle;
    public GlitchOffsetCamera glitchOffsetCamera;
    public CameraGlitchedToBoxes cameraGlitchedToBoxes;

	public float recoveryRate = 3.0f;			// Time it takes to make a recovery bump
	public float timeBetweenUpdates = 1;		// Seconds between slow updates
	public float MAXTime = 10.0f;				// Max time the power can be active
	public float timeRemaining;					// Remaining time the power can be active
	public float slowDown = 0.5f;				//How much the world slows down on slowFPS

    private float timeInFPS = 0.0f;

	private float recoveryTime;					// Time to the next recovery bump
	public bool powerActive = false;
	private float timeLastUpdate = 0;			// When the last slow update was done

	public delegate void SlowFPSDelegate();
	public event SlowFPSDelegate SlowFPSActivated;
	public event SlowFPSDelegate SlowFPSDeactivated;

	void Start()
	{
		// Begin with 10 seconds and the cooldown time for each second are 3 seconds
		timeRemaining = MAXTime;
		recoveryTime = recoveryRate;
	}

	// In the update we control the available time of the power and it's recovery
	void Update()
	{
		if (InputManager.ActiveDevice.Action2.WasPressed)
		{
			if (powerActive == false)
			{
				powerActive = true;
                cameraGlitchedToBoxes.isFPSActivated = true;
				world.doUpdate = false;
				timeLastUpdate = Time.time;
				world.toggleSlowFPS();
				glitchParticle.Play();
                if (SlowFPSActivated != null)
                {
                    SlowFPSActivated();
                }
                timeInFPS = 0.0f;
                glitchOffsetCamera.divisions = 20;
                glitchOffsetCamera.inestability = 0.3f;
                glitchOffsetCamera.frequency = 0.5f;
                glitchOffsetCamera.enabled = true;
			}
			else
			{
                DeactivatePower();
			}
		}

		// If power is enabled we check the time left and if is minus than 0, we disable the power
		if (powerActive)
		{
            timeInFPS += Time.deltaTime;
			timeRemaining -= Time.deltaTime;

			if (timeRemaining <= 0.0f)
			{
				timeRemaining = 0;
                DeactivatePower();

			}
            else if (world.doUpdate == true)
            {
                if(timeInFPS <= 5.0f)
                {
                    float auxTime = Mathf.Min(0.5f, timeInFPS / 10.0f);
                    SoundManager.instance.ChangeMusicSpeed(1.0f - auxTime);				
                }
				timeLastUpdate = Time.time;

			}
            else if (Time.time >= timeLastUpdate + timeBetweenUpdates)
            {
                if (timeInFPS <= 5.0f)
                {
                    float auxTime = Mathf.Min(0.5f, timeInFPS / 10.0f);
                    SoundManager.instance.ChangeMusicSpeed(1.0f - auxTime);
                }
                world.requestUpdate((Time.time - timeLastUpdate) * slowDown);
			}
		}
		else
		{
			// This is to check the recovery time. 3 seconds gives us one second of power
			if (timeRemaining < MAXTime)
			{
				recoveryTime -= Time.deltaTime;

				if (recoveryTime <= 0.0f)
				{
					timeRemaining += 1.0f;

					if (timeRemaining > MAXTime)
					{
						timeRemaining = MAXTime;
					}

					recoveryTime = recoveryRate;
				}
			}
		}
	}

	public void DeactivatePower(){
		powerActive = false;
        cameraGlitchedToBoxes.isFPSActivated = false;
		world.toggleSlowFPS();
		if (SlowFPSDeactivated != null)
			SlowFPSDeactivated ();
        glitchOffsetCamera.enabled = false;
        SoundManager.instance.ChangeMusicSpeed(1.0f);
		
	}

	public void RestartCooldowns()
	{
		timeRemaining = MAXTime;
        DeactivatePower();
	}

}
