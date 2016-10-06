using UnityEngine;
using System.Collections;
using InControl;

public enum error_box_state
{
    NON_REACHED,
    SLEEPING,
    GROWING,
    ACTIVATED,
    FLICKERING,
    DISSAPEARING,
    COOLDOWN
};

public class ErrorBoxScript : MonoBehaviour
{

    public Player playerScript;
    public AudioClip ErrorBoxNoiseSound;
    public AudioClip ErrorBoxActivateSound;
    public CameraGlitchedToBoxes cameraGlitchedToBoxes;

    private error_box_state state = error_box_state.NON_REACHED;

    public float timeToBecomeBig = 0.5f;
    public float timeActive = 5.0f;
    public float timeFlickering = 1.0f;
    public float flickTime = 0.1f;
    public float timeCooldownBox = 2.0f;

    private Transform box;
    private BoxCollider boxCollider;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    private float timeStateChange;
    private float lastFlicker = 0;
    private bool activable = false;

    private AudioSource noiseSoundSource = null;

    void Start()
    {
        box = transform.GetChild(0);
        spriteRenderer = box.GetComponent<SpriteRenderer>();
        boxCollider = box.GetComponent<BoxCollider>();
        boxCollider.enabled = false;
        Color boxColor = spriteRenderer.color;
        boxColor.a = 0.0f;
        spriteRenderer.color = boxColor;
        playerController = playerScript.gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

        switch (state)
        {
            case (error_box_state.NON_REACHED):

                if (activable)
                {
                    playerScript.IncreaseActivableBox();
                    cameraGlitchedToBoxes.AddBox(transform.position);
                    state = error_box_state.SLEEPING;
                }
                break;

            case (error_box_state.SLEEPING):

                if ((!activable && playerController.state != PlayerController.player_state.TELEPORTING) ||
                    activable && playerController.state == PlayerController.player_state.DEATH)
                {
                    activable = false;
                    playerScript.DecreaseActivableBox();
                    cameraGlitchedToBoxes.RemoveBox(transform.position);
                    state = error_box_state.NON_REACHED;
                    noiseSoundSource = null;
                }
                else if (InputManager.ActiveDevice.Action4.WasPressed)
                {
                    playerScript.DecreaseActivableBox();
                    cameraGlitchedToBoxes.RemoveBox(transform.position);

                    Color boxColor = spriteRenderer.color;
                    boxColor.a = 1.0f;
                    spriteRenderer.color = boxColor;

                    boxCollider.enabled = true;
                    timeStateChange = Time.time;

                    box.localScale = new Vector3(0f, 0f, 0f);

                    if (noiseSoundSource != null)
                        noiseSoundSource.Stop();
                    SoundManager.instance.PlaySingle(ErrorBoxActivateSound);
                    state = error_box_state.GROWING;
                }
                else if(noiseSoundSource == null || !noiseSoundSource.isPlaying)
                {
                    noiseSoundSource = SoundManager.instance.PlaySingle(ErrorBoxNoiseSound);
                }
                break;

            case (error_box_state.GROWING):

                float tempTime = Time.time - timeStateChange;
                if (tempTime > timeToBecomeBig)
                {
                    tempTime = timeToBecomeBig;
                    timeStateChange = Time.time;
                    state = error_box_state.ACTIVATED;
                }
                float size = Mathf.Lerp(0.0f, 1f, tempTime / timeToBecomeBig);
                box.localScale = new Vector3(size, size, size);
                break;

            case (error_box_state.ACTIVATED):

                if (Time.time - timeStateChange >= timeActive - timeFlickering)
                {
                    state = error_box_state.FLICKERING;
                }
                break;

            case (error_box_state.FLICKERING):
                if (Time.time - timeStateChange > timeActive)
                {
                    boxCollider.enabled = false;
                    Color boxColor = spriteRenderer.color;
                    boxColor.a = 1.0f;
                    spriteRenderer.color = boxColor;
                    timeStateChange = Time.time;
                    state = error_box_state.DISSAPEARING;
                }
                else
                {

                    if (Time.time - lastFlicker >= flickTime)
                    {
                        Color boxColor = spriteRenderer.color;
                        if (boxColor.a == 1.0f)
                        {
                            boxColor.a = 0.6f;
                        }
                        else
                        {
                            boxColor.a = 1.0f;
                        }
                        spriteRenderer.color = boxColor;

                        lastFlicker = Time.time;
                    }
                }

                break;

            case (error_box_state.DISSAPEARING):

                float tempTimeP = Time.time - timeStateChange;
                if (tempTimeP > timeToBecomeBig)
                {
                    tempTimeP = timeToBecomeBig;
                    timeStateChange = Time.time;
                    Color boxColor = spriteRenderer.color;
                    boxColor.a = 0.0f;
                    spriteRenderer.color = boxColor;
                    state = error_box_state.COOLDOWN;
                }
                float sizeP = Mathf.Lerp(0.1f, 0.0f, tempTimeP / timeToBecomeBig);
                box.localScale = new Vector3(sizeP, sizeP, sizeP);
                break;

            case (error_box_state.COOLDOWN):

                if (Time.time - timeStateChange > timeCooldownBox)
                {
                    state = error_box_state.NON_REACHED;
                    noiseSoundSource = null;
                }
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activable = false;
        }
    }
}