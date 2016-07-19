using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class TargetingSensor : MonoBehaviour {

	public GameObject equippedTo;
	private List<Transform> targets;
	private List<Transform> friendlys;
	private List<Transform> obstacles;
	private int curTarget = 0;
	private int curFriendly = 0;
	public string playerTag = "Player";
	public string obstacleTag = "Obstacle";
	
	// Use this for initialization
	void Start () 
	{
		targets = new List<Transform>();
		targets.Add(null);

		friendlys = new List<Transform>();
		friendlys.Add(null);

		obstacles = new List<Transform>();
		obstacles.Add(null);

	}
	
	// Update is called once per frame
	void Update () 
	{
	
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

}
