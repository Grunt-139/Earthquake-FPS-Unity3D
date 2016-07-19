using UnityEngine;
using System.Collections;
public class StateGameLost : GameState 
{
	public StateGameLost(GameManager manager):base(manager){ }
	
	public override void OnStateEntered()
	{
		Screen.lockCursor = false;
	}
	public override void OnStateExit(){}
	public override void StateUpdate() {}
	public override void StateGUI() 
	{
		GUILayout.Label("state: GAME LOST");

		if(GUI.Button(new Rect(Screen.width * 0.5f, Screen.height *0.5f,50f,100f),"Main Menu"))
		{
			GameManager.Instance.NewGameState(GameManager.Instance.stateGameMenu);
		}
	}
}
