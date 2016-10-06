using UnityEngine;
using System.Collections;

public class BossStageCanvasController : MonoBehaviour {

    private Transform bossLives;
    private Transform glitchHer;

    public BossArcherIA bossArcher;
    public GlitchArcher glitchArcher;

	// Use this for initialization
	void Start () {
        bossLives = transform.FindChild("BossLives");
        glitchHer = transform.FindChild("GlitchHer");

        bossArcher.BossDeadEvent += StartAnimation;
        glitchArcher.BossGlitchedEvent += EndAnimation;
    }

    public void StartAnimation()
    {
        glitchHer.gameObject.SetActive(true);
        bossLives.gameObject.SetActive(false);
    }

    public void EndAnimation()
    {
        glitchHer.gameObject.SetActive(false);
    }

}
