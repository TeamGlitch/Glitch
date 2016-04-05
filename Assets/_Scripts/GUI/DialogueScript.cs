using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class DialogueScript : MonoBehaviour {

	public enum dialogueBoxState
	{
		PREPARE_TEXT,
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
	private dialogueBoxState state = dialogueBoxState.PREPARE_TEXT;

	//Dialogue Box
	public Text dialogueBoxText;				//The dialogue box text reference
	public GameObject continueButton;			//The continue button reference

	//Writting state
	public float waitBetweenLetters = 0.03f;	//How much time there is between letter and letter printing

	private List<string> messageList = new List<string>();  //Messages to print in the dialogue box  
	private string messageToPrint;							//Message being printed actually
	private string cleanMessage;                            //Message without tags

	private int letterIndex = 0;                //Index of the currently printing letter
	private int cleanIndex = 0;                    //Index of the currently printing letter in the clean message
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
		continueButton.SetActive(false);
		messageList.Add("Pack my box with five dozen liquor jugs. <speed=0.09>The five boxing wizards jump quickly. <speed=0.03>How vexingly quick daft zebras jump!");
		messageList.Add("The five boxing wizards jump quickly! I just said quickly! QUICKLYYYYYY!!!");
	}
	
	// Update is called once per frame
	void Update () {

		switch (state) {

			case dialogueBoxState.PREPARE_TEXT: //Preparing the message to write

				//Sets the message to print to the first message in the list
				messageToPrint = messageList[0];

				//Cleans the tags and stores it as the clean message
				int i = 0;
				cleanMessage = "";
				while (i < messageToPrint.Length) {
					if (messageToPrint[i] == '<') {
						while (i < messageToPrint.Length && messageToPrint[i] != '>') {
							i++;
						}
						if (messageToPrint[i] == '>') {
							i++;
						}
					} else {
						cleanMessage += messageToPrint[i];
						i++;
					}
				}

				//Starts writting
				state = dialogueBoxState.WRITTING;
				letterIndex = 0;
				cleanIndex = 0;

			break;

			case dialogueBoxState.WRITTING: //Currently writting in the dialogue box

				//If A is pressed, skip message
				if (InputManager.ActiveDevice.Action1.WasPressed) {
					letterIndex  = messageToPrint.Length;
					nextLetterTime = 0;
				}
				
				//If we can print the next letter
				if (Time.time > nextLetterTime) {

					//Go to the next letter
					letterIndex++;
					cleanIndex++;

					//If the end has been reached
					if (letterIndex >= messageToPrint.Length) {

						//Put the message directly
						dialogueBoxText.text = cleanMessage;

						//Go to wait, remove the message from the list and activate the continue button
						state = dialogueBoxState.WAITING;
						messageList.RemoveAt(0);
						continueButton.SetActive(true);

					} else {

						//If not, read all the tags in this position (if there's any)
						while (letterIndex < messageToPrint.Length && messageToPrint[letterIndex] == '<') {
							ReadTag();
						}

						//If not, we send the message to print with color tags that make it invisible from the print
						//letter to the end. We do this rather than doing subscripts because they mess the paragraph aligment
						dialogueBoxText.text = cleanMessage.Insert(cleanIndex, randomLetters [currentRandomLetter] + "<color=#00000000>") + "</color>";

						//Finally we determine when the next letter will appear
						nextLetterTime = Time.time + waitBetweenLetters;

					}

				} else if (Time.time > nextBuggedLetterTime) {

					//We change the writting pointer's currently showed sign and we determine the next sign change
					currentRandomLetter = Random.Range (0, randomLetters.Length);
					dialogueBoxText.text = cleanMessage.Insert(cleanIndex, randomLetters [currentRandomLetter] + "<color=#00000000>") + "</color>";
					nextBuggedLetterTime = Time.time + waitBetweenBuggedLetters;
				}
					
			break;

			case dialogueBoxState.WAITING: //Message on dialogue box. Waiting for next instruction

				//If there is no corruption on the message
				if (corruptedPosition == -1) {
				
					//0.75% chance to corrupt a char
					if(Random.value > 0.9925f){

						//Selects a position, stores it's value and sets a duration
						corruptedPosition = Random.Range(0, dialogueBoxText.text.Length);
						originalChar = dialogueBoxText.text[corruptedPosition];
						timeToSolve = Time.time + Random.Range(0.05f, 0.4f);

						//Changes the position to a random symbol
						string newText = dialogueBoxText.text.Remove(corruptedPosition, 1);
						newText = newText.Insert(corruptedPosition, randomLetters[Random.Range(0, randomLetters.Length)]);
						dialogueBoxText.text = newText;
					}
					
				}
				//If there is corruption and its duration has ended
				else if (Time.time > timeToSolve){

					//Fixes the corrupted char
					string newText = dialogueBoxText.text.Remove(corruptedPosition, 1);
					newText = newText.Insert(corruptedPosition, originalChar.ToString());
					dialogueBoxText.text = newText;
					corruptedPosition = -1;

				}

			break;

		}
	}

	void ReadTag(){

		string tag = "";		//The tag name
		string value = "";		//The tag value
		bool tagging = true;	//Where writting the tag? Else, we're writting the value
		letterIndex++;

		//Do until the '>' or the message end
		while (letterIndex < messageToPrint.Length && messageToPrint[letterIndex] != '>') {

			//If it's the =, we're now writting the value
			if (messageToPrint[letterIndex] == '=') {
				tagging = false;
			} else {
				
				//Store the value where needed
				if (tagging) {
					tag += messageToPrint[letterIndex];
				} else {
					value += messageToPrint[letterIndex];
				}
			}
			letterIndex++;
		}

		//If it ended with '>' (not unclosed tag), search in the effects
		if (messageToPrint[letterIndex] == '>') {

			//Text speed
			if (tag == "speed") {
				waitBetweenLetters = float.Parse(value);
			}

		}
	}
}
