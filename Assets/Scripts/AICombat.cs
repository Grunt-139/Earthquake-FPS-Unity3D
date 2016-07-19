using UnityEngine;
using System.Collections;

public class AICombat : EnemyState {


	public vp_Shooter m_Shooter = null;
	public float attackCooldown = 2.0f;
	public float attackTime = 1.0f;
	public float moveDist = 5.0f;
	public float attackDist = 20.0f;
	public int ammoCount = 20;
	public int burstCount = 5;

	public enum STATE{ATTACK,MOVE};

	private STATE curState = STATE.MOVE;

	private int curAmmo;
	private float curTime;
	private Vector3 coverPosition;
	private Transform target;
	private bool needAmmo = false;
	private Vector3 distToPlayer;

	public override void OnCreate()
	{
		curAmmo = ammoCount;
		stateName = "combat";
	}
	public override void OnStateEntered()
	{
		FindCover();
		manager.senseManager.ChangeAlertStatus(SensesManager.ALERT_STATE.COMBAT);
		manager.isAiming = true;
		target = manager.target;
		curState = STATE.ATTACK;
	}
	public override void OnStateExit(){}
	public override void StateUpdate()
	{
		//print(curState);
		//Leave this state and enter the search state if the player is out of sight OR out of range of the enemy's senses
		if(!manager.canSeeTarget || manager.target == null)
		{
			if(!manager.CheckState(manager.searchState))
			{
				manager.NewState(manager.searchState);
			}
			return;
		}
	
		if(!manager.isReloading && needAmmo)
		{
			Reload();
		}

		distToPlayer = target.position - transform.position;
		FacePlayer();

		switch(curState)
		{
		case STATE.MOVE:
			curTime -= Time.deltaTime;
			if(curTime <=0)
			{
				if(distToPlayer.magnitude > attackDist)
				{
					manager.agent.SetDestination(target.position);
				}
				else
				{
					curState = STATE.ATTACK;
				}
			}
			else
			{
				manager.agent.SetDestination(coverPosition);
			}
			break;
		case STATE.ATTACK:
			Attack();
			break;
		}
	}

	private void Attack()
	{	
		if(manager.isReloading)
		{
			FindCover();
			curState = STATE.MOVE;
		}
		//curTime -= Time.deltaTime;
		manager.agent.Stop();

		// fire the shooter
		Fire();

		FindCover();
	}


	private void FacePlayer()
	{
		//Have the enemy face the player
		Vector3 lookDirection = (target.position - transform.position).normalized;
		Quaternion rotation = Quaternion.LookRotation(lookDirection);
		
		transform.rotation = Quaternion.Slerp(transform.rotation,rotation,20f);
	}

	private void Fire()
	{
		//If its out of ammo then return
		if(needAmmo)
			return;

		if(curAmmo <= 0)
		{
			manager.isReloading = true;
			manager.isAiming = false;
			manager.isAttacking = false;
			needAmmo = true;

			FindCover();

			return;
		}
		manager.isAttacking = true;
		int shots = burstCount;

		if((curAmmo - burstCount) < 0)
		{
			shots = curAmmo;
		}

		for(int i=0; i < shots; i++)
		{
			m_Shooter.TryFire();
			curAmmo-= m_Shooter.ProjectileCount;
		}
	}

	public void Reload()
	{
		curAmmo = ammoCount; 
		needAmmo = false;
		manager.isReloading = false;
		manager.isAiming = true;

		FindCover();
	}

	private void FindCover()
	{
		curTime = attackCooldown;
		if(manager.senseManager.GetObstacle() != null)
		{
			NavMeshHit hit;
			NavMesh.SamplePosition(manager.senseManager.GetObstacle().position,out hit, moveDist,1);
			coverPosition = hit.position;
		}
		else
		{
			//Point me in a direction
			Vector3 randDir = transform.position + (Random.insideUnitSphere * moveDist);

			NavMeshHit hit;
			NavMesh.SamplePosition(randDir,out hit,moveDist,1);
			coverPosition = hit.position;
		}

		manager.agent.SetDestination(coverPosition);

		curState = STATE.MOVE;
	}

}
