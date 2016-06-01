using UnityEngine;
using System.Collections;

public class CollectibleScript : MonoBehaviour {

	public Player player;
    public AudioClip itemSound;
    public int orderNum;

	void OnTriggerEnter(Collider collider)
	{
        player.IncreaseItem(this);
        gameObject.SetActive(false);
        SoundManager.instance.PlaySingle(itemSound);
	}
}
