using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ToScene3 : MonoBehaviour
{

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            //TODO: NEEEDS TO USE LOADER
            SceneManager.LoadScene("Level Boss3");
        }
    }
}
