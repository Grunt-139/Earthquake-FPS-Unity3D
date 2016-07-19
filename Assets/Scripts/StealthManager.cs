using UnityEngine;
using System.Collections;


public class StealthManager : MonoBehaviour {
	
	public SphereCollider noise;
	public float crouchModifier = 0.5f;
	public float shootingModifier = 10.0f;
	public float movingModifier = 1.5f;
	public float sprintingModifier = 5.0f;
	public float jumpingModifier = 2.0f;
	public float speed = 1.0f;

	private vp_FPPlayerEventHandler player = null;

	private float baseRadius;
	private float curRadius;
	private float curSpeed;

	// Use this for initialization
	void Start () 
	{
		baseRadius = noise.radius;
		player = gameObject.GetComponent<vp_FPPlayerEventHandler>();
	}

	void Update () 
	{
		curRadius = baseRadius;
		curSpeed = speed;
		if(player.Crouch.Active)
		{
			curRadius *= crouchModifier;
			curSpeed *= crouchModifier;
		}

		if(player.Run.Active)
		{
			curRadius *= sprintingModifier;
			curSpeed *= sprintingModifier;
		}


		if(player.Attack.Active)
		{
			curRadius *= shootingModifier;
			curSpeed *= shootingModifier;
		}

		if(player.Jump.Active)
		{
			curRadius *= jumpingModifier;
			curSpeed *= jumpingModifier;
		}

		if(player.Velocity.Get().magnitude> 0)
		{
			curRadius *= movingModifier;
			curSpeed *= movingModifier;
		}

		noise.radius = Mathf.Lerp(noise.radius, curRadius,speed);

	}

}
