using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

    public PlayerController player;
    public float ySmooth;
    public float xSmooth;
	
    private float ySpeed;
    private float xSpeed;
	private int offsetX = 7;
	private int offsetY = 6;
    private bool moveY = false;
    private float posy;
    private float posx;
	
    void Update()
    {
        // Camera only moves if surpasses a limit of height in a jump. This limit is 4 times the Y scale of the player.
        if ((player.transform.position.y > player.cameraYBound) || (player.state == PlayerController.player_state.IN_GROUND) || 
            ((player.state == PlayerController.player_state.JUMPING) && (player.vSpeed < 0) && (moveY == true)))
        {
            moveY = true;

            // We use SmoothDamp with a begin and end point, velocity and smoothness
            // With a low smoothness reachs target faster
            posy = Mathf.SmoothDamp(transform.position.y, player.transform.position.y + offsetY, ref ySpeed, ySmooth);
        }
        else
        {
            moveY = false;
            posy = transform.position.y;
        }

        posx = Mathf.SmoothDamp(transform.position.x, player.transform.position.x + offsetX, ref xSpeed, xSmooth * Time.deltaTime);
        transform.position = new Vector3 (posx, posy, transform.position.z);
    } 

}
