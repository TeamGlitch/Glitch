using UnityEngine;
using System.Collections;

public class TextureSwapper : MonoBehaviour {

	public GameObject sight;
	public GameObject stealerBullet;
	public GameObject paintBullet;
	public float shootSpeed = 0.45f;
	public float radius = 2.0f; 

	public Material actualTexture = null;

	private RectTransform sightRectTransform;
	private bool shootingMode = false;

	// Use this for initialization
	void Start () {
		sight.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Q)) {
			shootingMode = true;
			sightRectTransform = sight.GetComponent<RectTransform>();
			sight.SetActive(true);
		}
		if (Input.GetKeyUp(KeyCode.Q)) {
			shootingMode = false;
			sight.SetActive(false);
		}

		if (shootingMode) {

			//Mouse position
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = transform.position.z - Camera.main.transform.position.z;
			mousePos = Camera.main.ScreenToWorldPoint(mousePos); 

			//Angle calculation
			Vector2 distance = mousePos - transform.position;
			float angle = (Mathf.Atan2(distance.y,distance.x) * Mathf.Rad2Deg);
			if (angle < 0.0f) angle += 360.0f;
			angle *= Mathf.Deg2Rad;

			//Sight position
			Vector3 sightPosition = new Vector3(transform.position.x + (radius * Mathf.Cos (angle)), transform.position.y + (radius * Mathf.Sin (angle)), 0);
			sightRectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(sightPosition);

			//Disparo stealer
			if((Input.GetMouseButtonUp (0)  && actualTexture != null) || Input.GetMouseButtonUp (1)){
				float cosShoot = Mathf.Cos(angle);
				float sinShoot = Mathf.Sin(angle);

				GameObject bullet = null;

				//Disparo stealer
				if (Input.GetMouseButtonUp (1)) { 						
					bullet  = (GameObject)Instantiate(stealerBullet);
					TextureStealerBulletScript TEScript = bullet.GetComponent<TextureStealerBulletScript>();
					TEScript.speed = new Vector3 (shootSpeed * cosShoot, shootSpeed * sinShoot, 0);
					TEScript.textureSwapper = this;
				} 
				//Disparo paint
				else if (Input.GetMouseButtonUp (0)) {					
					bullet = (GameObject)Instantiate (paintBullet);
					TexturePainterBulletScript TPScript = bullet.GetComponent<TexturePainterBulletScript>();
					TPScript.speed = new Vector3 (shootSpeed * cosShoot, shootSpeed * sinShoot, 0);
					TPScript.paintMaterial = actualTexture;
				}  

				if (bullet != null) {
					bullet.transform.position = new Vector3 (transform.position.x + (1.0f * cosShoot), transform.position.y + (1.0f * sinShoot), 0);
				}

			}


		}
	}
}