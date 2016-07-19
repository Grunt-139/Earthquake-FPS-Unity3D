using UnityEngine;
using System.Collections;

public class ArmourPickup : vp_Pickup {

	public float armour = 0.1f;

	protected override bool TryGive (vp_FPPlayerEventHandler player)
	{
		if(player.Armour.Get() == 1.0f)
			return false;

		player.Armour.Set(Mathf.Min(1,player.Armour.Get() + armour));
		return true;
	}
}
