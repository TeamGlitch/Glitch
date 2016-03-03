using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	//External references
	public HUDLives guiLife;
	public HUDCollects guiItem;

	//Internal references
	private PlayerController playerController;

	//Properties
	public int lives;
	public int items = 0;						// Items collected

	// Use this for initialization
	void Start () {
		playerController = GetComponent<PlayerController>();
		lives = 3;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
