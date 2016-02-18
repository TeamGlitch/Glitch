using UnityEngine;
using System.Collections;

public class TeleportScript : PlayerController {

    public player_state Teleport()
    {
        // We catch the direction to teleport
        float directionVertical = Input.GetAxis("Vertical");
        float directionHorizontal = Input.GetAxis("Horizontal");

        // The distance of teleport is always the same (positive, negative or neutral)
        // Vertical movement
        if (directionVertical > 0.0f)
        {
            directionVertical = 1.0f;
        }
        else
        {
            if (directionVertical < 0.0f)
            {
                directionVertical = -1.0f;
            }
        }

        // Horizontal movement
        if (directionHorizontal < 0)
        {
            directionHorizontal = -1;
        }
        else
        {
            if (directionHorizontal > 0)
            {
                directionHorizontal = 1;
            }
        }
        
        // Vector to know if the position to teleport is occupied
        Vector3 newPosition;
        newPosition.x = transform.position.x + (transform.localScale.x * 2) * directionHorizontal;
        newPosition.y = transform.position.y + (2 * transform.localScale.y) * directionVertical + 0.1f;
        newPosition.z = transform.position.z;

		if (!Physics.CheckCapsule(newPosition, newPosition, transform.localScale.x / 2))
        {
            // Teleport always moves the player twice it's width in X axis 
            transform.Translate((transform.localScale.x * 2) * directionHorizontal, directionVertical * transform.localScale.y, 0.0f);
        }
        return player_state.FALLING;
    }
}
