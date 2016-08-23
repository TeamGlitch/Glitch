using UnityEngine;
using System.Collections;

public class DebrisManagerGlitch : MonoBehaviour {

    public Debris[] debris;
    public CameraShake shake;
    public AudioClip fall;
    public BossArcherIA boss;

    private int rand1;
    private int rand2;
    private int rand3;
    private float rand;

    public void Fall()
    {
        rand1 = Random.Range(0, debris.Length);
        if (debris[rand1].mode == Debris.debris_state.WAITING)
        {
            debris[rand1].Fall();
        }

        if (debris[rand2].mode == Debris.debris_state.WAITING)
        {
            rand2 = Random.Range(0, debris.Length);
            debris[rand2].Fall();
        }

        if (debris[rand3].mode == Debris.debris_state.WAITING)
        {
            rand3 = Random.Range(0, debris.Length);
            debris[rand3].Fall();
        }

        shake.shakeIt = true;

        SoundManager.instance.PlaySingle(fall);
        if (boss.lives == 0) 
        {
            rand = Random.Range(2.0f, 6.0f);
            Invoke("Fall", rand);
        }
        else
        {
            Invoke("Fall", 10.0f);
        }
    }

    public void ArcherDead()
    {
        for (int i = 0; i < debris.Length; ++i)
        {
            debris[i].Reubicate();
            debris[i].Restart();
        }
    }
}
