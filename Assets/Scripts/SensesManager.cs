using UnityEngine;
using System.Collections;


public class SensesManager : MonoBehaviour {
	
	//Senses
	public Sense ears;
	public Sense eyes;
	public DetectionMarker marker;
	public float headOffset = 0.5f;
	public float alertDelay = 0.1f;
	
	private EnemyAIManager manager;
	private Transform curTarget;
	private float curTime;
	private bool delay = false;

	public enum ALERT_STATE{CALM,ALERT,COMBAT};
	public ALERT_STATE curState = ALERT_STATE.CALM;
	
	// Use this for initialization
	void Start () 
	{
		//Get the AI manager
		manager = gameObject.GetComponent<EnemyAIManager>();
		//Tell the manager about itself
		manager.senseManager = this;

		ears.manager = this;
		eyes.manager = this;
	}
	
	// Update is called once per frame
	//This update just checks to see if there is a valid target or not in either of the senses
	void Update () 
	{
		ears.FindValidTarget();
		eyes.FindValidTarget();
		//Ordering makes it so the eyes have priority 
		if(ears.GetTarget() != null)
		{
			curTarget = ears.GetTarget();
		}
		else if(eyes.GetTarget() != null)
		{
			curTarget = eyes.GetTarget();
		}
		else if(eyes.GetTarget() == null && ears.GetTarget() == null && curTime <= 0)
		{
			curTarget = null;
		}
		else if(eyes.GetTarget() == null && ears.GetTarget() == null && !delay)
		{
			curTime = alertDelay;
			delay = true;
		}
		else
		{
			if(curTime > 0.0f)
			{
				curTime -= Time.deltaTime;
			}

			if(curTime < 0.0f)
			{
				curTime = 0.0f;
				delay = false;
			}
		}
		
		manager.target = curTarget;
		
		//If the target is not null, it checks to see if the target is in line of sight
		//If it is and the enemy isnt in its combat state it sets it to it
		//Either way it will tell it that the target is in line of sight
		if(curTarget != null)
		{
			if(InLineOfSight(curTarget.position))
			{
				if(!manager.CheckState(manager.combatState))
				{
					manager.NewState(manager.combatState);
				}
				
				manager.canSeeTarget = true;
			}
			else
			{
				manager.canSeeTarget = false;

				if(!manager.CheckState(manager.searchState))
				{
					manager.NewState(manager.searchState);
				}
			}
		}
		
	}
	
	private bool InLineOfSight(Vector3 target)
	{
		RaycastHit sight;
		//Set up a mask for the obstacles and default
		LayerMask mask = ((1 << 11) | (1 << 0));
		Vector3 pos = transform.position;
		pos.y += headOffset;
		Debug.DrawLine(pos,target);
		if(Physics.Linecast(pos,target,out sight,mask))
		{
			return false;
		}
		//		if(Physics.Raycast(pos,target - pos,Mathf.Infinity,mask))
		//		{
		//			return true;
		//		}
		
		return true;
		
	}

	//This must be called for the alert status to be properly changed
	public void ChangeAlertStatus(SensesManager.ALERT_STATE newState)
	{
		if(newState != curState)
		{
			curState = newState;
			ears.ChangeAlertState(newState);
			eyes.ChangeAlertState(newState);
			marker.UpdateImage(newState,curTarget);

//			switch(newState)
//			{
//			case ALERT_STATE.CALM:
//				AudioManager.Instance.enemiesAlerted--;
//				AudioManager.Instance.ChangeMood(AudioManager.MOOD.CALM);
//				break;
//			case ALERT_STATE.ALERT:
//				AudioManager.Instance.enemiesAlerted++;
//				AudioManager.Instance.ChangeMood(AudioManager.MOOD.COMBAT);
//				break;
//			case ALERT_STATE.COMBAT:
//				AudioManager.Instance.ChangeMood(AudioManager.MOOD.COMBAT);
//				break;
//			}
		}
	}

	public Transform GetObstacle()
	{
		ears.FindValidObstacle();
		eyes.FindValidObstacle();

		Transform ret = null;
		if(ears.GetObstacle() != null)
		{
			ret = ears.GetObstacle();
		}
		else if(eyes.GetObstacle() != null)
		{
			ret = eyes.GetObstacle();
		}
		else if(ears.GetObstacle() == null && eyes.GetObstacle() == null)
		{
			ret = null;
		}
		
		return ret;
	}
	
}
