using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

    public PlayerController player;
    public float xSmooth = 20.0f;
	public float maxUp = 0.5f;
	public float maxDown = 0.33f;
	public Camera thisCamera;

    private float xSpeed;
	private int offsetX = 7;

    void Update()
    {
		
		if (Camera.current == thisCamera) 
		{

			float posy = transform.position.y;
			float posx;

			Vector3 positionOnViewport = Camera.current.WorldToViewportPoint(player.transform.position);

			// Camera only moves if it's over the 50% of the screen or over the 33% of the screen
			if (positionOnViewport.y > maxUp) 
			{
				Vector3 expectedPosition = Camera.current.ViewportToWorldPoint ( new Vector3(0, maxUp, positionOnViewport.z) );
				expectedPosition = expectedPosition - player.transform.position;
				posy -= expectedPosition.y;

			} else if (positionOnViewport.y < maxDown) {
				
				Vector3 expectedPosition = Camera.current.ViewportToWorldPoint ( new Vector3(0, maxDown, positionOnViewport.z) );
				expectedPosition = expectedPosition - player.transform.position;
				posy -= expectedPosition.y;
			}
				
			posx = Mathf.SmoothDamp(transform.position.x, player.transform.position.x + offsetX, ref xSpeed, xSmooth * Time.deltaTime);
			transform.position = new Vector3 (posx, posy, transform.position.z);
		}
    } 

}
