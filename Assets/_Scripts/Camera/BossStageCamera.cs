using UnityEngine;
using System.Collections;

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
    public Camera camera;
    public GameObject colliders;
    public Transform startPoint;
    public Player player;
    public camera_state mode = camera_state.WAITING;

    private float zoomTime = 0;
    private Vector3 initialPosition;
    private Vector3 archerPosition;
    private Rigidbody playerRigid;
    private BoxCollider playerCollider;

    void Start()
    {
        initialPosition = camera.transform.position;
        playerRigid = player.GetComponent<Rigidbody>();
        playerCollider = player.GetComponent<BoxCollider>();

        float xFactor = Screen.width / 1920f;
        float yFactor = Screen.height / 1080f;

        camera.rect = new Rect(0, 0, 1, xFactor / yFactor);
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
                }
                break;

            case camera_state.ZOOM_IN_ARCHER:
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
                zoomTime += Time.deltaTime;
                camera.fieldOfView = Mathf.Lerp(archerZoom, maxZoom, zoomTime / maxZoomTime);
                camera.transform.position = new Vector3(Mathf.Lerp(archerPosition.x, initialPosition.x, zoomTime / maxZoomTime),
                    Mathf.Lerp(archerPosition.y - 5, initialPosition.y, zoomTime / maxZoomTime), camera.transform.position.z);

                if (zoomTime > maxZoomTime)
                {
                    mode = camera_state.WAITING;
                    zoomTime = 0.0f;
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
    }

    public void ZoomArcherIn()
    {
        archerPosition = archer.transform.position;
        playerRigid.isKinematic = true;
        playerCollider.enabled = false;
        mode = camera_state.ZOOM_IN_ARCHER;
    }

    public void ZoomArcherOut()
    {
        playerCollider.enabled = true;
        playerRigid.isKinematic = false;
        mode = camera_state.ZOOM_OUT_ARCHER;   
    }
}