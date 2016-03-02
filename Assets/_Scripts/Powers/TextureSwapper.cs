using UnityEngine;
using System.Collections;
using InControl;

public class TextureSwapper : MonoBehaviour {

	public HUDTexture HUDtex;							//UI reference
	public GameObject sight;							//Sight reference

	public GameObject stealerBullet;					//Stealer bullet prefab
	public GameObject painterBullet;					//Painter bullet prefab

	private RectTransform sightRectTransform;			//Transformation of the sight

	private ObjectPool stealerBulletPool;				//Object pools
	private ObjectPool painterBulletPool;

	public Material actualTexture = null;				//Actual texture used for painting

	public float shootSpeed = 0.45f;					//Speed of the bullet
	public float radius = 2.0f; 						//Radius of the sight transformation
	public float shootCooldown = 0.1f;					//Cooldown between shoots

	private bool shootingMode = false;					//If it is in shooting mode
	private float lastShootStart = 0.0f;				//When the last shoot started

	// Use this for initialization
	void Start () {
		
		sight.SetActive(false);

		stealerBulletPool = new ObjectPool(stealerBullet);
		painterBulletPool = new ObjectPool(painterBullet);

		sightRectTransform = sight.GetComponent<RectTransform>();

	}
	
	// Update is called once per frame
	void Update () {

		//print ("Stealer - Size: " + stealerBulletPool.getActualSize () + " Max: " + stealerBulletPool.getMaxSize () + " Activated: " + stealerBulletPool.getActiveMembers () + " Inactivated: " + stealerBulletPool.getInactiveMembers ());
		//print ("Painter - Size: " + painterBulletPool.getActualSize () + " Max: " + painterBulletPool.getMaxSize () + " Activated: " + painterBulletPool.getActiveMembers () + " Inactivated: " + painterBulletPool.getInactiveMembers ()); 

		//Shooting mode activation - desactivation
		if (InputManager.ActiveDevice.LeftTrigger.WasPressed) {
			shootingMode = true;
			sight.SetActive(true);
		}
		if (InputManager.ActiveDevice.LeftTrigger.WasReleased) {
			shootingMode = false;
			sight.SetActive(false);
		}

		//If in shooter mode
		if (shootingMode) {

			//Calcule the distance to where the sight is aiming at
			Vector2 distance;

			if (InputManager.ActiveDevice.Name != "Keyboard/Mouse") {
				
				//If a controller is being used
				distance = new Vector2 (InputManager.ActiveDevice.RightStickX * 100, InputManager.ActiveDevice.RightStickY * 100);

			} else {
				
				//If a mouse is being used
				Vector3 mousePos = Input.mousePosition;
				mousePos.z = transform.position.z - Camera.main.transform.position.z;
				mousePos = Camera.main.ScreenToWorldPoint(mousePos); 
				distance = mousePos - transform.position;

			}

			//Angle calculation
			float angle = (Mathf.Atan2(distance.y,distance.x) * Mathf.Rad2Deg);
			if (angle < 0.0f) angle += 360.0f;
			angle *= Mathf.Deg2Rad;

			//Sight position
			Vector3 sightPosition = new Vector3(transform.position.x + (radius * Mathf.Cos (angle)), transform.position.y + (radius * Mathf.Sin (angle)), 0);
			sightRectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(sightPosition);

			//Checks if it can shoot
			//If the paint bullet button is pressed, there is a paint texture and the shoot cooldown is over
			//If the stealer bullet button is pressed and the shoot cooldown x 4 is over
			//shoot = -1 do not shoot, 0 shoot painter, 1 shoot stealer
			int shoot = -1;
			if (InputManager.ActiveDevice.RightTrigger.IsPressed && (actualTexture != null) && (Time.time - lastShootStart > shootCooldown)) {
				shoot = 0;
			} else if ( InputManager.ActiveDevice.RightBumper.IsPressed && (Time.time - lastShootStart > shootCooldown * 4)){
				shoot = 1;
			}

			//Shoot
			if(shoot != -1){
				
				//Calcule sinus and consinus
				float cosShoot = Mathf.Cos(angle);
				float sinShoot = Mathf.Sin(angle);

				GameObject bullet = null;

				//Stealer
				if (shoot == 1) { 						
					bullet  = stealerBulletPool.getObject();
					TextureStealerBulletScript TEScript = bullet.GetComponent<TextureStealerBulletScript>();
					TEScript.speed = new Vector3 (shootSpeed * cosShoot, shootSpeed * sinShoot, 0);
					TEScript.textureSwapper = this;
				} 
				//Painter
				else if (shoot == 0) {					
					bullet  = painterBulletPool.getObject();
					TexturePainterBulletScript TPScript = bullet.GetComponent<TexturePainterBulletScript>();
					TPScript.speed = new Vector3 (shootSpeed * cosShoot, shootSpeed * sinShoot, 0);
					TPScript.paintMaterial = actualTexture;
				}  

				//Move the bullet a bit forward and create a shoot coldown reference point
				if (bullet != null) {
					bullet.transform.position = new Vector3 (transform.position.x + (1.0f * cosShoot), transform.position.y + (1.0f * sinShoot), 0);
					lastShootStart = Time.time;
				}

			}


		}
	}
}