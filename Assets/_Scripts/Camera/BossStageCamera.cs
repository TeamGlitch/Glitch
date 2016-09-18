using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossStageCamera : MonoBehaviour {

    private const float maxZoom = 47.2f;
    private const float minZoom = 60.0f;
    private const float archerZoom = 15.0f;
    private const float maxZoomTime = 0.5f;

    public enum camera_state
    {
        ZOOM_IN,
        ZOOM_OUT,
        WAITING,
        ZOOM_IN_ARCHER,
        ZOOM_OUT_ARCHER
    };

    public Transform archer;
    public CameraShake shake;
    public GameObject colliders;
    public Transform startPoint;
    public Player player;
    public PlayerController playerCon;
    public RectTransform topLeft;
    public RectTransform bossLives;
    public camera_state mode = camera_state.WAITING;

    private Camera camera;
    private bool inZoom = false;
    private float zoomTime = 0;
    private Vector3 initialPosition;
    private Vector3 archerPosition;
    private Rigidbody playerRigid;
    private BoxCollider playerCollider;
    private float newYPosition;
    private float height;

    void Start()
    {
        camera = GetComponent<Camera>();
        initialPosition = camera.transform.position;
        playerRigid = player.GetComponent<Rigidbody>();
        playerCollider = player.GetComponent<BoxCollider>();
        
        float targetaspect = 16.0f / 9.0f;
        float windowaspect = (float)Screen.width / (float)Screen.height;
        float scaleheight = windowaspect / targetaspect;
        Rect rect = camera.rect;

        if (scaleheight < 1.0f)
        {

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = ((1.0f - scaleheight) / 2.0f)*scaleheight;

            camera.rect = rect;
        }
        else
        {
            float scalewidth = 1.0f / scaleheight;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }

        height = Screen.currentResolution.height;
        print(height);
        print(Screen.height);
        print(camera.pixelHeight);
        int pix = camera.pixelHeight;
        newYPosition = ((pix - height)*scaleheight);
        topLeft.anchoredPosition = new Vector2(topLeft.anchoredPosition.x, newYPosition);
        bossLives.anchoredPosition = new Vector2(bossLives.anchoredPosition.x, newYPosition);
    }

    void Update()
    {
        switch (mode)
        {
            case camera_state.ZOOM_IN:
                zoomTime += Time.deltaTime;
                camera.fieldOfView = Mathf.Lerp(minZoom, maxZoom, zoomTime / maxZoomTime);

                if (zoomTime > maxZoomTime)
                {
                    playerCon.allowMovement = true;
                    mode = camera_state.WAITING;
                    zoomTime = 0.0f;
                }
                break;

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
                camera.transform.position = new Vector3(Mathf.Lerp(initialPosition.x, archerPosition.x, zoomTime / maxZoomTime),
                    Mathf.Lerp(initialPosition.y, archerPosition.y - 5, zoomTime / maxZoomTime), camera.transform.position.z);

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
                camera.transform.position = new Vector3(Mathf.Lerp(archerPosition.x, initialPosition.x, zoomTime / maxZoomTime),
                    Mathf.Lerp(archerPosition.y - 5, initialPosition.y, zoomTime / maxZoomTime), camera.transform.position.z);

                if (zoomTime > maxZoomTime)
                {
                    mode = camera_state.WAITING;
                    zoomTime = 0.0f;
                    player.isInZoom = false;
                }
                break;
        }
    }

    public void ZoomIn()
    {
        startPoint.localPosition = new Vector3(-8.8f, startPoint.localPosition.y, startPoint.localPosition.z);
        mode = camera_state.ZOOM_IN;
        colliders.SetActive(true);
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

    public void ZoomArcherOut()
    {
        playerCollider.isTrigger = false;
        playerRigid.isKinematic = false;
        mode = camera_state.ZOOM_OUT_ARCHER;   
    }
}