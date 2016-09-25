using UnityEngine;
using System.Collections;

public class ChangingGlobalFog : MonoBehaviour {

    public UnityStandardAssets.ImageEffects.GlobalFog globalFog;

    private float timeToChangeFog = 1f;
    public float endDistanceFog;
    private float startingDistanceFog;
    private float timeChanging;

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            timeChanging = 0f;
            startingDistanceFog = globalFog.startDistance;
        }
    }

    IEnumerator ChangeFog()
    {
        while (timeChanging < timeToChangeFog)
        {
            timeChanging += Time.deltaTime;
            if (timeChanging >= timeToChangeFog)
                timeChanging = timeToChangeFog;
            globalFog.startDistance = Mathf.Lerp(startingDistanceFog, endDistanceFog, timeChanging / timeToChangeFog);
            yield return null;
        }
    }


}