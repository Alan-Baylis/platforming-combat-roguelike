using UnityEngine;
using System.Collections;

public class DOTBlast : CloseBlast
{
	
	public int intervals;

	override public void burstEffect(Transform blast, Transform target)
	{
		GameActor actor;
		if(actor = target.GetComponent<GameActor>()) {
			actor.ApplyDOT(effectSize,intervals);
			actor.SetColor(Color.red);
		}
	}




}

