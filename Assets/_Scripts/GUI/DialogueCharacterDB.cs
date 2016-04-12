using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//Class that includes all the character information
public class CharacterEntry {
	public List<Sprite> faceAnimation;		//Animation sprites
	public float pitch;						//Sound pitch
}

public class DialogueCharacterDB {

	//Dictionary that stores previously used characters
	Dictionary<string, CharacterEntry> loadedCharacters;

	public DialogueCharacterDB(){

		//Initializes the dictionary
		loadedCharacters = new Dictionary<string, CharacterEntry>();

		//Adds the "none" entry
		loadedCharacters.Add ("none", 
			new CharacterEntry {
				faceAnimation = new List<Sprite>(),
				pitch = 1.0f
			}
		);
	}

	//Function to load a character data
	public CharacterEntry loadCharacter(string name){

		CharacterEntry character = null;
		name = name.ToLower();

		//If the requested character is in the dictionary, store it in "character" variable
		//else...
		if(!loadedCharacters.TryGetValue(name, out character))
		{

			//Create a new character
			character = new CharacterEntry();

			//Load the data of that character
			if (name == "glitch") {
				character.faceAnimation = Resources.LoadAll<Sprite> ("Sprites/Faces/glitch-face").ToList ();
				character.pitch = 0.92f;
			}
			else if (name == "bug") {
				character.faceAnimation = Resources.LoadAll<Sprite> ("Sprites/Faces/bug-face").ToList ();
				character.pitch = 1.6f;
			} else {
				
				//If it's not in the dictionary and it doesn't exist, simply return "none"
				loadedCharacters.TryGetValue("none", out character);
				return character;

			}

			//If it does exist, store the data in the dictionary for fast future uses
			loadedCharacters.Add(name, character);
			
		}

		//return the character info
		return character;
			
	}

}
