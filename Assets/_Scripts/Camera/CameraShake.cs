using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

    private const float maxShakeTime = 0.1f;

    public Camera camera;
    public bool shakeIt = false;
    public int maxRepetitions = 3;

    private float shakeTime = 0;
    private bool up = false;
    private float minY;
    private float maxY;
    private float middleY;
    private bool right = false;
    private float minX;
    private float maxX;
    private float middleX;
    private int repetitions = 0;

    void Start()
    {
        middleY = camera.transform.position.y;
        minY = middleY - 1.25f;
        maxY = middleY + 1.25f;
        middleX = camera.transform.position.x;
        minX = middleX - 1.0f;
        maxX = middleX + 1.0f;
    }

	void Update () {
        if (shakeIt)
        {
            if (up)
            {
                shakeTime += Time.deltaTime;
                camera.transform.position = new Vector3(Mathf.Lerp(minX, maxX, shakeTime / maxShakeTime), Mathf.Lerp(minY, maxY, shakeTime / maxShakeTime), camera.transform.position.z);

                if (shakeTime > maxShakeTime)
                {
                    up = false;
                    shakeTime = 0.0f;
                }
            }
            else
            {
                shakeTime += Time.deltaTime;
                camera.transform.position = new Vector3(Mathf.Lerp(maxX, minX, shakeTime / maxShakeTime), Mathf.Lerp(maxY, minY, shakeTime / maxShakeTime), camera.transform.position.z);

                if (shakeTime > maxShakeTime)
                {
                    up = true;
                    shakeTime = 0.0f;
                    ++repetitions;
                    if (repetitions == maxRepetitions)
                    {
                        shakeIt = false;
                        repetitions = 0;
                    }
                }
            }
        }
	}
}
