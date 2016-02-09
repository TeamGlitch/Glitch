using UnityEngine;
using System.Collections;

public class TeleportScript : PlayerController {

    public void RightTeleport(int direction)
    {
        Vector3 newPosition = new Vector3(transform.localPosition.x + (transform.localScale.x * 2) * direction, transform.localPosition.y + 0.1f, transform.localPosition.z);
        Vector3 halfExtents = new Vector3(transform.localScale.x / 2, transform.localScale.y / 2, transform.localScale.z / 2);
        if (!Physics.CheckBox(newPosition, halfExtents))
        {
            transform.Translate((transform.localScale.x * 2) * direction, 0.0f, 0.0f);
        }
    }
}
