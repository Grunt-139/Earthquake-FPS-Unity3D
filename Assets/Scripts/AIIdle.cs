using UnityEngine;
using System.Collections;

public class AIIdle : EnemyState {

	public float idleTime = 5.0f;
	private float curTime;

	public override void OnCreate()
	{
		curTime = idleTime;
		stateName = "Idle";
	}
	public override void OnStateEntered()
	{
		curTime = idleTime;
		manager.senseManager.ChangeAlertStatus(SensesManager.ALERT_STATE.CALM);
		manager.isAiming = false;
		manager.isAttacking = false;
	}
	public override void OnStateExit(){}
	public override void StateUpdate()
	{
		curTime -= Time.deltaTime;

		if(curTime < 0)
		{
			manager.NewState(manager.patrolState);
		}
	}
}
