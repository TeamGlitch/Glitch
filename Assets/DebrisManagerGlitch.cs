using UnityEngine;
using System.Collections;

public class DebrisManagerGlitch : MonoBehaviour {

    public Debris[] debris;
    public CameraShake shake;
    public AudioClip fall;

    private int rand1;
    private int rand2;
    private int rand3;

    public void Fall()
    {
        rand1 = Random.Range(0, debris.Length);
        debris[rand1].Fall();

        rand2 = Random.Range(0, debris.Length);
        debris[rand2].Fall();

        rand3 = Random.Range(0, debris.Length);
        debris[rand3].Fall();

        shake.shakeIt = true;

        SoundManager.instance.PlaySingle(fall);
        Invoke("Restart", 5.0f);
    }

    public void Restart()
    {
        debris[rand1].Restart();
        debris[rand2].Restart();
        debris[rand3].Restart();
        Invoke("Fall", 1.0f);
    }

    public void ArcherDead()
    {
        for (int i = 0; i < debris.Length; ++i)
        {
            debris[i].Reubicate();
        }
    }
}
