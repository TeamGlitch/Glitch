using UnityEngine;
using System.Collections;

public class RootsManager : MonoBehaviour {
    public GlitchRoots [] roots;
    public BossArcherIA boss;
    public bool isActivable = false;

    public void AllGlitched()
    {
        for (int i = 0; i < 4; ++i)
        {
            roots[i].RootGlitched();
        }
        StartCoroutine(GlitchesAvailable(7.0f));
    }

    // Coroutine activate capacity of Glitch
    IEnumerator GlitchesAvailable(float wait)
    {
        yield return new WaitForSeconds(wait);
        for (int i = 0; i < 4; ++i)
        {
            roots[i].TurnToNormality();
        }
        if (boss.lives > 0)
        {
            isActivable = true;
        }
        else
        {
            isActivable = false;
        }
    }
}
