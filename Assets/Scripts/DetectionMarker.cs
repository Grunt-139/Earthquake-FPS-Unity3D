using UnityEngine;
using System.Collections;

public class DetectionMarker : MonoBehaviour {

	public MeshRenderer myMesh;
	public Material calmMarker;
	public Material alertMarker;
	public Material combatMarker;
	
	private Transform curTarget;

	private Vector3 lookDirection;
	private Quaternion lookRotation;

	void Awake()
	{
	}

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(curTarget != null)
		{
			lookRotation = transform.rotation;
			lookRotation.eulerAngles = new Vector3(0,curTarget.rotation.eulerAngles.y,0);

			transform.rotation = lookRotation;
		}
	}

	public void UpdateImage(SensesManager.ALERT_STATE state,Transform target)
	{
		switch(state)
		{
		case SensesManager.ALERT_STATE.CALM:
			myMesh.material = calmMarker;
			break;
		case SensesManager.ALERT_STATE.ALERT:
			myMesh.material = alertMarker;
			break;
		case SensesManager.ALERT_STATE.COMBAT:
			myMesh.material = combatMarker;
			break;
		}

		curTarget = target;
	}
}
