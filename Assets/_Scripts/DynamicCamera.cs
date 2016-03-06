using UnityEngine;
using System.Collections;

public class DynamicCamera : MonoBehaviour {
    public Transform player;
    public Camera mainCamera;
    public GameObject powers;

    private int speed = 10;
    private float delay = 5.0f;
    private PlayerController movePlayer;

    void Start()
    {
        movePlayer = player.GetComponent<PlayerController>();
    }

    void Update () {
        if (transform.position.x <= (player.position.x + 7))
        {
            delay -= Time.deltaTime;
            if (delay <= 0.0f)
            {
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
        mainCamera.gameObject.SetActive(true);
        movePlayer.enabled = true;
        powers.SetActive(true);
        gameObject.SetActive(false);
    } 
}
