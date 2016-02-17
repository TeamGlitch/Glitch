using UnityEngine;
using System.Collections;

public class TeleportScript : MonoBehaviour {

	CharacterController controller;
	bool alreadyUsed = false;

	void Start (){
		controller = GetComponent<CharacterController>();
	}

    void Update()
    {
		if (alreadyUsed == true && controller.isGrounded) {
			alreadyUsed = false;
		}

		if (Input.GetKeyDown (KeyCode.L) && alreadyUsed == false) {

			if (!controller.isGrounded){
				alreadyUsed = true;
			}
		
			float horizontalTranslation = Input.GetAxisRaw("Horizontal") * (transform.localScale.x * 2);
			float verticalTranslation = Input.GetAxisRaw("Vertical") * (2 * transform.localScale.y);

			Vector3 newPosition = new Vector3(transform.localPosition.x + horizontalTranslation, transform.localPosition.y + verticalTranslation + 0.1f, transform.localPosition.z);

			if (!Physics.CheckCapsule(newPosition, newPosition, transform.localScale.x / 2))
			{
				transform.Translate(horizontalTranslation, verticalTranslation, transform.localPosition.z);
			}

		}
    }

}