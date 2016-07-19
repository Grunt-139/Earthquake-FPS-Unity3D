using UnityEngine;
using System.Collections;

public class WinOnContact : MonoBehaviour {

	public GameObject player;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag(player.tag))
		{
			GameManager.Instance.NewGameState(GameManager.Instance.stateGameWon);
		}
	}
}
