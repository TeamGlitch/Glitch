﻿using UnityEngine;
using System.Collections;
using InControl;
using UnityEngine.UI;
using System.Xml;

public class DynamicCamera : MonoBehaviour {

	public enum dynamic_camera_state
	{
		PANNING,
		WAITING,
		ZOOMING
	};

	public Transform player;
    public PlayerController playerC;
	public Camera mainCamera;
	public World world;
	public GameObject titles;
    public AdvanceBarEnemies advanceBarEnemies;

	public int speed = 10;
	public int zoomSpeed;

	private dynamic_camera_state state = dynamic_camera_state.PANNING;
	private float delay = 5.0f;
	private int offsetX = 7;
	private int zPosition = -12;

	private Vector3 zoomSpeedVector;

    void Start(){
        playerC.allowMovement = false;
    }

	void Update () {

		// If any button is pressed the intro is skipped
		if (InputManager.ActiveDevice.AnyButton.IsPressed) {
			titles.SetActive(false);
			transform.position = new Vector3(player.position.x + offsetX, player.position.y, zPosition);
			beginGame();

		} else {

			switch (state) {

			case dynamic_camera_state.PANNING:

				if (transform.position.x <= (player.position.x + offsetX)) {

                    state = dynamic_camera_state.WAITING;

                    //TODO: Change to getchild and have a own textasset
                    Text title = advanceBarEnemies.endPoint.title;
                    Text subtitle = advanceBarEnemies.endPoint.subtitle;
                    TextAsset XMLAsset = advanceBarEnemies.endPoint.XMLAsset;

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(XMLAsset.text);

                    XmlNode texts = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/StartMessage/Title");
                    title.text = texts.InnerText;
                    title.color = Color.red;

                    texts = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/StartMessage/Subtitle");
                    subtitle.text = texts.InnerText;

					titles.SetActive(true);

				} else {
					// Camera moves to left until reach player
					transform.Translate((Time.deltaTime) * -speed, 0.0f, 0.0f);
				}
				break;

			case dynamic_camera_state.WAITING:

				delay -= Time.deltaTime; 
				if (delay <= 0.0f) {
					state = dynamic_camera_state.ZOOMING;
					titles.SetActive(false);
					Vector3 destination = new Vector3 (player.transform.position.x, player.transform.position.y, zPosition);
					zoomSpeedVector = (destination - transform.position) / 2.0f;
				};
				break;

			case dynamic_camera_state.ZOOMING:

				transform.Translate(zoomSpeedVector * Time.deltaTime);
				if (transform.position.z >= zPosition)
				{
					beginGame();
				}
				break;
			}
		}
	}

	// Function that positions the main camera, active "World" (that active player
	// movements) and deactive this class.
	void beginGame()
	{
        playerC.allowMovement = true;
        advanceBarEnemies.Pause(false);
		mainCamera.transform.position = transform.position;
		world.enabled = true;
		gameObject.SetActive(false);
	} 
}
