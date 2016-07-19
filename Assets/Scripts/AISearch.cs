using UnityEngine;
using System.Collections;

public class AISearch : EnemyState {

	//Search waypoints
	private Vector3[] waypoints;
	private int currentWaypoint;
	
	//Search waypoints
	public float searchDistance = 5.0f;
	public int searchLength = 5;

	public override void OnCreate()
	{
		waypoints = new Vector3[searchLength];
		stateName = "Search";
	}

	public override void OnStateEntered()
	{
		currentWaypoint = 0;
		BuildSearchRoute();
		manager.agent.SetDestination(waypoints[currentWaypoint]);
		manager.senseManager.ChangeAlertStatus(SensesManager.ALERT_STATE.ALERT);
		manager.isAiming = true;
		manager.isAttacking = false;
	}
	public override void OnStateExit(){}
	public override void StateUpdate()
	{
		Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3( waypoints[currentWaypoint].x, transform.position.y, waypoints[currentWaypoint].z ) );
		if ( RelativeWaypointPosition.magnitude <= manager.agent.stoppingDistance ) 
		{
			currentWaypoint ++;
			
			//completed a lap
			if ( currentWaypoint >= waypoints.Length ) 
			{
				currentWaypoint = 0;
				manager.NewState(manager.idleState);
			}
			manager.agent.SetDestination(waypoints[currentWaypoint]);
		}
	}
	
	void BuildSearchRoute()
	{
		Vector3 randomPoint = Vector3.zero;
		randomPoint = transform.forward.normalized * searchDistance;
		randomPoint += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomPoint,out hit,searchDistance,1);
		waypoints[0] = hit.position;
		
		for(int i=1; i < searchLength; i++)
		{
			randomPoint = Random.insideUnitSphere * searchDistance;
			randomPoint += waypoints[i-1];
			NavMesh.SamplePosition(randomPoint,out hit,searchDistance,1);
			waypoints[i] = hit.position;
		}
	}

	void OnDrawGizmos () 
	{
		if(waypoints != null)
		{
 			Vector3 last = waypoints[waypoints.Length-1];
			for (int i = 1; i < waypoints.Length; i++ )
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere( waypoints[i], 0.5f);
				Gizmos.DrawLine(last,waypoints[i]);
				last = waypoints[i];
			}
		}
	}
}
