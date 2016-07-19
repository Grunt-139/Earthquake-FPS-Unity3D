using UnityEngine;
using System.Collections;

public class AIPatrol : EnemyState{

	public Transform waypointContainer;
	private Transform[] waypoints;
	
	private int currentWaypoint = 0;
	
	
	public override void OnCreate()
	{
		GetWaypoints();
		stateName ="Patrol";
	}
	public override void OnStateEntered()
	{
		currentWaypoint = 0;
		manager.senseManager.ChangeAlertStatus(SensesManager.ALERT_STATE.CALM);
		manager.isAiming = false;
		manager.isAttacking = false;
	}

	public override void OnStateExit(){}
	
	public override void StateUpdate()
	{
		Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3( waypoints[currentWaypoint].position.x, transform.position.y, waypoints[currentWaypoint].position.z ) );
		if ( RelativeWaypointPosition.magnitude <= manager.agent.stoppingDistance ) 
		{
			currentWaypoint ++;

			//completed a lap
			if ( currentWaypoint >= waypoints.Length ) 
			{
				currentWaypoint = 0;
				manager.NewState(manager.idleState);
			}
		}
		manager.agent.SetDestination(waypoints[currentWaypoint].position);
	}
	
	
	void GetWaypoints()
	{
		//NOTE: Unity named this function poorly it also returns the parent’s component.
		Transform[] potentialWaypoints = waypointContainer.GetComponentsInChildren<Transform>();
		
		//initialize the waypoints array so that is has enough space to store the nodes.
		waypoints = new Transform[ (potentialWaypoints.Length - 1) ];
		
		//loop through the list and copy the nodes into the array.
		//start at 1 instead of 0 to skip the WaypointContainer’s transform.
		for (int i = 1; i < potentialWaypoints.Length; ++i ) 
		{
			waypoints[ i-1 ] = potentialWaypoints[i];
		}
	}
	
	public void SetWaypointContainer(Transform container)
	{
		waypointContainer = container;
		GetWaypoints();
		currentWaypoint = 0;
	}
}
