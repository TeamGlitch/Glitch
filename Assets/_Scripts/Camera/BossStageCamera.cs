using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class BossStageCamera : MonoBehaviour {

    private const float maxZoom = 47.2f;
    private const float minZoom = 60.0f;
    private const float archerZoom = 15.0f;
    private const float glitchZoom = 20.0f;
    private const float maxZoomTime = 0.9f;
    private const float maxDarkTime = 1.0f;
    private const float maxDarkness = 1.0f;
    private const float minDarkness = 0.3f;
    private const float maxFisheye = 0.7f;
    private const float minFisheye = 0.0f;

    public enum camera_state
    {
        ZOOM_OUT,
        WAITING,
        ZOOM_IN_ARCHER,
        ZOOM_OUT_ARCHER,
        ZOOM_OUT_GLITCH,
        FINAL_ZOOM_OUT,
        DARKNESS_IN,
        DARKNESS_OUT
    };

    public Transform cameraInitPoint;
    public Transform archer;
    public CameraShake shake;
    public GameObject colliders;
    public Transform startPoint;
    public Player player;
    public PlayerController playerCon;
    public RectTransform topLeft;
    public RectTransform bossLives;
    public GameObject hud;
    public World world;
    public camera_state mode;

    private VignetteAndChromaticAberration darkness;
    private Fisheye fisheye;
    private Camera camera;
    private bool inZoom = false;
    private float zoomTime = 0;
    private Vector3 initialPosition;
    private Vector3 archerPosition;
    private Rigidbody playerRigid;
    private BoxCollider playerCollider;
    private float newYPosition;
    private float height;
    private BoxCollider playerCol;
    private Rigidbody playerRig;
    private float archerElevation = -5;
    private float archerXoffset = 0;
    private float pix;

    void Start()
    {
        camera = GetComponent<Camera>();
        darkness = GetComponent<VignetteAndChromaticAberration>();
        fisheye = GetComponent<Fisheye>();
        playerCol = player.GetComponent<BoxCollider>();
        playerRig = player.GetComponent<Rigidbody>();
        initialPosition = camera.transform.position;        // Position of camera normally
        playerRigid = player.GetComponent<Rigidbody>();
        playerCollider = player.GetComponent<BoxCollider>();

        // Setting of camera
        camera.fieldOfView = glitchZoom;
        camera.transform.position = new Vector3(cameraInitPoint.position.x, cameraInitPoint.position.y, camera.transform.position.z);
        
        // Adjusting screen
        float targetaspect = 16.0f / 9.0f;
        float windowaspect = (float)Screen.width / (float)Screen.height;
        float scaleheight = windowaspect / targetaspect;
        Rect rect = camera.rect;
        
        if (scaleheight < 1.0f)
        {

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0.0f;
            rect.y = (1.0f - scaleheight)/2.0f;

            camera.rect = rect;

            // Adjusting UI
            height = Screen.currentResolution.height;
            pix = camera.pixelHeight;
            newYPosition = ((pix - height) / 2.0f);
            topLeft.anchoredPosition = new Vector2(topLeft.anchoredPosition.x, newYPosition);
            bossLives.anchoredPosition = new Vector2(bossLives.anchoredPosition.x, newYPosition);
        }
        
    }

    void Update()
    {
        switch (mode)
        {

            case camera_state.ZOOM_OUT:
                zoomTime += Time.deltaTime;
                camera.fieldOfView = Mathf.Lerp(maxZoom, minZoom, zoomTime / maxZoomTime);

                if (zoomTime > maxZoomTime)
                {
                    mode = camera_state.WAITING;
                    zoomTime = 0.0f;
                    player.isInZoom = false;
                }
                break;

            case camera_state.ZOOM_IN_ARCHER:
                if (shake.shakeIt)
                {
                    camera.transform.position = initialPosition;
                    shake.shakeIt = false;
                }
                zoomTime += Time.deltaTime;
                camera.fieldOfView = Mathf.Lerp(maxZoom, archerZoom, zoomTime / maxZoomTime);
                camera.transform.position = new Vector3(Mathf.Lerp(initialPosition.x, archerPosition.x + archerXoffset, zoomTime / maxZoomTime),
                    Mathf.Lerp(initialPosition.y, archerPosition.y + archerElevation, zoomTime / maxZoomTime), camera.transform.position.z);

                if (zoomTime > maxZoomTime)
                {
                    mode = camera_state.WAITING;
                    zoomTime = 0.0f;
                }
                break;

            case camera_state.ZOOM_OUT_ARCHER:
                if (shake.shakeIt)
                {
                    camera.transform.position = initialPosition;
                    shake.shakeIt = false;
                }
                zoomTime += Time.deltaTime;
                camera.fieldOfView = Mathf.Lerp(archerZoom, maxZoom, zoomTime / maxZoomTime);
                camera.transform.position = new Vector3(Mathf.Lerp(archerPosition.x + archerXoffset, initialPosition.x, zoomTime / maxZoomTime),
                    Mathf.Lerp(archerPosition.y + archerElevation, initialPosition.y, zoomTime / maxZoomTime), camera.transform.position.z);

                if (zoomTime > maxZoomTime)
                {
                    mode = camera_state.WAITING;
                    zoomTime = 0.0f;
                    player.isInZoom = false;
                }
                break;

            case camera_state.ZOOM_OUT_GLITCH:
                zoomTime += Time.deltaTime;
                camera.fieldOfView = Mathf.Lerp(glitchZoom, minZoom, zoomTime / maxZoomTime);
                camera.transform.position = new Vector3(Mathf.Lerp(cameraInitPoint.position.x, initialPosition.x, zoomTime / maxZoomTime),
                    Mathf.Lerp(cameraInitPoint.position.y, initialPosition.y, zoomTime / maxZoomTime), camera.transform.position.z);

                if (zoomTime > maxZoomTime)
                {
                    mode = camera_state.WAITING;
                    zoomTime = 0.0f;
                    player.isInZoom = false;
                }
                break;

            case camera_state.FINAL_ZOOM_OUT:
                if (shake.shakeIt)
                {
                    camera.transform.position = initialPosition;
                    shake.shakeIt = false;
                }
                zoomTime += Time.deltaTime;
                camera.fieldOfView = Mathf.Lerp(archerZoom, minZoom, zoomTime / maxZoomTime);
                camera.transform.position = new Vector3(Mathf.Lerp(archerPosition.x + archerXoffset, initialPosition.x, zoomTime / maxZoomTime),
                    Mathf.Lerp(archerPosition.y + archerElevation, initialPosition.y, zoomTime / maxZoomTime), camera.transform.position.z);

                if (zoomTime > maxZoomTime)
                {
                    mode = camera_state.WAITING;
                    zoomTime = 0.0f;
                    player.isInZoom = false;
                }
                break;

            case camera_state.DARKNESS_IN:
                zoomTime += Time.deltaTime;
                darkness.intensity = Mathf.Lerp(minDarkness, maxDarkness, zoomTime / maxDarkTime);
                fisheye.strengthX = Mathf.Lerp(minFisheye, maxFisheye, zoomTime / maxZoomTime);

                if (zoomTime > maxDarkTime)
                {
                    mode = camera_state.WAITING;
                    zoomTime = 0.0f;
                }
                break;

            case camera_state.DARKNESS_OUT:
                zoomTime += Time.deltaTime;
                darkness.intensity = Mathf.Lerp(maxDarkness, minDarkness, zoomTime / maxDarkTime);
                fisheye.strengthX = Mathf.Lerp(maxFisheye, minFisheye, zoomTime / maxZoomTime);

                if (zoomTime > maxDarkTime)
                {
                    mode = camera_state.ZOOM_OUT_GLITCH;
                    zoomTime = 0.0f;
                    hud.SetActive(true);
                    world.enabled = true;
                }
                break;
        }
    }

    public void ZoomInBattle()
    {
        startPoint.localPosition = new Vector3(-8.8f, startPoint.localPosition.y, startPoint.localPosition.z);
        mode = camera_state.ZOOM_OUT_ARCHER;
        colliders.SetActive(true);
        playerRig.isKinematic = false;
        playerCol.isTrigger = false;
    }

    public void ZoomOut()
    {
        mode = camera_state.ZOOM_OUT;
        colliders.SetActive(false);
        player.isInZoom = true;
    }

    public void ZoomArcherIn()
    {
        archerPosition = archer.transform.position;
        playerRigid.isKinematic = true;
        playerCollider.isTrigger = true;
        player.isInZoom = true;
        mode = camera_state.ZOOM_IN_ARCHER;
    }

    public void FinalZoomArcherIn()
    {
        archerElevation = 0;
        archerXoffset = 7;
        archerPosition = archer.transform.position;
        playerRigid.isKinematic = true;
        playerCollider.isTrigger = true;
        player.isInZoom = true;
        mode = camera_state.ZOOM_IN_ARCHER;
    }

    public void FinalZoomArcherOut()
    {
        playerRigid.isKinematic = false;
        playerCollider.isTrigger = false;
        player.isInZoom = false;
        mode = camera_state.FINAL_ZOOM_OUT;
    }

    public void ZoomArcherOut()
    {
        playerCollider.isTrigger = false;
        playerRigid.isKinematic = false;
        mode = camera_state.ZOOM_OUT_ARCHER;   
    }

    public void DarknessIn()
    {
        hud.SetActive(false);
        playerRigid.isKinematic = true;
        playerCollider.isTrigger = true;
        player.isInZoom = true;
        mode = camera_state.DARKNESS_IN;
    }
}