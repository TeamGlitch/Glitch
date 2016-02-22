using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUILifes : MonoBehaviour {
    public PlayerController player;
    public Image[] lifes;
    private int life = 3;

    public void IncrementLifes()
    {
        int aux = player.lifes - life;
        for (int i = lifes.Length - 1; i > lifes.Length -1 - aux; --i)
        {
            lifes[i - life].sprite = Resources.Load<Sprite>("Sprites/life");
        }
        life += aux;
    }

    public void DecrementLifes()
    {
        int aux = 3 - player.lifes - 1;
        lifes[aux].sprite = Resources.Load<Sprite>("Sprites/lostLife");
        --life;
    }
}