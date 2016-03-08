using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

    public Transform player;
    public float ySmooth = 0.1f;
    public float xSmooth = 20.0f;
	
    private float ySpeed;
    private float xSpeed;
	private int offsetX = 7;
	private int offsetY = 6;
	
    void Update()
    {
		// We use SmoothDamp with a begin and end point, velocity and smoothness
		// With a low smoothness reachs target faster
        float posy = Mathf.SmoothDamp(transform.position.y, player.transform.position.y + offsetY, ref ySpeed, ySmooth);
        float posx = Mathf.SmoothDamp(transform.position.x, player.transform.position.x + offsetX, ref xSpeed, xSmooth * Time.deltaTime);
        transform.position = new Vector3 (posx, posy, transform.position.z);
    } 
}
