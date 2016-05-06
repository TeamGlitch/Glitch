using UnityEngine;
using System.Collections;

public class GodCamera : MonoBehaviour {
    private Vector3 moveDirection;
    private int speed = 10;

	void Update () {
        moveDirection = Vector3.zero;
		// Inputs to move camera:
		// h: Left, k: Right, u: Up, j: Down
        if (Input.GetKey(KeyCode.H))
        {
            moveDirection.x = -Time.deltaTime * speed;

        } else if (Input.GetKey(KeyCode.K)){     
            moveDirection.x = Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.U))
        {
            moveDirection.y = Time.deltaTime * speed;

        } else if (Input.GetKey(KeyCode.J)){
            moveDirection.y = -Time.deltaTime * speed;
        }

        transform.Translate(moveDirection);
	}
}
