using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ToScene2 : MonoBehaviour {

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Level Boss2");
        }
    }
}
