using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance = null;

    //General values:
    private int base_points;

    private float total_time;
    private float time_spent;

    private int remaining_lives;
    private int times_retry;

    private int colectionables_taken;

    private int num_enemies;
    private int enemies_defeated;

    //Extra:
    private bool checkpoint_used;
    private bool attack_evaded;

    private float time_last_enemy_kill;
    private bool combo_done;

    private List<Vector3> death_positions;
    private bool jinxed;

    private float time_SFPS_used;

    //Final
    private float finalPoints;

    void Awake()
    {
        death_positions = new List<Vector3>();

        //Check if there is already an instance of ScoreManager
        if (instance == null){
            //if not, set it to this.
            instance = this;
            DontDestroyOnLoad(gameObject);
            RestartValues();
        }
        //If instance already exists:
        else if (instance != this)
        {
            //Destroy this, this enforces our singleton pattern so there can only be one instance
            Destroy(gameObject);
        }

    }

    public void RestartValues()
    {
        print("ressetting");

        times_retry = 0;
        colectionables_taken = 0;
        num_enemies = 0;
        enemies_defeated = 0;

        //Extra:
        checkpoint_used = false;
        attack_evaded = false;

        time_last_enemy_kill = 0;
        combo_done = false;

        death_positions.Clear();
        jinxed = false;

        time_SFPS_used = 0;
    }

    public void SetBasePoints(int points)
    {
        base_points = points;
    }

    public void SetTimes(float total, float spent)
    {
        total_time = total;
        time_spent = spent;
    }

    public void SetRemaniningLives(int remaining)
    {
        remaining_lives = remaining;
    }

    public void RetryDone()
    {
        times_retry++;
    }

    public void AddCollectionable()
    {
        colectionables_taken++;
    }

    public void EnemyAdded()
    {
        num_enemies++;
    }

    public void EnemyDefeated()
    {
        enemies_defeated++;
        
        if(!combo_done && Time.time - time_last_enemy_kill <= 5.0f)
        {
            combo_done = true;
        }
        
        time_last_enemy_kill = Time.time;
    }

    public void CheckpointUsed()
    {
        if (!checkpoint_used) checkpoint_used = true;
    }

    //TODO: Pendiente!
    public void AttackEvaded()
    {
        if (!attack_evaded) attack_evaded = true;
    }

    public void PlayerKilled(Vector3 deathPosition)
    {
        if (!jinxed)
        {
            for (int i = 0; i < death_positions.Count; i++)
            {
                print(Vector3.Distance(death_positions[i], deathPosition));
                if (Vector3.Distance(death_positions[i], deathPosition) <= 15.0f)
                {
                    jinxed = true;
                    break;
                }
            }
        }

        death_positions.Add(deathPosition);

    }

    public void SFPSUsed(float timeUsed)
    {
        time_SFPS_used += timeUsed;
    }

    public string calculatePoints()
    {
        string text;

        float points = base_points * 100;
        text = "Puntos base: " + base_points + " puntos x 100 = " + points;
        
        float percentTimeLeft = (total_time - time_spent) / total_time; //0->1
        float timeExtra = roundToTwo(1.0f + (percentTimeLeft * 9.0f));
        points = points * timeExtra;
        text += "\nTiempo: " + (percentTimeLeft * 100) + "% restante. x" + timeExtra + " = " + points;

        float livesLeftPoints = roundToTwo((remaining_lives * 0.25f) + 0.75f);
        points = points * livesLeftPoints;
        text += "\nVidas: " + remaining_lives + " de 3. x" + livesLeftPoints + " = " + points;

        float colectPoints;
        switch (colectionables_taken)
        {
            case 1: colectPoints = 1.5f; break;
            case 2: colectPoints = 2.5f; break;
            case 3: colectPoints = 4f; break;
            default: colectPoints = 1; break;
        }
        points = points * colectPoints;
        text += "\nColeccionables: " + colectionables_taken + " de 3. x" + colectPoints + " = " + points;

        print(enemies_defeated + " / " + num_enemies);

        //PACIFIST
        if (enemies_defeated == 0)
        {
            points = points * 3.0f;
            text += "\nPacifista. x3 = " + points;
        }

        //GENOCIDE
        if (enemies_defeated == num_enemies)
        {
            points = points * 4.0f;
            text += "\nGenocida. x4 = " + points;
        }

        //PERMADEATH
        if (!checkpoint_used)
        {
            points = points * 10.0f;
            text += "\nPermadeath. x10 = " + points;
        }

        //GODMODE
        if(death_positions.Count == 0)
        {
            points = points * 15.0f;
            text += "\nGodMode. x15 = " + points;
        }

        //COMBO
        if (combo_done)
        {
            points = points * 1.2f;
            text += "\nJinxed. x1.2 = " + points;
        }

        //JINXED
        if (jinxed)
        {
            points = points * 1.25f;
            text += "\nJinxed. x1.25 = " + points;
        }

        //LAAGGG
        if (time_SFPS_used >= 20f)
        {
            points = points * 1.25f;
            text += "\nLAAGGGGGG. x1.25 = " + points;
        }

        if (times_retry > 0)
        {
            points = points / (2 * times_retry);
            text += "\nRetry penalty: / " + (2 * times_retry) + " = " + points;
        }
        


        text += "\nFINAL POINTS: " + points;
        finalPoints = points;
        return text;
    }

    public float getFinalPoints()
    {
        return finalPoints;
    }

    private float roundToTwo(float num)
    {
        return Mathf.Floor(num * 100) / 100;
    }

}
