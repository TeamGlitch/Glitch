using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDLives : MonoBehaviour {

    public Player player;
    public Image[] lives;
    public Sprite life;
    public Sprite noLife;

    private int maxLives = 3;


    // Function to update lives in HUD
    // If player lives is minus than i then is a lost life
    public void UpdateLives()
    {
        for (int i = maxLives - 1; i >= 0; --i)
        {
            if (i <= (player.lives - 1))
            {
                lives[i].sprite = life;
            }
            else
            {
                lives[i].sprite = noLife;
            }
        }
    }
}