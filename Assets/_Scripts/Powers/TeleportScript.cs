using UnityEngine;
using InControl;

public class TeleportScript : MonoBehaviour {

	//Teleport movement scale
	public float teleportDistance = 2.0f;
    public AudioClip TeleportSound;
	public bool teleportUsed = false;

	private Vector3 initialPos;
	private Vector3 endPos;
	private float initialTime;
	private float endTime;
    private float distToGround;

    private int layerMask = ~((1 << 1) | (1 << 2) | (1 << 4) | (1 << 5) | (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 13));

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

        distToGround = collider.bounds.extents.y;
        // We get the teleport direction
        float directionVertical = InputManager.ActiveDevice.LeftStickY.Value;
        float directionHorizontal = InputManager.ActiveDevice.LeftStickX.Value;

        endPos = collider.transform.position;

		if(directionHorizontal > 0.0f && directionVertical == 0.0f)
        {
            endPos.x += collider.bounds.extents.x * 2.0f + teleportDistance;
        }
		else if(directionHorizontal < 0.0f && directionVertical == 0.0f)
        {
            endPos.x -= (collider.bounds.extents.x * 2.0f + teleportDistance);
        }
		else if(directionHorizontal == 0.0f && directionVertical > 0.0f)
        {
            endPos.y += collider.bounds.extents.y * 2.0f + teleportDistance;
        }
		else if(directionHorizontal == 0.0f && directionVertical < 0.0f)
        {
            endPos.y -= (collider.bounds.extents.y * 2.0f + teleportDistance);
        }
		else
	    {
			float aux = Mathf.Cos(Mathf.PI / 4.0f);
			if(directionHorizontal > 0.0f)
            {
        		endPos.x += collider.bounds.extents.x * 2.0f + teleportDistance * aux;
            }
			else
            {
           		endPos.x -= (collider.bounds.extents.x * 2.0f + teleportDistance * aux);
            }

			if(directionVertical > 0.0f)
            {
        		endPos.y += collider.bounds.extents.y * 2.0f + teleportDistance * aux;
            }
			else
            {
                endPos.y -= (collider.bounds.extents.y * 2.0f + teleportDistance * aux);
            }
		}

        Debug.DrawLine(new Vector3(endPos.x - collider.bounds.extents.x, endPos.y + collider.bounds.extents.y, endPos.z),
            new Vector3(endPos.x + collider.bounds.extents.x, endPos.y + collider.bounds.extents.y, endPos.z),
            Color.red, 100.0f, false);
        Debug.DrawLine(new Vector3(endPos.x + collider.bounds.extents.x, endPos.y + collider.bounds.extents.y, endPos.z),
            new Vector3(endPos.x + collider.bounds.extents.x, endPos.y - collider.bounds.extents.y, endPos.z),
            Color.red, 100.0f, false);
        Debug.DrawLine(new Vector3(endPos.x - collider.bounds.extents.x, endPos.y - collider.bounds.extents.y, endPos.z),
            new Vector3(endPos.x + collider.bounds.extents.x, endPos.y - collider.bounds.extents.y, endPos.z),
            Color.red, 100.0f, false);
        Debug.DrawLine(new Vector3(endPos.x - collider.bounds.extents.x, endPos.y + collider.bounds.extents.y, endPos.z),
            new Vector3(endPos.x - collider.bounds.extents.x, endPos.y - collider.bounds.extents.y, endPos.z),
            Color.red, 100.0f, false);

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
                endPos.y = collider.transform.position.y;
            }
			else
			{
				// Wait for 0.5 seconds
				endTime = initialTime + 0.5f;
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
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f, layerMask);
    }


}