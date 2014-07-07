using UnityEngine;
using System.Collections;
using System;

public abstract class CloseBlast : Ability
{

	public GameObject blastObj;
	public GameObject BlastObj
	{
		get
		{
			return blastObj;
		}
		set
		{
			blastObj = value;
			blastPool = PoolManager.Instance.GetPoolByRepresentative(blastObj);
		}
	}
	protected ObjectPool blastPool;
	public bool attachedToFirer = false;
	public Action<Transform, Transform> onHitByBurst;
	public Action<Transform, Transform> blastFunc;
	public float blastDelay;
	public float blastSize = 1;
	public LayerMask burstTargets;

	protected Transform channeler;

	private GameObject origBlastObj;
	private float origBlastDelay;
	private LayerMask origBurstTargets;

	int passiveActionID = 0;

	void Start()
	{
		base.Start();
		if(blastObj != null) {
			blastPool = PoolManager.Instance.GetPoolByRepresentative(blastObj);
		}
		onHitByBurst = burstEffect;
		blastFunc = createBlast;

		origBlastObj = blastObj;
		origBlastDelay = blastDelay;
		origBurstTargets = burstTargets;
	}

	public abstract void burstEffect(Transform blast, Transform target);

	override public void activeEffect(Transform player)
	{
		if(channeler == null) {
			channeler = player.FindChild("channeler");
		}
		blastFunc(channeler,player);
	}

	GameObject getBlast()
	{
		GameObject blast = blastPool.getPooled();
		blast.SetActive(true);
		blast.SendMessage ("StartDelay",blastDelay);
		blast.SendMessage("SetBlastEffect",new UpgradeAction(onHitByBurst,burstTargets));
		scaleBlast(blast,blastSize);
		return blast;
	}

	protected virtual void createBlast(Transform location, Transform player)
	{
		GameObject blast = getBlast();
		blast.transform.position = location.position;
		blast.transform.right = player.right*Mathf.Sign (player.localScale.x);
		if(attachedToFirer)
			blast.SendMessage("SetOwner",player.GetComponent<GameActor>());
	}

	void createBlastFromProjCollision(Transform proj, Transform target)
	{
		GameObject blast = getBlast();
		blast.transform.position = target.position;
		blast.transform.right = (proj.position - target.position).normalized;
	}

	protected virtual void scaleBlast(GameObject blast, float scaling)
	{
		Vector3 scale = blast.transform.localScale;
		blast.transform.localScale = scaling*scale;
	}
	
	override protected void upgradeOtherAbility(Ability other)
	{
		print(abilityName + " upgrading " + other.abilityName);
		if(other.GetType().BaseType == typeof(ProjectileAttack)) {
			ProjectileAttack pa = (ProjectileAttack)other;
			pa.onCollision = createBlastFromProjCollision;
			pa.onCollisionTargets = burstTargets;
		} else if(other.GetType().BaseType == typeof(CloseBlast)) {
			CloseBlast cb = (CloseBlast)other;
			cb.onHitByBurst = onHitByBurst;
			cb.burstTargets = burstTargets;
		} else if(other.GetType().BaseType == typeof(Buff)) {
			Buff b = (Buff)other;
			onHitByBurst = b.buffEffect;
			b.activeFunc = activeEffect;
		} else if(other.GetType().BaseType == typeof(Special)) {
			if(other.GetType() == typeof(Dash)) {
				Dash d = (Dash)other;
				d.preDashAction = activeEffect;
			} else if(other.GetType() == typeof(ClusterShower)) {
				ClusterShower cs = (ClusterShower)other;
				cs.onCollision = createBlast;
				cs.onCollisionTargets = burstTargets;
			}  else if(other.GetType() == typeof(OrbGenerator)) {
				OrbGenerator og = (OrbGenerator)other;
				og.onCollision = createBlast;
				og.onCollisionTargets = burstTargets;
			}
		}
	}

	void upgradeProjectileAttack(ProjectileAttack pa)
	{
		pa.onCollision = burstEffect;
	}

	override protected void reset()
	{
		BlastObj = origBlastObj;
		onHitByBurst = burstEffect;
		blastFunc = createBlast;
		blastDelay = origBlastDelay;
		burstTargets = origBurstTargets;
	}

	override public void applyPassive(Transform player)
	{
		ActionOnHit action = player.gameObject.AddComponent<ActionOnHit>();
		action.init(burstEffect,burstTargets,cooldown);
		passiveActionID = action.GetInstanceID();
	}
	
	override public void undoPassive(Transform player)
	{
		ActionOnHit[] actions = player.gameObject.GetComponents<ActionOnHit>();
		ActionOnHit action = null;
		foreach(ActionOnHit a in actions)
			if(a.GetInstanceID() == passiveActionID)
				action = a;
		if(action)
			Destroy(action);
	}


}

