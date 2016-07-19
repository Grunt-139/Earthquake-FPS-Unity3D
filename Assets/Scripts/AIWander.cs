using UnityEngine;
using System.Collections;

public class AIWander : EnemyState {


	public override void OnCreate()
	{ 
		stateName = "Wander";
	}
	public override void OnStateEntered(){}
	public override void OnStateExit(){}
	public override void StateUpdate(){}
}
