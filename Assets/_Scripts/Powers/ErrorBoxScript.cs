using UnityEngine;
using System.Collections;

public class ErrorBoxScript : MonoBehaviour
{

    public ErrorBoxCreator errorBoxCreator;
    public SpriteRenderer spriteRenderer;
    public BoxCollider boxCollider;
    public float startTime = -1;
    public float duration = -1;
    public float cooldown = -1;

    private bool active = true;
    private float nextFlicker = -1;


    void Update()
    {

        if (startTime != -1)
        {

            //If it's active
            if (active)
            {

                //Set first ficker if not stablished
                if (nextFlicker == -1)
                {
                    nextFlicker = Time.time + (duration - 1.0f);
                }

                //If it's flicker time
                if (Time.time >= nextFlicker)
                {

                    //Change alpha
                    if (spriteRenderer.color.a == 1)
                    {
                        spriteRenderer.color = new Color(1, 1, 1, 0.6f);
                    }
                    else
                    {
                        spriteRenderer.color = new Color(1, 1, 1, 1);
                    }

                    nextFlicker = Time.time + 0.15f;
                }

                //If it's time over, remove renderer and collider
                if (Time.time - startTime >= duration)
                {
                    spriteRenderer.enabled = false;
                    boxCollider.enabled = false;
                    active = false;
                }

                //If cooldown is over, disable the box
            }
            else if (!active && (Time.time - startTime >= cooldown))
            {

                errorBoxCreator.errorBoxDeleted(1);
                gameObject.SetActive(false);

            }

        }
    }

    //When is disabled, enable the sprite renderer and box collider
    //to be ready if the object pool calls it again
    void OnDisable()
    {

        active = true;

        if (spriteRenderer != null)
		{ 
			spriteRenderer.enabled = true;
		}
        if (boxCollider != null)
		{
			boxCollider.enabled = true;
		}

        nextFlicker = -1;
        spriteRenderer.color = new Color(1, 1, 1, 1);

    }
}
