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

	private int currentRandomLetter = 0;
	private float waitBetweenBuggedLetters = 0.02f;
	private float nextBuggedLetterTime = 0f;
	private string[] randomLetters = new string[] {
		"•","|","£","Ø","ƒ","º","¿","®","¡","»","µ","±",
		"¶","÷","ª","ø","†","‡","‘","—","™","œ","©","€",
		"¥","~","!","$","%","&","/","(",")","=","?","@",
		"#","~","€","☺","☻","♥","♦","♣","♠","▲","✓","☀",
		"★","☂","♞","☯","☭","☢","☎","❄"
	};

	// Use this for initialization
	void Start () {
		messageToPrint = "Pack my box with five dozen liquor jugs. The five boxing wizards jump quickly. How vexingly quick daft zebras jump!";
		printing = true;
	}
	
	// Update is called once per frame
	void Update () {

		if (printing) {

			//If we can print the next letter
			if (Time.time > nextLetterTime) {

				//Go to the next letter
				printLetter++;

				if (printLetter >= messageToPrint.Length) {

					//If the end has been reached, put the message directly and restore variables
					dialogueBoxText.text = messageToPrint;
					printing = false;
					printLetter = 0;

				} else {

					//If not, we send the message to print with color tags that make it invisible from the print
					//letter to the end. We do this rather than doing subscripts because they mess the paragraph aligment
					dialogueBoxText.text = messageToPrint.Insert (printLetter, randomLetters[currentRandomLetter] + "<color=#00000000>") + "</color>";

					//Finally we determine when the next letter will appear
					nextLetterTime = Time.time + waitBetweenLetters;

				}

			} else {
				
				if (Time.time > nextBuggedLetterTime) {
					
					//We change the currently showed sign and we determine the next sign change
					currentRandomLetter = Random.Range (0, randomLetters.Length);
					dialogueBoxText.text = messageToPrint.Insert (printLetter, randomLetters [currentRandomLetter] + "<color=#00000000>") + "</color>";
					nextBuggedLetterTime = Time.time + waitBetweenBuggedLetters;
				}

			}

		}






	}
}
