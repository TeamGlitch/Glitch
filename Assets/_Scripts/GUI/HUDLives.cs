using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDLives : MonoBehaviour {
    public PlayerController player;
    public Image[] livesArray;

    private Sprite life;
    private Sprite noLife;
    private int lives = 3;

    void Start()
    {
        life = Resources.Load<Sprite>("Sprites/life");
        noLife = Resources.Load<Sprite>("Sprites/lostLife");
    }

	// Function to increment lives in the GUI.
	// Do a comparison from lives before and now and change the 
	// correspondent sprites. Is possible increment more than 1 life.
    public void IncrementLives()
    {
        int increment = player.lives - lives;
		for (int i = livesArray.Length - 1; i > livesArray.Length -1 - increment; --i)
        {
            livesArray[i - lives].sprite = life;
        }
		lives += increment;
    }

	// Function to decrement lives in the GUI.
	// Do a check for know which life has been lost
	// and change the sprite of this life to lost life sprite.
    public void DecrementLives()
    {
        int aux = 3 - player.lives - 1;
        livesArray[aux].sprite = noLife;
        --lives;
    }
}