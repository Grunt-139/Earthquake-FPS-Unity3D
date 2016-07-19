using UnityEngine;
using System.Collections;

public class EnemySpawnTrigger : MonoBehaviour {

	public GameManager.DIFICULTY myDifficulty = GameManager.DIFICULTY.EASY;
	public GameObject[] enemies;
	public int playerLayer = 10;

	private bool troopsActive = false;

	// Use this for initialization
	void Start () 
	{
		for(int i=0; i < enemies.Length; i++)
		{
			enemies[i].SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(GameManager.Instance.difficulty != myDifficulty)
		{
			for(int i=0; i < enemies.Length; i++)
			{
				Destroy(enemies[i]);
			}
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == playerLayer && !troopsActive)
		{
			ActivateTroops();
		}
	}

	private void ActivateTroops()
	{
		for(int i=0; i < enemies.Length; i++)
		{
			enemies[i].SetActive(true);
		}
		troopsActive = true;
	}
}
