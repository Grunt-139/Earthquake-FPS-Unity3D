/////////////////////////////////////////////////////////////////////////////////
//
//	vp_UFPSMenu.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	unity editor main menu items for UFPS
//
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;

public class UFPSMenu 
{

	[MenuItem("UFPS/About UFPS", false, 0)]
	public static void About()
	{
		vp_AboutBox.Create();
	}

	/////////////////////////////////////////////////////////////////

	[MenuItem("UFPS/UFPS Manual", false, 22)]
	public static void Manual()
	{
		Application.OpenURL("http://www.visionpunk.com/hub/assets/ufps/manual");
	}


	[MenuItem("UFPS/UFPS Add-ons", false, 23)]
	public static void Addons()
	{
		vp_AddonBrowser.Create();
	}

	/////////////////////////////////////////////////////////////////

	[MenuItem("UFPS/Input Manager", false, 100)]
	public static void InputManager()
	{
		vp_InputWindow.Init();
	}

	[MenuItem("UFPS/Event Handler", false, 102)]
	public static void EventHandler()
	{

		vp_EventHandler EventHandler = (vp_EventHandler)GameObject.FindObjectOfType(typeof(vp_EventHandler));
		vp_EventDumpWindow.Create((vp_EventHandler)EventHandler);

	}
	
	[MenuItem("UFPS/Event Handler", true)]
	public static bool ValidateEventHandler()
	{
		return Application.isPlaying;
	}

	/////////////////////////////////////////////////////////////////

	// AI

	// Mobile

	// Multiplayer

	/////////////////////////////////////////////////////////////////

	[MenuItem("UFPS/Help/F.A.Q.", false, 201)]
	public static void FAQ()
	{
		Application.OpenURL("http://www.visionpunk.com/hub/assets/ufps/faq");
	}

	[MenuItem("UFPS/Help/Support Forum", false, 202)]
	public static void SupportForum()
	{
		Application.OpenURL("http://www.visionpunk.com/hub/assets/ufps/supportforum");
	}
	
	[MenuItem("UFPS/Help/Release Notes", false, 203)]
	public static void ReleaseNotes()
	{
		Application.OpenURL("http://www.visionpunk.com/hub/assets/ufps/releasenotes");
	}

	[MenuItem("UFPS/Help/Update Instructions", false, 204)]
	public static void UpdateInstructions()
	{
		Application.OpenURL("http://www.visionpunk.com/hub/assets/ufps/upgrading");
	}

	[MenuItem("UFPS/Community/Follow us on Twitter", false, 205)]
	public static void Twitter()
	{
		Application.OpenURL("http://www.visionpunk.com/hub/twitter");
	}

	[MenuItem("UFPS/Community/YouTube Channel", false, 206)]
	public static void YouTube()
	{
		Application.OpenURL("http://www.visionpunk.com/hub/youtube");
	}

	[MenuItem("UFPS/Community/Official UFPS Forum", false, 207)]
	public static void OfficialUFPSForum()
	{
		Application.OpenURL("http://www.visionpunk.com/hub/assets/ufps/forum");
	}

	[MenuItem("UFPS/Community/Unity Community Forum Thread", false, 208)]
	public static void UnityCommunityForumThread()
	{
		Application.OpenURL("http://www.visionpunk.com/hub/assets/ufps/unityforum");
	}

	/////////////////////////////////////////////////////////////////

	[MenuItem("UFPS/Check for Updates", false, 300)]
	public static void CheckForUpdates()
	{
		vp_UpdateDialog.Create("ufps", UFPSInfo.Version);
	}

}
