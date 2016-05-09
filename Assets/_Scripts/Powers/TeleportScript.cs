using UnityEngine;
using InControl;

public class TeleportScript : MonoBehaviour {

	//Teleport movement scale
	public float teleportDistance = 4.0f;
    public AudioClip TeleportSound;
	public bool teleportUsed = false;

	private Vector3 initialPos;
	private Vector3 endPos;
	private float initialTime;
	private float endTime;
    private float distToGround;

	public bool movePosition(out Vector3 position){

		float timePassed = (Time.time - initialTime) / getDuration();
		bool ended = false;

		if (timePassed >= 1) {
			timePassed = 1;
			ended = true;
		}

		position = Vector3.Lerp(initialPos, endPos, timePassed);

		return ended;
	}

	//Checks if it can teleport to the given position
    public bool CheckTeleport(BoxCollider collider)
    {

		float proportion = collider.bounds.extents.y / collider.bounds.extents.x;
		float teleportVerticalDistance = proportion * teleportDistance;

        distToGround = collider.bounds.extents.y;
        // We get the teleport direction
        float directionVertical = InputManager.ActiveDevice.LeftStickY.Value;
        float directionHorizontal = InputManager.ActiveDevice.LeftStickX.Value;

		if(directionHorizontal > 0.0f && directionVertical == 0.0f)
			directionHorizontal = 1.0f;
        else if (directionHorizontal < 0.0f && directionVertical == 0.0f)
            directionHorizontal = -1.0f;
        else if (directionHorizontal == 0.0f && directionVertical > 0.0f)
			directionVertical = 1.0f;
        else if (directionHorizontal == 0.0f && directionVertical < 0.0f)
            directionVertical = -1.0f;
        else
	    {
			float aux = Mathf.Cos(Mathf.PI / 4.0f);
			if(directionHorizontal > 0.0f)
				directionHorizontal = aux;
			else
				directionHorizontal = -aux;

			if(directionVertical > 0.0f)
				directionVertical = aux;
			else
				directionVertical = -aux;
		}

        Debug.Log("DIRECTIONAL HORIZONTAL: " + directionHorizontal);
        Debug.Log("DIRECTIONAL VERTICAL: " + directionVertical);

        // Vector to know if the position to teleport is occupied
        endPos = collider.transform.position;
        endPos.x += teleportDistance * collider.bounds.extents.x * 2.0f * directionHorizontal;
        endPos.y += teleportDistance * collider.bounds.extents.y * 2.0f * directionVertical + 0.1f;

		LayerMask mask = -1;
		if (!Physics.CheckBox(endPos, collider.bounds.extents, collider.transform.rotation, mask, QueryTriggerInteraction.Ignore))
        {
            SoundManager.instance.PlaySingle(TeleportSound);
			initialPos = transform.position;
			initialTime = Time.time;
			teleportUsed = true;

            if (IsGrounded())
			{
				// Wait for 0.3 seconds
				endTime = initialTime + 0.3f;
			}
			else
			{
				// Wait for 0.5 seconds
				endTime = initialTime + 0.5f;
			}

            if (IsGrounded())
            {
				endPos.y = collider.transform.position.y;
			}

            return true;
        }

        return false;
    }

	public float getDuration(){
		return endTime - initialTime;
	}

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }


}