using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIlives : MonoBehaviour {
    public PlayerController player;
    public Image[] lives;
    private int life = 3;

	// Function to increment lives in the GUI.
	// Do a comparison from lives before and now and change the 
	// correspondent sprites. Is possible increment more than 1 life.
    public void IncrementLives()
    {
        int increment = player.lives - life;
		for (int i = lives.Length - 1; i > lives.Length -1 - increment; --i)
        {
            lives[i - life].sprite = Resources.Load<Sprite>("Sprites/life");
        }
		life += increment;
    }

	// Function to decrement lives in the GUI.
	// Do a check for know which life has been lost
	// and change the sprite of this life to lost life sprite.
    public void Decrementlives()
    {
        int aux = 3 - player.lives - 1;
        lives[aux].sprite = Resources.Load<Sprite>("Sprites/lostLife");
        --life;
    }
}