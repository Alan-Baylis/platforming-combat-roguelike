using UnityEngine;
using System.Collections;
using System;

public class ProjectileAttack : Ability
{

	protected ObjectPool projectiles;
	public GameObject projectileRep;
	protected Transform channeler;
	public float bulletVelocity = 1000;
	public Action<Transform> onDestroy;
	public Action<Transform, Transform> onCollision;
	public LayerMask onCollisionTargets;
	
	public void Start()
	{
		projectiles = ObjectPool.GetPoolByRepresentative(projectileRep);
		onDestroy = null;
		onCollision = defaultCollision;
		if(upgrade != null) {
			print(gameObject.name + " upgrading");
			upgradeAbility(upgrade);
			print(bulletVelocity);
		}
	}


	override sealed public void activeEffect(Transform player)
	{
		GameObject bullet = projectiles.getPooled();
		bullet.SetActive(true);
		bullet.SendMessage("IgnoreCollider",player.collider2D);
		fireProjectile(bullet,player);
	}

	protected virtual void fireProjectile(GameObject bullet, Transform player)
	{
		if(channeler == null) {
			channeler = player.FindChild("channeler");
		}

		bullet.SendMessage("SetOnDestroy",new UpgradeAction(onDestroy));
		bullet.SendMessage("SetOnCollision",new UpgradeAction(onCollision,onCollisionTargets));
		bullet.transform.position = channeler.position;
		bullet.transform.rotation = channeler.rotation;
		bullet.rigidbody2D.AddForce(player.right*Mathf.Sign(player.localScale.x)*bulletVelocity);
	}

	public void defaultCollision(Transform projectile, Transform target)
	{
		target.SendMessage("Damage",effectSize);
	}

	public void defaultDestroy(Transform projectile)
	{

	}
	
	override protected void upgradeOtherAbility(Ability projectile)
	{
		
	}
	
	override public void passiveEffect(Transform player)
	{
		
	}

}

