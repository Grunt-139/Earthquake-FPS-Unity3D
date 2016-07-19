using UnityEngine;
using System.Collections;

public class EnemyAIManager : MonoBehaviour {

	//States
	public EnemyState idleState;
	public EnemyState patrolState;
	public EnemyState searchState;
	public EnemyState combatState;

	public NavMeshAgent agent{get;set;}
	public Animator animController{get;set;}

	//Current State
	private EnemyState currentState;

	//Senses
	public SensesManager senseManager{get;set;}

	//Move Direction
	public Vector3 moveDirection{get;set;}
	//Combat Bools
	public bool isAttacking{get;set;}
	public bool isReloading{get;set;}
	public bool isAiming{get;set;}

	//Target
	public Transform target{get;set;}
	public bool canSeeTarget{get;set;}


	private int reloadingState;
	private AnimatorStateInfo curUpperBodyState;

	// Use this for initialization
	void Start () 
	{
		idleState.manager = this;
		patrolState.manager = this;
		searchState.manager = this;
		combatState.manager = this;

		idleState.OnCreate();
		patrolState.OnCreate();
		searchState.OnCreate();
		combatState.OnCreate();

		//Initialization
		isAttacking = false;
		isReloading = false;
		isAiming = false;

		animController = gameObject.GetComponent<Animator>();
		agent = gameObject.GetComponent<NavMeshAgent>();

		currentState = idleState;

		reloadingState = Animator.StringToHash("UpperBody.Reload");
	}
	
	// Update is called once per frame
	void Update () 
	{
		print(currentState + " " + senseManager.curState + " " + canSeeTarget);
		if(currentState != null)
		{
			currentState.StateUpdate();
		}

		moveDirection = gameObject.transform.forward;

		//print (isReloading);
		//Tell the anim controller what is happening
		animController.SetFloat("Speed",agent.speed);
		animController.SetFloat("Direction",moveDirection.x);	
		animController.SetBool("isAttacking",isAttacking);
		animController.SetBool("isReloading",isReloading);
		animController.SetBool("isAiming",isAiming);

		if(isReloading)
		{
			//This state is the current animator state, the variable curUpperBodyState at this point is what the state was LAST FRAME
			//If the last frame state was the reloading state but the current one is not, the animation is done
			AnimatorStateInfo curAnimState = animController.GetCurrentAnimatorStateInfo(1);
			if(curUpperBodyState.nameHash == reloadingState && curAnimState.nameHash != reloadingState)
			{
				isReloading = false;
			}
			
		}
		curUpperBodyState = animController.GetCurrentAnimatorStateInfo(1);
	}

	public void NewState(EnemyState newState)
	{
		if(newState == null)
			return;

		if( null != currentState)
		{
			currentState.OnStateExit();
		}

		currentState = newState;
		agent.speed = currentState.speed;
		currentState.OnStateEntered();
	}

	public bool CheckState(EnemyState state)
	{
		if(currentState == state)
		{
			return true;
		}
		return false;
	}
}
