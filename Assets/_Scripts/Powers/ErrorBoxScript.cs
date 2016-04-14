using UnityEngine;
using System.Collections;
using InControl;

public class ErrorBoxScript : MonoBehaviour
{

    public float timeActive = 5.0f;
    public float timeFlickering = 1.0f;
    public int framesBeforeChangeStateWhenFlickering = 6;
    public Player playerScript;

    public GameObject boxUIActivated;
    public Canvas gui;

    private RectTransform boxUIActivatedRectTransform;
    private RectTransform guiRectTrans;

    private int framesInCurrentStateWhenFlickering = 0;
    private float timeActivated = 0.0f;
    private BoxCollider boxCollider;
    private SpriteRenderer spriteRenderer;
    private bool activated = false;
    private bool visible = false;

    private float timeBoxDissapeared;

    public Camera cam;
    CameraGlitchedToBoxes cameraGlitchedToBoxes;

    public float timeCooldownBox = 2.0f;
    private float timeBoxDeactivated = 0.0f;
    private bool onCooldown = false;

    void Start()
    {
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        boxCollider = transform.GetComponent<BoxCollider>();
        boxCollider.enabled = false;
        Color boxColor = spriteRenderer.color;
        boxColor.a = 0.0f;
        spriteRenderer.color = boxColor;

        boxUIActivatedRectTransform = boxUIActivated.GetComponent<RectTransform>();
        boxUIActivated.SetActive(false);
        guiRectTrans = gui.GetComponent<RectTransform>();

        timeBoxDissapeared = 0.0f;

        cameraGlitchedToBoxes = cam.GetComponent<CameraGlitchedToBoxes>();

    }

    // Update is called once per frame
    void Update()
    {
        if (visible && !activated && InputManager.ActiveDevice.Action4.WasPressed && !onCooldown)
        {
            playerScript.DecreaseActivableBox();
            cameraGlitchedToBoxes.RemoveBox(transform.position);
            Color boxColor = spriteRenderer.color;
            boxColor.a = 1.0f;
            spriteRenderer.color = boxColor;
            boxCollider.enabled = true;
            timeActivated = 0.0f;
            activated = true;
            framesInCurrentStateWhenFlickering = 0;

        }
        else if (activated && timeActivated < timeActive && timeActivated >= (timeActive - timeFlickering))
        {
            timeActivated += Time.deltaTime;

            Color boxColor = spriteRenderer.color;
            if (framesInCurrentStateWhenFlickering >= framesBeforeChangeStateWhenFlickering && boxColor.a == 1.0f)
            {
                framesInCurrentStateWhenFlickering = 0;
                boxColor.a = 0.6f;
            }
            else if (framesInCurrentStateWhenFlickering >= framesBeforeChangeStateWhenFlickering / 2 && boxColor.a != 1.0f)
            {
                framesInCurrentStateWhenFlickering = 0;
                boxColor.a = 1.0f;
            }
            spriteRenderer.color = boxColor;
            ++framesInCurrentStateWhenFlickering;
        }
        else if (activated && timeActivated < timeActive)
        {
            timeActivated += Time.deltaTime;
        }
        else if (activated && visible)
        {
            Color boxColor = spriteRenderer.color;
            boxCollider.enabled = false;
            boxColor.a = 0.0f;
            spriteRenderer.color = boxColor;
            activated = false;
            onCooldown = true;
            timeBoxDeactivated = Time.time;
        }
        else if (onCooldown && (Time.time - timeBoxDeactivated > 2.0f))
        {
            if (visible)
            {
                playerScript.IncreaseActivableBox();
                cameraGlitchedToBoxes.AddBox(transform.position);
            }
            onCooldown = false;
        }
        else if (activated)
        {
            Color boxColor = spriteRenderer.color;
            boxCollider.enabled = false;
            boxColor.a = 0.0f;
            spriteRenderer.color = boxColor;
            activated = false;
            onCooldown = true;
            timeBoxDeactivated = Time.time;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 boxUIPosition = new Vector3(transform.position.x, transform.position.y + 4.0f, 0);
            Vector3 camPosition = Camera.main.WorldToScreenPoint(boxUIPosition);
            camPosition.x *= guiRectTrans.rect.width / Camera.main.pixelWidth;
            camPosition.y *= guiRectTrans.rect.height / Camera.main.pixelHeight;

            if (!activated && !onCooldown)
            {
                Color boxColor = spriteRenderer.color;
                spriteRenderer.color = boxColor;
                playerScript.IncreaseActivableBox();
                cameraGlitchedToBoxes.AddBox(transform.position);
            }
            visible = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!activated && !onCooldown)
            {
                playerScript.DecreaseActivableBox();
                cameraGlitchedToBoxes.RemoveBox(transform.position);
                boxCollider.enabled = false;
                Color boxColor = spriteRenderer.color;
                boxColor.a = 0.0f;
                spriteRenderer.color = boxColor;
            }
            visible = false;
        }
    }
}
