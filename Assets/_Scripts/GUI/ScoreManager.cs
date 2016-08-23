using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour {

    public enum pointsCalculationPhases
    {
        NONE,
        BASE,
        TIME,
        LIFE,
        COLLECTIBLES,
        PACIFIST,
        GENOCIDE,
        PERMADEATH,
        GODMODE,
        COMBO,
        JINXED,
        LAG,
        PENALTY
    };

    public static ScoreManager instance = null;
    public pointsCalculationPhases phase = pointsCalculationPhases.NONE;

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

    //Calculation
    private float points;
    private float finalPoints;
    private string outputText;

    void Awake()
    {
        death_positions = new List<Vector3>();

        //Check if there is already an instance of ScoreManager
        if (instance == null){
            //if not, set it to this.
            instance = this;
            DontDestroyOnLoad(gameObject);
            RestartValues();
            testValues();
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

    public float calculatePoints()
    {
        float multiplier = 0;

        switch (phase)
        {
            case pointsCalculationPhases.NONE:
                points = base_points * 10;
                outputText = "Puntos base: " + base_points + " puntos x 10 = " + points;
                
                phase = pointsCalculationPhases.BASE;
                multiplier = 10;
                break;

            case pointsCalculationPhases.BASE:
                float percentTimeLeft = (total_time - time_spent) / total_time; //0->1
                float timeExtra = roundToTwo(1.0f + (percentTimeLeft * 9.0f));
                points = points * timeExtra;
                outputText += "\nTiempo: " + (percentTimeLeft * 100) + "% restante. x" + timeExtra + " = " + points;

                phase = pointsCalculationPhases.TIME;
                multiplier = timeExtra;
                break;

            case pointsCalculationPhases.TIME:
                float livesLeftPoints = roundToTwo((remaining_lives * 0.25f) + 0.75f);
                points = points * livesLeftPoints;
                outputText += "\nVidas: " + remaining_lives + " de 3. x" + livesLeftPoints + " = " + points;
               
                phase = pointsCalculationPhases.LIFE;
                multiplier = livesLeftPoints;
                break;

            case pointsCalculationPhases.LIFE:
                float colectPoints;
                switch (colectionables_taken)
                {
                    case 1: colectPoints = 1.5f; break;
                    case 2: colectPoints = 2.5f; break;
                    case 3: colectPoints = 4f; break;
                    default: colectPoints = 1; break;
                }
                points = points * colectPoints;
                outputText += "\nColeccionables: " + colectionables_taken + " de 3. x" + colectPoints + " = " + points;
                
                phase = pointsCalculationPhases.COLLECTIBLES;
                multiplier = colectPoints;
                break;

            case pointsCalculationPhases.COLLECTIBLES:
                //PACIFIST
                if (enemies_defeated == 0)
                {
                    points = points * 3.0f;
                    outputText += "\nPacifista. x3 = " + points;
                    multiplier = 3.0f;
                }
                phase = pointsCalculationPhases.PACIFIST;
                break;

            case pointsCalculationPhases.PACIFIST:
                //GENOCIDE
                if (enemies_defeated == num_enemies)
                {
                    points = points * 4.0f;
                    outputText += "\nGenocida. x4 = " + points;
                    multiplier = 4.0f;
                }
                phase = pointsCalculationPhases.GENOCIDE;
                break;

            case pointsCalculationPhases.GENOCIDE:
                //PERMADEATH
                if (!checkpoint_used)
                {
                    points = points * 10.0f;
                    outputText += "\nPermadeath. x10 = " + points;
                    multiplier = 10.0f;
                }
                phase = pointsCalculationPhases.PERMADEATH;
                break;

            case pointsCalculationPhases.PERMADEATH:
                //GODMODE
                if (death_positions.Count == 0)
                {
                    points = points * 15.0f;
                    outputText += "\nGodMode. x15 = " + points;
                    multiplier = 15.0f;
                }
                phase = pointsCalculationPhases.GODMODE;
                break;

            case pointsCalculationPhases.GODMODE:
                //COMBO
                if (combo_done)
                {
                    points = points * 1.25f;
                    outputText += "\nCombo. x1.2 = " + points;
                    multiplier = 1.25f;
                }
                phase = pointsCalculationPhases.COMBO;
                break;

            case pointsCalculationPhases.COMBO:
                //JINXED
                if (jinxed)
                {
                    points = points * 1.12f;
                    outputText += "\nJinxed. x1.12 = " + points;
                    multiplier = 1.12f;
                }
                phase = pointsCalculationPhases.JINXED;
                break;

            case pointsCalculationPhases.JINXED:
                //LAAGGG
                if (time_SFPS_used >= 20f)
                {
                    points = points * 1.15f;
                    outputText += "\nLAAGGGGGG. x1.15 = " + points;
                    multiplier = 1.15f;
                }
                phase = pointsCalculationPhases.LAG;
                break;

            case pointsCalculationPhases.LAG:
                if (times_retry > 0)
                {
                    points = points / (2 * times_retry);
                    outputText += "\nRetry penalty: / " + (2 * times_retry) + " = " + points;
                    multiplier = (2 * times_retry);
                }
                phase = pointsCalculationPhases.PENALTY;
                break;

            case pointsCalculationPhases.PENALTY:
                outputText += "\nFINAL POINTS: " + points;
                finalPoints = points;
                phase = pointsCalculationPhases.NONE;
                break;

            default:
                break;
        }

        return multiplier;
    }

    public float getFinalPoints()
    {
        return finalPoints;
    }

    private float roundToTwo(float num)
    {
        return Mathf.Floor(num * 100) / 100;
    }

    private void testValues()
    {
        base_points = 80;
        total_time = 600;
        time_spent = 435;
        remaining_lives = 3;
        times_retry = 1;
        colectionables_taken = 3;
        num_enemies = 7;
        enemies_defeated = 7;
        checkpoint_used = false;
        time_last_enemy_kill = 200;
        combo_done = true;
        jinxed = true;
        time_SFPS_used = 2000;
    }

    public float getPoints() { return points; }
    public int getBasePoints() { return base_points; }
    public float getTotalTime() { return total_time; }
    public float getTimeSpent() { return time_spent; }
    public int getRemainingLives() { return remaining_lives; }
    public int getTimesRetry() { return times_retry; }
    public int getColectionablesTaken() { return colectionables_taken; }

}
