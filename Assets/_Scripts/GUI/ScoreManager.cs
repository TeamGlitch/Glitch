using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance = null;

    //General values:
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

}
