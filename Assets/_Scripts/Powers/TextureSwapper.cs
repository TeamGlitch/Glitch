using UnityEngine;
using System.Collections;
using InControl;

public class TextureSwapper : MonoBehaviour {

	public HUDTexture HUD_tex;							//UI reference
	public GameObject sight;							//Sight reference
	public Canvas gui;
    public GameObject bullet;
    public float shootSpeed = 0.45f;					//Speed of the bullet
    public float radius = 2.0f; 						//Radius of the sight transformation
    public float shootCooldown = 0.1f;					//Cooldown between shoots
    public Material actualTexture = null;				//Actual texture used for painting
    public RectTransform sightRectTransform;			//Transformation of the sight

    private ObjectPool bulletPool;
	private bool shootingMode = false;					//If it is in shooting mode
	private float lastShootStart = 0.0f;				//When the last shoot started
	private RectTransform guiRectTrans;


	void Start () 
    {
        bulletPool = new ObjectPool(bullet);
		sight.SetActive(false);
        guiRectTrans = gui.GetComponent<RectTransform>();
	}
	

	void Update () 
    {

		//Shooting mode activation - desactivation
		if (InputManager.ActiveDevice.LeftTrigger.WasPressed) 
        {
			shootingMode = true;
			sight.SetActive(true);
		}
		if (InputManager.ActiveDevice.LeftTrigger.WasReleased) 
        {
			shootingMode = false;
			sight.SetActive(false);
		}

		//If in shooter mode
		if (shootingMode) 
        {

			//Calcule the distance to where the sight is aiming at
			Vector2 distance;

			if (InputManager.ActiveDevice.Name != "Keyboard/Mouse") 
            {	
				//If a controller is being used
				distance = new Vector2 (InputManager.ActiveDevice.RightStickX * 100, InputManager.ActiveDevice.RightStickY * 100);
			} 
            else 
            {
				//If a mouse is being used
				Vector3 mousePos = Input.mousePosition;
				mousePos.z = transform.position.z - Camera.main.transform.position.z;
				mousePos = Camera.main.ScreenToWorldPoint(mousePos); 
				distance = mousePos - transform.position;
			}

			//Angle calculation
			float angle = (Mathf.Atan2(distance.y,distance.x) * Mathf.Rad2Deg);
            if (angle < 0.0f)
            {
                angle += 360.0f;
            }
			angle *= Mathf.Deg2Rad;

			//Sight position
			Vector3 sightPosition = new Vector3(transform.position.x + (radius * Mathf.Cos (angle)), transform.position.y + (radius * Mathf.Sin (angle)), 0);
			Vector3 camPosition = Camera.main.WorldToScreenPoint(sightPosition);

			camPosition.x *= guiRectTrans.rect.width / Camera.main.pixelWidth; 
			camPosition.y *= guiRectTrans.rect.height / Camera.main.pixelHeight; 

			sightRectTransform.anchoredPosition = camPosition;

            //Checks if it can shoot
            //If the paint bullet button is pressed, there is a paint texture and the shoot cooldown is over
            //If the stealer bullet button is pressed and the shoot cooldown x 4 is over
            //shoot = -1 do not shoot, 0 shoot painter, 1 shoot stealer
            int shoot = -1;
            if (InputManager.ActiveDevice.RightTrigger.IsPressed && (actualTexture != null) && (Time.time - lastShootStart > shootCooldown))
            {
                shoot = 0;
            }
            else if (InputManager.ActiveDevice.RightBumper.IsPressed && (Time.time - lastShootStart > shootCooldown * 4))
            {
                shoot = 1;
            }

            //Shoot
            if (shoot != -1)
            {

                //Calcule sinus and consinus
                float cosShoot = Mathf.Cos(angle);
                float sinShoot = Mathf.Sin(angle);

                GameObject bullet = null;

                //Stealer
                if (shoot == 1)
                {
                    bullet = bulletPool.getObject();
                    TextureBullet textBullet = bullet.GetComponent<TextureBullet>();
                    textBullet.speed = new Vector3(shootSpeed * cosShoot, shootSpeed * sinShoot, 0);
                    textBullet.textureSwapper = this;
                    textBullet.painter = false;
                }
                //Painter
                else if (shoot == 0)
                {
                    bullet = bulletPool.getObject();
                    TextureBullet textBullet = bullet.GetComponent<TextureBullet>();
                    textBullet.speed = new Vector3(shootSpeed * cosShoot, shootSpeed * sinShoot, 0);
                    textBullet.paintMaterial = actualTexture;
                    textBullet.painter = true;
                }

                //Move the bullet a bit forward and create a shoot coldown reference point
                if (bullet != null)
                {
                    bullet.transform.position = new Vector3(transform.position.x + (1.0f * cosShoot), transform.position.y + (1.0f * sinShoot), 0);
                    lastShootStart = Time.time;
                }

            }

		}
	}
}