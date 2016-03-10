using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

    public PlayerController player;
    public float ySmooth = 0.1f;
    public float xSmooth = 20.0f;
	
    private float ySpeed;
    private float xSpeed;
	private int offsetX = 7;
	private int offsetY = 6;
    private bool moveY = false;
	
    void Update()
    {
        float posy = transform.position.y;
        float posx;

        // Camera only moves if surpasses a limit of height in a jump. This limit is 4 times the Y scale of the player.
        if ((player.transform.position.y > player.preJumpPosY) || (player.state == PlayerController.player_state.IN_GROUND) || ((player.state == PlayerController.player_state.JUMPING) && (player.vSpeed < 0) && (moveY == true)))
        {
            moveY = true;
        }
        else
        {
            moveY = false;
        }

		// We use SmoothDamp with a begin and end point, velocity and smoothness
		// With a low smoothness reachs target faster
        if (moveY == true)
        {
            posy = Mathf.SmoothDamp(transform.position.y, player.transform.position.y + offsetY, ref ySpeed, ySmooth);
        }
        posx = Mathf.SmoothDamp(transform.position.x, player.transform.position.x + offsetX, ref xSpeed, xSmooth * Time.deltaTime);
        transform.position = new Vector3 (posx, posy, transform.position.z);
    } 
}
