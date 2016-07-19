
//Expanded hud based on VisionPunk's Simple Hud from their Ultimate FPS package
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class ExpandedHud : MonoBehaviour
{
	
	public Texture DamageFlashTexture = null;
	public bool ShowHUD = true;
	Color m_MessageColor = new Color(2, 2, 0, 2);
	Color m_InvisibleColor = new Color(1, 1, 0, 0);
	Color m_DamageFlashColor = new Color(0.8f, 0, 0, 0);
	Color m_DamageFlashInvisibleColor = new Color(1, 0, 0, 0);
	string m_PickupMessage = "";
	protected static GUIStyle m_MessageStyle = null;

	public Texture2D compassTex;

	public Transform helicopter;

	private Vector3 heliDist;

	public static GUIStyle MessageStyle
	{
		get
		{
			if (m_MessageStyle == null)
			{
				m_MessageStyle = new GUIStyle("Label");
				m_MessageStyle.alignment = TextAnchor.MiddleCenter;
			}
			return m_MessageStyle;
		}
	}
	
	private vp_FPPlayerEventHandler m_Player = null;
	
	
	/// <summary>
	///
	/// </summary>
	void Awake()
	{
		m_Player = transform.GetComponent<vp_FPPlayerEventHandler>();
	}


	void Start()
	{
	}


	void Update()
	{
		heliDist = helicopter.position - transform.position;
	}
	

	/// <summary>
	/// registers this component with the event handler (if any)
	/// </summary>
	protected virtual void OnEnable()
	{
		
		if (m_Player != null)
			m_Player.Register(this);
		
	}
	
	
	/// <summary>
	/// unregisters this component from the event handler (if any)
	/// </summary>
	protected virtual void OnDisable()
	{
		
		
		if (m_Player != null)
			m_Player.Unregister(this);
		
	}
	
	
	/// <summary>
	/// this draws a primitive HUD and also renders the current
	/// message, fading out in the middle of the screen
	/// </summary>
	protected virtual void OnGUI()
	{
		
		if (!ShowHUD)
			return;
		
		// display a simple 'Health' HUD
		GUI.Box(new Rect(10, Screen.height - 30, 100, 22), "Health: " + (int)(m_Player.Health.Get() * 100.0f) + "%");
		//Armour
		if(m_Player.Armour.Get() > 0.0f)
		{
			GUI.Box(new Rect(10, Screen.height - 60, 100, 22), "Armour: " + (int)(m_Player.Armour.Get() * 100.0f) + "%");
		}

		//Helicopter Dist
		GUI.Box(new Rect(10, Screen.height - 90, 180, 22), "Dist to helicopter: " + Mathf.FloorToInt(heliDist.magnitude) + " m");
		
		// display a simple 'Clips' HUD
		GUI.Box(new Rect(Screen.width - 220, Screen.height - 30, 100, 22), "9mm Clips: " + m_Player.GetItemCount.Send("PistolClip"));
		
		// display a simple 'Ammo' HUD
		GUI.Box(new Rect(Screen.width - 110, Screen.height - 30, 100, 22), "Ammo: " + m_Player.CurrentWeaponAmmoCount.Get());
		// display a simple 'Magazines' HUD
		GUI.Box(new Rect(Screen.width - 330, Screen.height - 30, 100, 22), "Rifle Mags: " + m_Player.GetItemCount.Send("MgClip"));

		// display a simple 'Health Pack' HUD
		GUI.Box(new Rect(Screen.width - 450, Screen.height - 30, 110, 22), "Health Packs: " + m_Player.GetItemCount.Send("HealthPack"));


		//Draw the compass
		//GUI.DrawTexture(GetCompassRect(),compassTex);


		
		// show a message in the middle of the screen and fade it out
		if (!string.IsNullOrEmpty(m_PickupMessage) && m_MessageColor.a > 0.01f)
		{
			m_MessageColor = Color.Lerp(m_MessageColor, m_InvisibleColor, Time.deltaTime * 0.4f);
			GUI.color = m_MessageColor;
			GUI.Box(new Rect(200, 150, Screen.width - 400, Screen.height - 400), m_PickupMessage, MessageStyle);
			GUI.color = Color.white;
		}
		
		// show a red glow along the screen edges when damaged
		if (DamageFlashTexture != null && m_DamageFlashColor.a > 0.01f)
		{
			m_DamageFlashColor = Color.Lerp(m_DamageFlashColor, m_DamageFlashInvisibleColor, Time.deltaTime * 0.4f);
			GUI.color = m_DamageFlashColor;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), DamageFlashTexture);
			GUI.color = Color.white;
		}

	}

	private Rect GetCompassRect()
	{
		Vector3 norm = heliDist.normalized;
		float blipX = (Screen.width * 0.5f) + norm.x * 100.0f;
		float blipY = (Screen.height * 0.5f) + norm.z *100.0f;

	
		return new Rect(blipX - 5,blipY - 5,10,10);
	}

	
	/// <summary>
	/// updates the HUD message text and makes it fully visible
	/// </summary>
	protected virtual void OnMessage_HUDText(string message)
	{
		
		m_MessageColor = Color.white;
		m_PickupMessage = (string)message;
		
	}
	
	
	/// <summary>
	/// shows or hides the HUD full screen flash 
	/// </summary>
	protected virtual void OnMessage_HUDDamageFlash(float intensity)
	{
		
		if (DamageFlashTexture == null)
			return;
		
		if (intensity == 0.0f)
			m_DamageFlashColor.a = 0.0f;
		else
			m_DamageFlashColor.a += intensity;
		
	}
	
	
}

