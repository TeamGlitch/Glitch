using UnityEngine;
using System.Collections;
using InControl;

public class SpringPlantScript : MonoBehaviour {

	public PlayerController playerController;
	public float forceToPlayerWhenEnter = 10.0f;
	public float forceToPlayerWhenEnterAndJump = 25.0f;

	private Animator _animator;

	void Start()
	{
		_animator = transform.GetComponent<Animator> ();
	}

	public void EndAnimation()
	{
		_animator.SetBool ("PlayerInside", false);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			playerController.vSpeed = forceToPlayerWhenEnter;
			_animator.SetBool ("PlayerInside", true);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			if(InputManager.ActiveDevice.Action1.IsPressed)
				playerController.vSpeed = forceToPlayerWhenEnterAndJump;
			playerController.RestartTeleportCooldown ();
		}
	}

}
