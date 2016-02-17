using UnityEngine;
using System.Collections;

public class TeleportScript : PlayerController {

    public void Teleport(int direction)
    {
        float directionVertical = Input.GetAxis("Vertical");
        print (directionVertical);
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
        Vector3 newPosition = new Vector3(transform.localPosition.x + (transform.localScale.x * 2) * direction, transform.localPosition.y + (2 * transform.localScale.y) * directionVertical + 0.1f, transform.localPosition.z);

		if (!Physics.CheckCapsule(newPosition, newPosition, transform.localScale.x / 2))
        {
            transform.Translate((transform.localScale.x * 2) * direction, directionVertical * transform.localScale.y, 0.0f);
        }
    }
}
