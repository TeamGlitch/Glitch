using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueScript : MonoBehaviour {

	//Dialogue Box Writing
	public Text dialogueBoxText;				//The dialogue box text reference
	public float waitBetweenLetters = 0.05f;	//How much time there is between letter and letter printing

	private bool printing = false;				//Currently printing in the dialogue box
	private string messageToPrint;				//Message to print in the dialogue box
	private int printLetter = 0;				//Index of the currently printing letter
	private float nextLetterTime = 0f;			//Time when the next letter will be printed

	// Use this for initialization
	void Start () {
		messageToPrint = "Pack my box with five dozen liquor jugs. The five boxing wizards jump quickly. How vexingly quick daft zebras jump!";
		printing = true;
	}
	
	// Update is called once per frame
	void Update () {

		//We check if we're printing something and we can print the next letter
		if (printing && Time.time > nextLetterTime) {







		}
	}


}
