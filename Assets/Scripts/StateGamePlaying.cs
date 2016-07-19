using UnityEngine;
using System.Collections;
public class StateGamePlaying : GameState 
{
	private bool isPaused = false;
	private GUIStyle myStyle;
	
	public StateGamePlaying(GameManager manager):base(manager){	}
	
	public override void OnStateExit()
	{
		Screen.lockCursor = false;
	}
	
	public override void OnStateEntered()
	{
		myStyle = new GUIStyle();
		myStyle.fontSize = 24;
	}
	public override void StateUpdate() 
	{
		gameManager.playerTime -= Time.deltaTime;

		if(gameManager.playerTime <=0)
		{
			gameManager.NewGameState(gameManager.stateGameLost);
		}

		if (Input.GetKeyDown(KeyCode.Escape)) 
		{
			if (isPaused)
			{
				ResumeGameMode();
			}
			else
			{
				PauseGameMode();
			}
		}
	}
	
	public override void StateGUI() 
	{	
		GUI.Label(new Rect(Screen.width * 0.5f, Screen.height * 0.1f, 50f,50f), new GUIContent(ConvertTimeToString(gameManager.playerTime)));
	}
	
	private void ResumeGameMode() 
	{
		Time.timeScale = 1.0f;
		isPaused = false;
	}
	
	private void PauseGameMode() 
	{
		Time.timeScale = 0.0f;
		isPaused = true;
	}

	private string ConvertTimeToString(float time)
	{
		float mins = Mathf.Floor(time * 0.016f);
		float seconds = Mathf.Floor(time - (mins*60));
		
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