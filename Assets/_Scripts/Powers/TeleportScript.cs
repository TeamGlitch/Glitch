using UnityEngine;
using InControl;

public class TeleportScript : MonoBehaviour {

	//Teleport movement scale
	public float teleportDistance = 4.0f;

	public bool teleportUsed = false;

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
		float x = teleportDistance * directionHorizontal;
		float y = teleportDistance * directionVertical;
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
        // We get the teleport direction
        directionVertical = InputManager.ActiveDevice.LeftStickY.Value;
        directionHorizontal = InputManager.ActiveDevice.LeftStickX.Value;

        // Vector to know if the position to teleport is occupied
		Vector3 newPosition = controller.transform.position;
		newPosition.x += teleportDistance * directionHorizontal;
		newPosition.y += (teleportDistance * directionVertical) + 0.1f;

		LayerMask mask = -1;
		if (!Physics.CheckCapsule(newPosition, newPosition, controller.radius, mask, QueryTriggerInteraction.Ignore))
        {
            return true;
        }

        return false;
    }
}