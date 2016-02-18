using UnityEngine;
using System.Collections;

public class TextureSwapper : MonoBehaviour {

	public GameObject sight;
	public GameObject stealerBullet;
	public GameObject paintBullet;

	//TerrainProperties

	private bool shootingMode = false;
	private float radius = 2.0f;
	private float shootSpeed = 0.45f;

	// Use this for initialization
	void Start () {
		sight.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Q) == true) {
			shootingMode = true;
			sight.SetActive(true);
		}
		if (Input.GetKeyUp(KeyCode.Q) == true) {
			shootingMode = false;
			sight.SetActive(false);
		}

		if (shootingMode == true) {

			//Mouse position
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = 10;
			//Screen dimensions to world dimensions
			mousePos = Camera.main.ScreenToWorldPoint(mousePos); 

			//Shooter position
			Vector3 character = transform.position;

			//Calculo de angulo
			Vector2 distance = mousePos - character;
			float angle = (Mathf.Atan2(distance.y,distance.x) * Mathf.Rad2Deg);
			if (angle < 0.0f) angle += 360.0f;
			angle *= Mathf.Deg2Rad;

			//Sight position
			sight.transform.position = new Vector3(character.x + (radius * Mathf.Cos(angle)), character.y + (radius * Mathf.Sin(angle)), -2);

			//Disparo stealer
			if(Input.GetMouseButtonUp (0) || Input.GetMouseButtonUp (1)){
				float cosShoot = Mathf.Cos(angle);
				float sinShoot = Mathf.Sin(angle);

				GameObject bullet = null;
				if (Input.GetMouseButtonUp (1)) { 						//Disparo stealer
					bullet  = (GameObject)Instantiate(stealerBullet);
					//bullet.GetComponent<StealerBulletScript>().speed = new Vector3 (shootSpeed * cosShoot, shootSpeed * sinShoot, 0);
				} 
				else if (Input.GetMouseButtonUp (0)) {					//Disparo paint
					bullet = (GameObject)Instantiate (paintBullet);
					//bullet.GetComponent<PaintBulletScript>().speed = new Vector3 (shootSpeed * cosShoot, shootSpeed * sinShoot, 0);
				}  

				if (bullet != null) {
					bullet.transform.parent = this.transform;
					bullet.transform.position = new Vector3 (character.x + (1.0f * cosShoot), character.y + (1.0f * sinShoot), 0);
				}

			}


		}
	}
}