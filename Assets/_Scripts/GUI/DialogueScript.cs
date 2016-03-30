using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueScript : MonoBehaviour {

	public enum dialogueBoxState
	{
		WRITTING,
		WAITING
	};

	private string[] randomLetters = new string[] {
		"•","|","£","Ø","ƒ","º","¿","®","¡","»","µ","±",
		"¶","÷","ª","ø","†","‡","‘","—","™","œ","©","€",
		"¥","~","!","$","%","&","/","(",")","=","?","@",
		"#","~","€","☺","☻","♥","♦","♣","♠","▲","✓","☀",
		"★","☂","♞","☯","☭","☢","☎","❄"
	};

	//State
	private dialogueBoxState state = dialogueBoxState.WRITTING;

	//Dialogue Box
	public Text dialogueBoxText;				//The dialogue box text reference

	//Writting state
	public float waitBetweenLetters = 0.05f;	//How much time there is between letter and letter printing

	private string messageToPrint;				//Message to print in the dialogue box
	private int printLetter = 0;				//Index of the currently printing letter
	private float nextLetterTime = 0f;			//Time when the next letter will be printed

	private int currentRandomLetter = 0;				//Current bugged letter in the writting effect
	private float waitBetweenBuggedLetters = 0.02f;		//Wait between bugged letter changes
	private float nextBuggedLetterTime = 0f;			//When the next bugged letter will appear

	//Waiting state
	private int corruptedPosition = -1;			//What index letter is corrupted
	private char originalChar;					//What original character was in it
	private float timeToSolve = 0;				//When the corruption will be solved


	// Use this for initialization
	void Start () {
		messageToPrint = "Pack my box with five dozen liquor jugs. The five boxing wizards jump quickly. How vexingly quick daft zebras jump!";
	}
	
	// Update is called once per frame
	void Update () {

		switch (state) {

			case dialogueBoxState.WRITTING: //Currently writting in the dialogue box

					//If we can print the next letter
				if (Time.time > nextLetterTime) {

					//Go to the next letter
					printLetter++;

					if (printLetter >= messageToPrint.Length) {

						//If the end has been reached, put the message directly and restore variables
						dialogueBoxText.text = messageToPrint;
						state = dialogueBoxState.WAITING;
						printLetter = 0;

					} else {

						//If not, we send the message to print with color tags that make it invisible from the print
						//letter to the end. We do this rather than doing subscripts because they mess the paragraph aligment
						dialogueBoxText.text = messageToPrint.Insert (printLetter, randomLetters [currentRandomLetter] + "<color=#00000000>") + "</color>";

						//Finally we determine when the next letter will appear
						nextLetterTime = Time.time + waitBetweenLetters;

					}

				} else if (Time.time > nextBuggedLetterTime) {

					//We change the currently showed sign and we determine the next sign change
					currentRandomLetter = Random.Range (0, randomLetters.Length);
					dialogueBoxText.text = messageToPrint.Insert (printLetter, randomLetters [currentRandomLetter] + "<color=#00000000>") + "</color>";
					nextBuggedLetterTime = Time.time + waitBetweenBuggedLetters;
				}
					
			break;

			case dialogueBoxState.WAITING: //Message on dialogue box. Waiting for next instruction
			
				if (corruptedPosition == -1) {
					if(Random.value > 0.9925f){
						corruptedPosition = Random.Range(0, dialogueBoxText.text.Length);
						originalChar = dialogueBoxText.text[corruptedPosition];
						timeToSolve = Time.time + Random.Range(0.05f, 0.4f);

						string newText = dialogueBoxText.text.Remove(corruptedPosition, 1);
						newText = newText.Insert(corruptedPosition, randomLetters[Random.Range(0, randomLetters.Length)]);
						dialogueBoxText.text = newText;
					}
					
				} else if (Time.time > timeToSolve){
					string newText = dialogueBoxText.text.Remove(corruptedPosition, 1);
					newText = newText.Insert(corruptedPosition, originalChar.ToString());
					dialogueBoxText.text = newText;
					corruptedPosition = -1;
				}

			break;

		}


	}
}
