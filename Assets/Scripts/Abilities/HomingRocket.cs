using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HomingRocket : ProjectileAttack 
{

	public GameObject explosionRep;
	ObjectPool explosionPool;
	public float explosionSize;
	public LayerMask explosionTargets;
	public Action<Transform, Transform> blastEffect;

	void Start()
	{
		base.Start ();
		if(explosionPool == null && explosionRep != null) {
			explosionPool = ObjectPool.GetPoolByRepresentative(explosionRep);
		}
		blastEffect = defaultBlast;
	}

	override protected void fireProjectile(GameObject bullet, Transform player)
	{
		if(channeler == null) {
			channeler = player.FindChild("channeler");
		}
		bullet.transform.position = channeler.position;
		bullet.rigidbody2D.velocity = player.right*Mathf.Sign (player.localScale.x);
		bullet.SendMessage("SetOnDestroy",new UpgradeAction(createExplosion));
		bullet.SendMessage("SetOnCollision",new UpgradeAction(onCollision,onCollisionTargets));


	}

	public void createExplosion(Transform projectile)
	{
		if(explosionPool != null) {
			GameObject explosion = explosionPool.getPooled();
			explosion.SetActive (true);
			explosion.transform.position = projectile.position;
			explosion.transform.rotation = projectile.rotation;
			Vector3 scale = explosion.transform.localScale;
			scale = explosionSize*scale;
			explosion.transform.localScale = scale;
			explosion.SendMessage("SetBlastEffect",new UpgradeAction(blastEffect,explosionTargets));
			explosion.SendMessage("StartDelay",0f);
		}
	}

	public void defaultBlast(Transform projectile, Transform target)
	{
		target.SendMessage("Damage",effectSize);
		if(target.rigidbody2D != null) {
			Vector3 forceDir = target.position - projectile.position;
			Vector3 force = forceDir.normalized*effectSize*10;//*1/Mathf.Pow(forceDir.magnitude,2);
			target.rigidbody2D.AddForce(force);
		}
	}

	override protected void upgradeOtherAbility(Ability other)
	{
		if(other.GetType().BaseType != typeof(Ability)) {
			if(other.GetType().BaseType == typeof(ProjectileAttack)) {
				ProjectileAttack pa = (ProjectileAttack)other;
				pa.onDestroy = createExplosion;
			}
		}
	}
	
	override public void passiveEffect(Transform player)
	{

	}

}