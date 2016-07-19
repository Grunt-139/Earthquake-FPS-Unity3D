using UnityEngine;
using System.Collections;

public class StateGameMainMenu : GameState {
	
	public StateGameMainMenu(GameManager manager):base(manager) { }
	
	public override void OnStateEntered(){}
	public override void OnStateExit(){}
	public override void StateUpdate() {}
	
	public override void StateGUI() 
	{
		if(GUI.Button( new Rect(Screen.width * 0.5f, Screen.height * 0.45f,80f,20f), new GUIContent("Start Game") ))
		{
			GameManager.Instance.NewGameState(GameManager.Instance.stateSetup);
		}
		
		if(GUI.Button( new Rect(Screen.width * 0.5f, Screen.height * 0.55f, 80f,20f), new GUIContent("Quit Game") ))
		{
			Application.Quit();
		}
	}
}
