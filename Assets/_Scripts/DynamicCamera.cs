using UnityEngine;
using System.Collections;

public class DynamicCamera : MonoBehaviour {
    public Transform player;
    public Camera mainCamera;

    private int speed = 10;
    private float delay = 5.0f;

    void Update () {
        if (transform.position.x <= (player.position.x + 7))
        {
            delay -= Time.deltaTime;
            if (delay <= 0.0f)
            {
                transform.Translate(0.0f, 0.0f, speed*Time.deltaTime);
                if (transform.position.z >= -15)
                {
                    mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -15);
                    mainCamera.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
        }
        else
        {
            transform.Translate((-Time.deltaTime) * speed, 0.0f, 0.0f);
        }
    }
}
