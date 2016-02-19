using UnityEngine;
using System.Collections;

public class TeleportScript : MonoBehaviour {

    public bool Teleport(CharacterController controller, PlayerController playerScript)
    {
        // We catch the direction to teleport
        float directionVertical = Input.GetAxisRaw("Vertical");
        float directionHorizontal = Input.GetAxisRaw("Horizontal");

        if (controller.isGrounded)
        {
            directionVertical = 0;
        }
        
        // Vector to know if the position to teleport is occupied
        Vector3 newPosition;
        newPosition.x = transform.position.x + (transform.localScale.x * 2) * directionHorizontal;
        newPosition.y = transform.position.y + (transform.localScale.y * 2) * directionVertical + 0.1f;
        newPosition.z = transform.position.z;

		if (!Physics.CheckCapsule(newPosition, newPosition, transform.localScale.x / 2))
        {
            // Teleport always moves the player twice it's width in X axis 
            transform.Translate((transform.localScale.x * 2) * directionHorizontal, directionVertical * transform.localScale.y, 0.0f);
            if (!controller.isGrounded)
            {
                return true;
            }
        }
        return false;
    }
}
