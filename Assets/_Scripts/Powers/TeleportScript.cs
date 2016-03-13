using UnityEngine;
using InControl;

public class TeleportScript : MonoBehaviour {

	public float teleportDistance = 4.0f;

    public bool Teleport(CharacterController controller)
    {
        // We catch the direction to teleport
		float directionVertical = InputManager.ActiveDevice.LeftStickY.Value;
		float directionHorizontal = InputManager.ActiveDevice.LeftStickX.Value;

        if (controller.isGrounded)
        {
            directionVertical = 0;
        }
        
        // Vector to know if the position to teleport is occupied
        Vector3 newPosition;
		newPosition.x = controller.transform.position.x + (controller.transform.localScale.x * teleportDistance) * directionHorizontal;
		newPosition.y = controller.transform.position.y + (controller.transform.localScale.y * teleportDistance) * directionVertical + 0.1f;
        newPosition.z = controller.transform.position.z;

        if (!Physics.CheckCapsule(newPosition, newPosition, controller.transform.localScale.x / 2))
        {
            // Teleport always moves the player twice it's width in X axis 
			controller.transform.Translate((controller.transform.localScale.x * teleportDistance) * directionHorizontal, directionVertical * controller.transform.localScale.y, 0.0f);
            if (!controller.isGrounded)
            {
                return true;
            }
        }
        return false;
    }
}
