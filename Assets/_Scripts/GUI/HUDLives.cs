using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDLives : MonoBehaviour {
	
    public Player player;
    public Image[] livesArray;

    private Sprite life;
    private Sprite noLife;
    private int activeNodes;

    void Awake()
    {
        life = Resources.Load<Sprite>("Sprites/life");
        noLife = Resources.Load<Sprite>("Sprites/lostLife");
		activeNodes = player.lives;
    }

	// Function to update the UI life representation according to
	// the current player lives
    public void UpdateLifeUI()
    {
		int diference = player.lives - activeNodes;

		if (diference > 0) {
			for (int i = activeNodes; i < player.lives; i++) {
				livesArray[i].sprite = life;
			}
		} else if (diference < 0) {
			for (int i = activeNodes + diference; i < activeNodes; i++) {
				livesArray [i].sprite = noLife;
			}
		}

		activeNodes = player.lives;
    }
}