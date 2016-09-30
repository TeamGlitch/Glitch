using UnityEngine;
using System.Collections;

public class TeleportBack : MonoBehaviour {

    public Player player;
    public StartPoint startpoint;
    public AudioClip audio;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            Vector3 newPosition = startpoint.transform.position;
            newPosition.z = player.transform.position.z;
            newPosition.y += 20f;
            player.transform.position = newPosition;
            SoundManager.instance.PlaySingle(audio);
        }
        

    }

}
