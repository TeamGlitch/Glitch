using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

    public Transform player;
    public float ySmooth = 0.05f;
    public float xSmooth = 5.0f;
    public float ySpeed = 0.0f;
    public float xSpeed = 5.0f;

    void Update()
    {
        float posy = Mathf.SmoothDamp(transform.position.y, player.transform.position.y + 6, ref ySpeed, ySmooth);
        float posx = Mathf.SmoothDamp(transform.position.x, player.transform.position.x + 7, ref xSpeed, xSmooth * Time.deltaTime);
        transform.position = new Vector3 (posx, posy, transform.position.z);
    } 
}
