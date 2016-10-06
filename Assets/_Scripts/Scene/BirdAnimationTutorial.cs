using UnityEngine;
using System.Collections;

public class BirdAnimationTutorial : MonoBehaviour {

    public DialoguePoint startDialogue;
    public DialogueScript dialogueScript;
    public PlayerController player;
    public SpikeTrapScript spikes;
    public AudioClip birdkill;
    public AudioClip birdScream;
    public DialoguePoint endDialogue;

    private bool playing = false;

    void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        endDialogue.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!playing && startDialogue.used && dialogueScript.state == DialogueScript.dialogueBoxState.OFF)
        {
            player.allowMovement = false;
            transform.GetChild(0).gameObject.SetActive(true);
            GetComponent<Animation>().Play();
            playing = true;
        }
    }

    public void HideWings()
    {
        transform.GetChild(0).GetComponent<Animator>().SetTrigger("Recogerse");
    }

    public void ActivateTrap()
    {
        spikes.startJump();
    }

    public void Feathers()
    {
        transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        SoundManager.instance.PlaySingle(birdkill);
        SoundManager.instance.PlaySingle(birdScream);
    }

    public void Finish()
    {
        endDialogue.gameObject.SetActive(true);
        GameObject.Destroy(this.gameObject);
    }
}
