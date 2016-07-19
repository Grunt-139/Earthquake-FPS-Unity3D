using UnityEngine;
using System.Collections;

public class SetupState : GameState {

	public SetupState(GameManager manager):base(manager) { }


	public override void OnStateEntered(){}
	public override void OnStateExit(){}
	public override void StateUpdate(){}
	public override void StateGUI() 
	{
		GUI.Label( new Rect(Screen.width * 0.25f, Screen.height * 0.4f,80f,20f), new GUIContent("How To:") );
		GUI.Label( new Rect(Screen.width * 0.3f, Screen.height * 0.41f,80f,20f), new GUIContent("WASD: Move") );
		GUI.Label( new Rect(Screen.width * 0.3f, Screen.height * 0.45f,80f,20f), new GUIContent("Mouse: Look/Shoot/Aim") );
		GUI.Label( new Rect(Screen.width * 0.3f, Screen.height * 0.48f,80f,20f), new GUIContent("E/Q: Switch Weapons") );
		GUI.Label( new Rect(Screen.width * 0.3f, Screen.height * 0.51f,80f,20f), new GUIContent("T: Use health") );
		GUI.Label( new Rect(Screen.width * 0.3f, Screen.height * 0.54f,80f,20f), new GUIContent("C: Crouch") );

		GUI.Label( new Rect(Screen.width * 0.25f, Screen.height * 0.58f,10f,20f), new GUIContent("Avoid enemy patrols and reach the helicopter for extraction") );

		if(GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * 0.75f, 80f,80f), gameManager.easyButton ))
		{
			gameManager.playerTime = gameManager.easyTime;
			gameManager.NewGameState(gameManager.stateGamePlaying);
			gameManager.difficulty = GameManager.DIFICULTY.EASY;
		}
		GUI.Label(new Rect(Screen.width * 0.25f, Screen.height * 0.8f, 80f,20f), new GUIContent(ConvertTimeToString(gameManager.easyTime) ));

		if(GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.75f, 80f,80f), gameManager.medButton))
		{
			gameManager.playerTime = gameManager.medTime;
			gameManager.NewGameState(gameManager.stateGamePlaying);
			gameManager.difficulty = GameManager.DIFICULTY.MEDIUM;
		}
		GUI.Label(new Rect(Screen.width * 0.5f, Screen.height * 0.8f, 80f,80f), new GUIContent(ConvertTimeToString(gameManager.medTime)));

		if(GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.75f, 80f,80f), gameManager.hardButton))
		{
			gameManager.playerTime = gameManager.hardTime;
			gameManager.NewGameState(gameManager.stateGamePlaying);
			gameManager.difficulty = GameManager.DIFICULTY.HARD;
		}
		GUI.Label(new Rect(Screen.width * 0.75f, Screen.height * 0.8f, 80f,20f), new GUIContent(ConvertTimeToString(gameManager.hardTime)));
	}

	private string ConvertTimeToString(int time)
	{
		int mins = time / 60;
		int seconds = time - (mins*60);

		string ret = mins.ToString() +":";

		if(seconds ==0)
		{
			ret += "00";
		}
		else
		{
			ret += seconds.ToString();
		}

		return ret;
	}
}
