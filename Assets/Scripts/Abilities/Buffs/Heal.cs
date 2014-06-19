using UnityEngine;
using System.Collections;

public class Heal : Buff
{

	override public void buffEffect(Transform applier, Transform target)
	{
		GameActor actor;
		if(actor = target.GetComponent<GameActor>())
			actor.currentHealth = Mathf.Clamp (actor.currentHealth + effectSize,0,actor.maxHealth);
	}

	override public void undoBuff(Transform applier, Transform target)
	{

	}
	

	
	override public void passiveEffect(Transform player)
	{

	}

}

