using UnityEngine;
using InControl;

public class TeleportScript : MonoBehaviour {

	//Teleport movement scale
	public float teleportDistance = 4.0f;

	public bool teleportUsed = false;

	private Vector3 initialPos;
	private Vector3 endPos;
	private float initialTime;
	private float endTime;

	public Vector3 returnPosition(){

		float timePassed = (Time.time - initialTime) / getDuration();

		if (timePassed >= 1) {
			timePassed = 1;
			teleportUsed = false;
		}

		return Vector3.Lerp(initialPos, endPos, timePassed);

	}

	//Checks if it can teleport to the given position
    public bool CheckTeleport(CharacterController controller)
    {
        // We get the teleport direction
        float directionVertical = InputManager.ActiveDevice.LeftStickY.Value;
        float directionHorizontal = InputManager.ActiveDevice.LeftStickX.Value;

        // Vector to know if the position to teleport is occupied
		endPos = controller.transform.position;
		endPos.x += teleportDistance * directionHorizontal;
		endPos.y += (teleportDistance * directionVertical) + 0.1f;

		LayerMask mask = -1;
		if (!Physics.CheckCapsule(endPos, endPos, controller.radius, mask, QueryTriggerInteraction.Ignore))
        {
			initialPos = transform.position;
			initialTime = Time.time;
			teleportUsed = true;

			if (controller.isGrounded)
			{
				// Wait for 0.3 seconds
				endTime = initialTime + 0.3f;
			}
			else
			{
				// Wait for 0.5 seconds
				endTime = initialTime + 0.5f;
			}

			if (controller.isGrounded) {
				endPos.y = controller.transform.position.y;
			}

            return true;
        }

        return false;
    }

	public float getDuration(){
		return endTime - initialTime;
	}
}