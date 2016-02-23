using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUILifes : MonoBehaviour {
    public PlayerController player;
    public Image[] lifes;
    private int life = 3;

	// Function to increment lifes in the GUI.
	// Do a comparison from lifes before and now and change the 
	// correspondent sprites. Is possible increment more than 1 life.
    public void IncrementLifes()
    {
        int aux = player.lifes - life;
        for (int i = lifes.Length - 1; i > lifes.Length -1 - aux; --i)
        {
            lifes[i - life].sprite = Resources.Load<Sprite>("Sprites/life");
        }
        life += aux;
    }

	// Function to decrement lifes in the GUI.
	// Do a check for know which life has been lost
	// and change the sprite of this life to lost life sprite.
    public void DecrementLifes()
    {
        int aux = 3 - player.lifes - 1;
        lifes[aux].sprite = Resources.Load<Sprite>("Sprites/lostLife");
        --life;
    }
}