using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sense : MonoBehaviour {

	public GameObject equippedTo;
	private List<Transform> targets;
	private List<Transform> friendlys;
	private List<Transform> obstacles;
	private int curTarget = 0;
	private int curFriendly = 0;
	private int curObstacle =0;
	public string playerTag = "Player";
	public string obstacleTag = "Obstacle";

	public enum SENSE_TYPE {HEARING,SIGHT};
	public SENSE_TYPE sense = SENSE_TYPE.SIGHT;

	public BoxCollider sightSensor;
	public SphereCollider hearingSensor;

	public float alertMod = 1.5f;
	public float combatMod = 2.0f;

	public Vector3 boxBaseSize;
	public Vector3 boxBaseCenter;
	public float sphereBaseRadius;

	public float newRadius;
	public Vector3 newSize;
	public Vector3 newCenter;
	public float changeSpeed = 1.0f;

	public SensesManager manager{get;set;}
	private float curSpeed;


	// Use this for initialization
	void Start () 
	{
		curSpeed = changeSpeed;

		targets = new List<Transform>();
		targets.Add(null);
		
		friendlys = new List<Transform>();
		friendlys.Add(null);
		
		obstacles = new List<Transform>();
		obstacles.Add(null);

		switch(sense)
		{
		case SENSE_TYPE.HEARING:
			sphereBaseRadius = hearingSensor.radius;
			newRadius = sphereBaseRadius;
			break;
		case SENSE_TYPE.SIGHT:
			boxBaseSize = sightSensor.size;
			boxBaseCenter = sightSensor.center;

			newSize = boxBaseSize;
			newCenter = boxBaseCenter;


			break;
		}
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(sense)
		{
		case SENSE_TYPE.HEARING:
			hearingSensor.radius = Mathf.Lerp(hearingSensor.radius,newRadius, curSpeed * Time.deltaTime);
			break;
		case SENSE_TYPE.SIGHT:
			sightSensor.size = Vector3.Lerp(sightSensor.size,newSize, curSpeed * Time.deltaTime);
			sightSensor.center = Vector3.Lerp(sightSensor.center,newCenter, curSpeed * Time.deltaTime);
			break;
		}

		//Debug code
		ChangeAlertState(manager.curState);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag(equippedTo.tag) && other.gameObject != equippedTo.gameObject)
		{
			friendlys.Add(other.transform);
		}
		else if(other.CompareTag(playerTag))
		{
			targets.Add(other.transform);
		}
		else if(other.CompareTag(obstacleTag))
		{
			obstacles.Add(other.transform);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag(equippedTo.tag))
		{
			friendlys.Remove(other.transform);
		}
		else if(other.CompareTag(playerTag))
		{
			targets.Remove(other.transform);
		}
		else if(other.CompareTag(obstacleTag))
		{
			obstacles.Remove(other.transform);
		}
	}
	
	
	public void FindValidTarget()
	{
		if(targets.Count > 0)
		{
			curTarget = Random.Range(0,targets.Count);
		}
		else
		{
			curTarget = 0;
		}
	}
	
	public void FindValidFriendly()
	{
		if(friendlys.Count > 0)
		{
			curFriendly = Random.Range(0,friendlys.Count);
		}
		else
		{
			curFriendly = 0;
		}
	}

	public void FindValidObstacle()
	{
		if(obstacles.Count > 0)
		{
			curObstacle = Random.Range(0,obstacles.Count);
		}
		else
		{
			curFriendly = 0;
		}
	}

	public Transform GetObstacle()
	{
		if(curObstacle >= obstacles.Count)
		{
			curObstacle =0;
		}
		return obstacles[curObstacle];
	}
	
	public Transform GetTarget()
	{
		if(curTarget >= targets.Count)
		{
			curTarget = 0;
		}
		return targets[curTarget];
	}
	
	public Transform GetFriendly()
	{
		if(curFriendly >= friendlys.Count)
		{
			curFriendly = 0;
		}
		return friendlys[curFriendly];
	}

	public int GetSenseTypeAsInt()
	{
		switch(sense)
		{
		case SENSE_TYPE.HEARING:
			return 0;
		case SENSE_TYPE.SIGHT:
			return 1;
		}
		return 0;
	}

	public void ChangeAlertState(SensesManager.ALERT_STATE state)
	{
		float mod = 1;
		switch(state)
		{
		case SensesManager.ALERT_STATE.CALM:
			switch(sense)
			{
			case SENSE_TYPE.HEARING:
				newRadius = sphereBaseRadius * mod;
				changeSpeed *= mod;
				break;
			case SENSE_TYPE.SIGHT:
				//Have to shift the cubes center plus the size
				newSize.x = boxBaseSize.x;
				newSize.z = boxBaseSize.z;
				newCenter.x = boxBaseCenter.x;
				newCenter.z = boxBaseCenter.z;


				curSpeed = changeSpeed * mod;
				break;
			}
			return;
		case SensesManager.ALERT_STATE.ALERT:
			mod = alertMod;
			break;
		case SensesManager.ALERT_STATE.COMBAT:
			mod = combatMod;
			break;
		}
		switch(sense)
		{
		case SENSE_TYPE.HEARING:
			newRadius = sphereBaseRadius * mod;
			break;
		case SENSE_TYPE.SIGHT:
			//Have to shift the cubes center plus the size
			newSize.x = boxBaseSize.x * mod;
			newSize.z = boxBaseSize.z * mod;
			newCenter.x = -(newSize.x * 0.5f);
			newCenter.z = newSize.z *0.5f;
			break;
		}

		curSpeed = changeSpeed *  mod;
	}
	
}

