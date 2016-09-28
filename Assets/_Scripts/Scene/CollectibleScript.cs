using UnityEngine;
using System.Collections;

public class CollectibleScript : MonoBehaviour {

	public Player player;
    public AudioClip itemSound;
    public int orderNum;
    public bool isFalling = false;
    public BoxCollider collider;

    private Animator animator;

	void OnTriggerEnter(Collider collider)
	{
        if(collider.CompareTag("Player"))
        {
            player.IncreaseItem(this);
            gameObject.SetActive(false);
            SoundManager.instance.PlaySingle(itemSound);
        }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Camera.current != null)
        {
            Vector3 position = Camera.current.WorldToViewportPoint(transform.position);
            if (position.x > -0.5f && position.x < 1.5f &&
                position.y > -0.5f && position.y < 1.5f)
            {
                if (!animator.enabled)
                    animator.enabled = true;
            }
            else if (animator.enabled)
                animator.enabled = false;
        }
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
