using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallingStonesController : MonoBehaviour {

	public Player player;
	public World world;
	public float timeBetweenStones = 0.15f;

	public SlowFPS slowFpsScript;
	List<FallingStoneScript> stones;
	bool activeStones;
	bool isFPSActive = false;
	float timeStonesActivated;
	float timeSinceLastUpdateWithPowerActivated;
	bool kinematicActivated = false;
	int lastActiveStone = -1;

	// Use this for initialization
	void Start () {
		stones = new List<FallingStoneScript> ();
		for (int i = 0; i < transform.childCount; ++i) {
			stones.Add (transform.GetChild (i).GetComponent<FallingStoneScript> ());
		}
		player.PlayerDeadEvent += ResposStones;
		slowFpsScript.SlowFPSActivated += ActivateFPS;
		slowFpsScript.SlowFPSDeactivated += DeactivateFPS;
	}
	
	// Update is called once per frame
	void Update () {
		if (isFPSActive && !kinematicActivated)
		{
			timeSinceLastUpdateWithPowerActivated = 0.0f;
			kinematicActivated = true;
			for (int i = 0; i <= lastActiveStone; ++i)
			{
				stones [i].SetKinematic (true);
			}
		}
		else if (!isFPSActive && kinematicActivated)
		{
			kinematicActivated = false;
			for (int i = 0; i <= lastActiveStone; ++i)
			{
				stones [i].SetKinematic (false);
			}			
		}
		if (kinematicActivated) {
			timeSinceLastUpdateWithPowerActivated += Time.deltaTime;
			if (world.doUpdate)
			{
				for (int i = 0; i <= lastActiveStone; ++i)
				{
					stones [i].DoUpdate (timeSinceLastUpdateWithPowerActivated);
				}
				if (activeStones)
				{
					int stoneNumber = Mathf.FloorToInt (timeStonesActivated / timeBetweenStones);
					for(int i = lastActiveStone + 1; i <= stoneNumber; ++i)
					{
						if (i < stones.Count && !stones [i].IsGravityActivated ())
						{
							stones [i].SetKinematic (true);
							stones [i].ActiveGravity ();
						}
						else if (stoneNumber >= stones.Count)
							activeStones = false;
					}
					lastActiveStone = Mathf.Min(stoneNumber, stones.Count - 1);
					timeStonesActivated += timeSinceLastUpdateWithPowerActivated/2.0f;
				}
				timeSinceLastUpdateWithPowerActivated = 0.0f;
			}
		}
		else
		{
			if(activeStones)
			{
				int stoneNumber = Mathf.FloorToInt (timeStonesActivated / timeBetweenStones);
				for(int i = lastActiveStone + 1; i <= stoneNumber; ++i)
				{
					if (i < stones.Count && !stones [i].IsGravityActivated ())
						stones [i].ActiveGravity ();
					else if (stoneNumber >= stones.Count)
						activeStones = false;
				}
				lastActiveStone = Mathf.Min(stoneNumber, stones.Count - 1);
				timeStonesActivated += Time.deltaTime;
			}
		}
	}

	public void ResposStones()
	{
		activeStones = false;
		for (int i = 0; i < transform.childCount; ++i) {
			stones [i].RestartPos ();
		}
		lastActiveStone = -1;
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Player") {
			activeStones = true;
			timeStonesActivated = 0.0f;
		}
	}

	void ActivateFPS()
	{
		isFPSActive = true;
	}

	void DeactivateFPS()
	{
		isFPSActive = false;
	}
}
