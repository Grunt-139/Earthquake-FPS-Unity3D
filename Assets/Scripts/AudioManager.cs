using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	private static AudioManager instance = null;
	public static AudioManager Instance{get{ return instance; }}

	public enum MOOD{CALM,COMBAT}

	public MOOD curMood = MOOD.CALM;

	public AudioSource calmSound;
	public AudioSource combatSound;

	public float targetVolume = 1.0f;
	public float fadeInSpeed = 0.1f;
	public float fadeOutSpeed = 0.1f;

	public int enemiesAlerted{get;set;}

	private AudioSource curAudio;
	private AudioSource nextAudio;
	private bool fadingIn = false;
	private bool fadingOut = false;

	void Awake()
	{
		if(instance != null && instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		else
		{
			instance = this;
		}
	}


	// Use this for initialization
	void Start () 
	{
		switch(curMood)
		{
		case MOOD.CALM:
			curAudio =calmSound;
			break;
		case MOOD.COMBAT:
			curAudio = combatSound;
			break;
		}
		curAudio.volume = targetVolume;
		curAudio.Play();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(fadingIn)
		{
			FadeIn();
		}

		if(fadingOut)
		{
			FadeOut();
		}
	}

	public void ChangeMood(AudioManager.MOOD newMood)
	{
		if(newMood != curMood && !fadingIn && !fadingOut)
		{
			switch(newMood)
			{
			case MOOD.CALM:
				if(enemiesAlerted == 0)
				{
					nextAudio = calmSound;
				}
				break;
			case MOOD.COMBAT:
				nextAudio = combatSound;
				break;
			}
			fadingOut = true;
		}
	}

	private void FadeIn()
	{
		if(curAudio.volume < targetVolume)
		{
			curAudio.volume += fadeInSpeed * Time.deltaTime;
		}
		else
		{
			fadingIn = false;
		}
	}

	private void FadeOut()
	{
		if(curAudio.volume > 0.1f)
		{
			curAudio.volume -= fadeOutSpeed * Time.deltaTime;
		}
		else
		{

			fadingOut = false;
			fadingIn = true;
			curAudio = nextAudio;

			curAudio.Play();
		}
	}


}
