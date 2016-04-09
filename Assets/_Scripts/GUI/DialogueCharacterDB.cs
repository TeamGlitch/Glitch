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
	Dictionary<string, CharacterEntry> loadedCharacters = new Dictionary<string, CharacterEntry>();

	//Function to load a character data
	public CharacterEntry loadCharacter(string name){

		CharacterEntry character = null;

		//If the requested character is in the dictionary, store it in "character" variable
		//else...
		if(!loadedCharacters.TryGetValue(name, out character))
		{

			//Create a new character
			character = new CharacterEntry();

			//Load the data of that character
			if (name == "Glitch") {
				character.faceAnimation = Resources.LoadAll<Sprite> ("Sprites/Faces/glitch-face").ToList();
				character.pitch = 0.92f;
			}
			else if (name == "Bug") {
				character.faceAnimation = Resources.LoadAll<Sprite> ("Sprites/Faces/bug-face").ToList ();
				character.pitch = 1.6f;
			}

			//Store the data in the dictionary for fast future uses
			loadedCharacters.Add(name, character);
			
		}

		//return the character info
		return character;
			
	}

}
