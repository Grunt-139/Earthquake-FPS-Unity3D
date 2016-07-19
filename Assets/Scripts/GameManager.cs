using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	private GameState currentState;
	public StateGamePlaying stateGamePlaying{get;set;}
	public StateGameMainMenu stateGameMenu{get;set;}
	public StateGameLost stateGameLost{get;set;}
	public StateGameWon stateGameWon{get;set;}
	public SetupState stateSetup{get;set;}

	public int easyTime = 480;
	public int medTime = 300;
	public int hardTime = 240;

	public Texture2D easyButton;
	public Texture2D medButton;
	public Texture2D hardButton;

	public float playerTime {get;set;}

	public enum DIFICULTY {EASY,MEDIUM,HARD};
	public DIFICULTY difficulty = DIFICULTY.EASY;
	
	
	private static GameManager instance = null;
	public static GameManager Instance { get { return instance; } }
	
	private void Awake () 
	{
		
		if(instance !=null && instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		else
		{
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
		
		stateGamePlaying = new StateGamePlaying(this);
		stateGameLost = new StateGameLost(this);
		stateGameWon = new StateGameWon(this);
		stateGameMenu = new StateGameMainMenu(this);
		stateSetup = new SetupState(this);
	}
	
	private void Start () 
	{
		NewGameState( stateGameMenu );
	}
	
	private void Update () 
	{
		if (currentState != null)
		{
			currentState.StateUpdate();
		}
	}
	
	private void OnGUI () 
	{
		if (currentState != null)
		{
			currentState.StateGUI();
		}
	}
	
	public void NewGameState(GameState newState)
	{
		if( null != currentState)
		{
			currentState.OnStateExit();
		}

		if(newState == stateGameMenu)
		{
			Application.LoadLevel("Menu");
		}

		if(newState == stateGamePlaying)
		{
			//Application.LoadLevel("Test");
			Application.LoadLevel("FinalLevel");
		}

		if(newState == stateGameLost || newState == stateGameWon)
		{
			Application.LoadLevel("End");
		}
		
		currentState = newState;
		currentState.OnStateEntered();
	}
}