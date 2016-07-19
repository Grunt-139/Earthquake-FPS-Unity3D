using UnityEngine;
using System.Collections;

public abstract class EnemyState : MonoBehaviour
{

	public EnemyAIManager manager{get;set;}
	public string stateName{get;set;}
	public float speed;

	public abstract void OnCreate();
	public abstract void OnStateEntered();
	public abstract void OnStateExit();
	public abstract void StateUpdate();
}
