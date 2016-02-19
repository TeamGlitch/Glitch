using UnityEngine;
using System.Collections;

public class TextureSwapper : MonoBehaviour {

	public GameObject sight;
	public GameObject stealerBullet;
	public GameObject paintBullet;
	public float shootSpeed = 0.45f;
	public float radius = 2.0f; 
	public float shootCooldown = 0.1f;

	public Material actualTexture = null;

	private RectTransform sightRectTransform;
	private bool shootingMode = false;
	private float lastShootStart = 0.0f;

	// Use this for initialization
	void Start () {
		sight.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

		//Shooting mode activation - desactivation
		if (Input.GetButtonDown("ToggleTextureChange") || (Input.GetAxisRaw("ToggleTextureChange") > 0 && !shootingMode)) {
			shootingMode = true;
			sightRectTransform = sight.GetComponent<RectTransform>();
			sight.SetActive(true);
		}
		if (Input.GetButtonUp("ToggleTextureChange") || (Input.GetAxisRaw("ToggleTextureChange") == 0 && shootingMode)) {
			shootingMode = false;
			sight.SetActive(false);
		}

		//If in shooter mode
		if (shootingMode) {

			//Calcule the distance to where the sight is aiming at
			Vector2 distance;
			if (Input.GetAxisRaw ("ToggleTextureChange") > 0) {
				
				//If a controller is being used
				distance = new Vector2 (Input.GetAxis("Aim_Horizontal") * 100, Input.GetAxis("Aim_Vertical") * 100);

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
			if ((Input.GetButton("PowerAction_0") || Input.GetAxisRaw ("PowerAction_0") > 0) && (actualTexture != null) && (Time.time - lastShootStart > shootCooldown)) {
				shoot = 0;
			} else if ( Input.GetButton("PowerAction_1") && (Time.time - lastShootStart > shootCooldown * 4)){
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
					bullet  = (GameObject)Instantiate(stealerBullet);
					TextureStealerBulletScript TEScript = bullet.GetComponent<TextureStealerBulletScript>();
					TEScript.speed = new Vector3 (shootSpeed * cosShoot, shootSpeed * sinShoot, 0);
					TEScript.textureSwapper = this;
				} 
				//Painter
				else if (shoot == 0) {					
					bullet = (GameObject)Instantiate (paintBullet);
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