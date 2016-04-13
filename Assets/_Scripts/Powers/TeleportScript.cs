using UnityEngine;
using InControl;

public class TeleportScript : MonoBehaviour {

	//Teleport movement scale
	public float teleportDistance = 4.0f;

    private float directionVertical;
    private float directionHorizontal;

	//Teleports the character. Returns true if there's cooldown.
    public bool Teleport(CharacterController controller)
    {
    
        if (controller.isGrounded)
        {
            directionVertical = 0;
        }
      
        // Teleport moves the character in a scale proportional to its size
		float x = (controller.transform.localScale.x * teleportDistance) * directionHorizontal;
		float y = (controller.transform.localScale.y * teleportDistance) * directionVertical;
		controller.transform.Translate(x, y, 0.0f);

        if (controller.isGrounded)
        {
            return false;
        }

        return true;
    }

	//Checks if it can teleport to the given position
    public bool CheckTeleport(CharacterController controller)
    {
        // We catch the direction to teleport
        directionVertical = InputManager.ActiveDevice.LeftStickY.Value;
        directionHorizontal = InputManager.ActiveDevice.LeftStickX.Value;

        // Vector to know if the position to teleport is occupied
        Vector3 newPosition;
        newPosition.x = controller.transform.position.x + (controller.transform.localScale.x * teleportDistance) * directionHorizontal;
        newPosition.y = controller.transform.position.y + (controller.transform.localScale.y * teleportDistance) * directionVertical + 0.1f;
        newPosition.z = controller.transform.position.z;

        if (!Physics.CheckCapsule(newPosition, newPosition, controller.transform.localScale.x / 2))
        {
            return true;
        }

        return false;
    }
}
