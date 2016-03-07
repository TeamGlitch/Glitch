using UnityEngine;
using System.Collections;

public class DynamicCamera : MonoBehaviour {
    public Transform player;
    public Camera mainCamera;
    public World world;
    public GameObject titles;

    private int speed = 10;
    private float delay = 5.0f;

    void Update () {
        if (transform.position.x <= (player.position.x + 7))
        {
            delay -= Time.deltaTime;
            titles.SetActive(true);
            if (delay <= 0.0f)
            {
                titles.SetActive(false);
                transform.Translate(0.0f, 0.0f, speed*Time.deltaTime);
                if (transform.position.z >= -15)
                {
                    beginGame();
                }
            }
        }
        else
        {
            transform.Translate((-Time.deltaTime) * speed, 0.0f, 0.0f);
        }
    }

    void beginGame()
    {
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -15);
        world.enabled = true;
        gameObject.SetActive(false);
    } 
}
