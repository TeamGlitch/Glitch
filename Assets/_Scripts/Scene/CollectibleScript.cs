using UnityEngine;
using System.Collections;

public class CollectibleScript : MonoBehaviour {

	public Player player;
    public AudioClip itemSound;

	void OnTriggerEnter(Collider collider)
	{
		player.IncreaseItem ();
		gameObject.SetActive (false);
        SoundManager.instance.PlaySingle(itemSound);
	}
}
