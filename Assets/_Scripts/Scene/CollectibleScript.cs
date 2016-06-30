using UnityEngine;
using System.Collections;

public class CollectibleScript : MonoBehaviour {

	public Player player;
    public AudioClip itemSound;
    public int orderNum;
    public bool isFalling = false;
    public BoxCollider collider;

	void OnTriggerEnter(Collider collider)
	{
        player.IncreaseItem(this);
        gameObject.SetActive(false);
        SoundManager.instance.PlaySingle(itemSound);
	}

    public void Parable()
    {
        StartCoroutine(StopItems(0.55f));
    }

    // Coroutine to activate colliders in x time
    IEnumerator StopItems(float wait)
    {
        yield return new WaitForSeconds(wait);
        GetComponent<Rigidbody>().isKinematic = true;
        collider.enabled = true;
    }
}
