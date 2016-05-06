using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FakeFPS : MonoBehaviour {

	public Text text;
	public ParticleSystem particles;

	private bool slow = false;
	private float nextChange = 0;

	private int corruptedCharacter = -1;
	private char corruptedValueOriginal;

	// Update is called once per frame
	void Update () {
		if (Time.time > nextChange) {
			if (!slow) {
				
				text.text = Random.Range (58, 61).ToString () + " FPS";

			} else {

				bool update = false;

				System.Text.StringBuilder corruptedText = new System.Text.StringBuilder(text.text);

				if (corruptedCharacter != -1) {
					corruptedText[corruptedCharacter] = corruptedValueOriginal;
					corruptedCharacter = -1;
					update = true;
				}

				if (Random.value > 0.3) {
					corruptedCharacter = Random.Range(0, text.text.Length);
					corruptedValueOriginal = corruptedText[corruptedCharacter];
					corruptedText[corruptedCharacter] = '#';
					update = true;
				}

				if(update)
					text.text = corruptedText.ToString();
			}

			nextChange = Time.time + Random.Range (0.3f, 1f);
		}
	}

	public void SlowActive(float slowdown){
		text.color = new Color (1, 0, 0);
		text.text = (60 * slowdown) + " FPS";
		corruptedCharacter = -1;
		particles.Play();
		slow = true;
	}

	public void SlowInactive(){
		text.color = new Color (1, 1, 0);
		text.text = "60 FPS";
		particles.Stop();
		slow = false;
	}
}
