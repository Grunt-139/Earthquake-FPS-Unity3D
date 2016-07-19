using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour 
{

	//Waypoints
	public Transform waypointContainer;
	private Transform[] waypoints;
	private Vector3[] searchWaypoints;
	private int currentWaypoint;

	//Search waypoints
	public float searchDistance = 5.0f;
	public int searchLength = 5;

	public GameObject eyes;
	public GameObject ears;

	private TargetingSensor eyeSensor;
	private TargetingSensor earSensor;

	//Delays
	public float idleDelay = 5.0f;

	//Nav Agent
	private NavMeshAgent agent;

	//Movement
	private Vector3 moveDirection = Vector3.zero;

	//States
	private enum STATES {IDLE,PATROL,SEARCH,COMBAT};
	private enum ALERT_STATE{CALM,SUSPICIOUS,HOSTILE};
	private ALERT_STATE curAlertState = ALERT_STATE.CALM;
	private STATES curState = STATES.IDLE;

	//Combat stats
//	private bool isReloading = false;
//	private bool isAttacking = false;
//	private bool isAiming = false;
	private bool isReloading = false;
	private bool isAttacking = false;
	private bool isAiming = false;

	//Animation Controller
	private Animator animController;


	//Target
	private Transform target;


	//Timers
	private float idleTime;

	// Use this for initialization
	void Start () 
	{

		eyeSensor = eyes.GetComponent<TargetingSensor>();
		earSensor = ears.GetComponent<TargetingSensor>();

		idleTime = idleDelay;

		//Get anim controller
		animController = gameObject.GetComponent<Animator>();

		//Get the navmesh agent
		agent = gameObject.GetComponent<NavMeshAgent>();

		//Get the waypoint
		GetWaypoints();

		//Build the search waypoints
		searchWaypoints = new Vector3[searchLength];
		BuildSearchRoute();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Update based on current state
		switch(curState)
		{
		case STATES.IDLE:
			//Stands and waits until told to leave this state
			Idle();
			break;
		case STATES.PATROL:
			//Will patrol an area following waypoints
			Patrol();
			break;
		case STATES.SEARCH:
			//Will search the area where the player was last seen
			Search();
			break;
		case STATES.COMBAT:
			//Will move into position to attack the player
			Combat();
			break;
		}

		CheckForTarget();

		animController.SetFloat("Speed",moveDirection.z);
		animController.SetFloat("Direction",moveDirection.x);	

		animController.SetBool("isAttacking",isAttacking);
		animController.SetBool("isReloading",isReloading);
		animController.SetBool("isAiming",isAiming);
	}

	private void Idle()
	{
		idleTime -= Time.deltaTime;

		if(idleTime < 0)
		{
			//curState = STATES.PATROL;
		}
		moveDirection = Vector3.zero;

	}

	private void Patrol()
	{
		Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3( waypoints[currentWaypoint].position.x, transform.position.y, waypoints[currentWaypoint].position.z ) );
		if ( RelativeWaypointPosition.magnitude <= agent.stoppingDistance ) 
		{
			currentWaypoint ++;
			
			//completed a lap
			if ( currentWaypoint >= waypoints.Length ) 
			{
				currentWaypoint = 0;
				curState = STATES.IDLE;
			}
			agent.SetDestination(waypoints[currentWaypoint].position);
		}
		agent.SetDestination(waypoints[currentWaypoint].position);

		moveDirection = gameObject.transform.forward;
	}

	private void Search()
	{
		Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3( searchWaypoints[currentWaypoint].x, transform.position.y, searchWaypoints[currentWaypoint].z ) );
		if ( RelativeWaypointPosition.magnitude <= agent.stoppingDistance ) 
		{
			currentWaypoint ++;
			
			//completed a lap
			if ( currentWaypoint >= searchWaypoints.Length ) 
			{
				curState = STATES.PATROL;
				currentWaypoint = 0;
			}
			agent.SetDestination(searchWaypoints[currentWaypoint]);
		}
		agent.SetDestination(searchWaypoints[currentWaypoint]);

		moveDirection = gameObject.transform.forward;
	}

	private void UpdateSensorSizes()
	{
		switch(curAlertState)
		{
		case ALERT_STATE.CALM:
			break;
		case ALERT_STATE.SUSPICIOUS:
			break;
		case ALERT_STATE.HOSTILE:
			break;
		}
	}

	private void Combat()
	{
		//This is where we do combat...wheeee
		//The enemy needs to move into range to fire at the player
		//Move into and out of cover
		//Decide to retreat or advance

		agent.SetDestination(target.position);
	}

	void BuildSearchRoute()
	{
		Vector3 randomPoint = Vector3.zero;
		randomPoint = Random.insideUnitSphere * searchDistance;
		randomPoint += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomPoint,out hit,searchDistance,1);
		searchWaypoints[0] = hit.position;
		
		for(int i=1; i < searchLength; i++)
		{
			randomPoint = Random.insideUnitSphere * searchDistance;
			randomPoint += searchWaypoints[i-1];
			NavMesh.SamplePosition(randomPoint,out hit,searchDistance,1);
			searchWaypoints[i] = hit.position;
		}
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

	void CheckForTarget()
	{
		//Checks for a target
		earSensor.FindValidTarget();
		eyeSensor.FindValidTarget();

		//Hearing sensor check
		if(earSensor.GetTarget() !=null)
		{
			target = earSensor.GetTarget();
		}
		//Sight sensor check
		else if(eyeSensor.GetTarget() !=null)
		{
			target = eyeSensor.GetTarget();
		}
		else
		{
			target = null;
		}

		//If the target is not null set the state to combat
		if(target != null)
		{
			curState = STATES.COMBAT;
		}

		if(curState == STATES.COMBAT && target == null)
		{
			BuildSearchRoute();
			curState = STATES.SEARCH;
		}

	}
	
	public Transform GetCurrentWaypoint()
	{
		return waypoints[currentWaypoint];	
	}
	
	public Transform GetLastWaypoint()
	{
		if(currentWaypoint - 1 < 0)
		{
			return waypoints[waypoints.Length - 1];
		}
		
		return waypoints[currentWaypoint - 1];
	}
	
	public void SetWaypointContainer(Transform container)
	{
		waypointContainer = container;
		GetWaypoints();
		currentWaypoint = 0;
	}

//	void OnDrawGizmos () 
//	{
//		if(searchWaypoints != null)
//		{
// 			Vector3 last = searchWaypoints[searchWaypoints.Length-1];
//			for (int i = 1; i < waypoints.Length; i++ )
//			{
//				Gizmos.color = Color.blue;
//				Gizmos.DrawSphere( searchWaypoints[i], 0.5f);
//				Gizmos.DrawLine(last,searchWaypoints[i]);
//				last = searchWaypoints[i];
//			}
//		}
//	}
}
