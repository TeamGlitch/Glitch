using UnityEngine;
using System.Collections;

public class LightChangeScript : MonoBehaviour {

    public float topLightDesiredLevel = 1.0f;
    public float timeToChange = 1f;

    public Light topLight;

    private float currentTopLight;
    private float timeChanging;

    public void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            currentTopLight = topLight.intensity;
            topLight.intensity = topLightDesiredLevel;
            timeChanging = 0f;
            StartCoroutine("ChangeLights");
        }
    }

    IEnumerator ChangeLights()
    {
        while (timeChanging < timeToChange)
        {
            timeChanging += Time.deltaTime;
            if (timeChanging >= timeToChange)
                timeChanging = timeToChange;
            topLight.intensity = Mathf.Lerp(currentTopLight, topLightDesiredLevel, timeChanging / timeToChange);
            yield return null;
        }
    }



}
