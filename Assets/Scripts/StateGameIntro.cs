using UnityEngine;
using System.Collections;
public class StateGameIntro : GameState 
{
	public StateGameIntro(GameManager manager):base(manager) { }
	private float delay = 2.0f;
	private float goTime = 0.0f;
	
	public override void OnStateEntered()
	{
		goTime = Time.time + delay;
	}
	public override void OnStateExit(){}
	public override void StateUpdate() 
	{
		if(Time.time > goTime)
		{
			GameManager.Instance.NewGameState(GameManager.Instance.stateGameMenu);
		}
	}
	public override void StateGUI() 
	{
		GUI.Label( new Rect(Screen.width * 0.5f, Screen.height * 0.5f,80f,20f), new GUIContent("Loading...") );
	}
}
