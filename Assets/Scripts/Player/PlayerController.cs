﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : GameActor 
{

	float hori = 0f;
	
	public Animator anim;


	bool jumpPressed = false;
	public float jumpPower;
	public float jumpDuration;
	float jumpTimeStart;



	bool onGround = false;
	public Transform groundCheck;
	float groundCheckRadius = 0.2f;
	public LayerMask groundLayer;
	
	public static GameObject GlobalPlayerInstance
	{
		get;
		private set;
	}

	void Start()
	{
		base.Start();
		jumpTimeStart = -1;
		if(GlobalPlayerInstance != null && GlobalPlayerInstance != this) {
			Destroy(gameObject);

		}

		GlobalPlayerInstance = gameObject;
	}

	void Update()
	{
		hori = Input.GetAxis ("Horizontal");
		if(onGround && Input.GetButtonDown("Jump")) {
			//TODO should only be able apply the jump force exactly once
			onGround = false;
			rigidbody2D.AddForce(new Vector2(0,jumpPower));
			jumpTimeStart = Time.time;				 
		}
	}

	override protected float horizontalMovingDir()
	{
		return hori;
	}

	override protected bool isStrafing()
	{
		return Input.GetButton("Strafe");
	}

	void FixedUpdate()
	{

		anim.SetFloat("horiSpeed",Mathf.Abs(hori));
		onGround = Physics2D.OverlapCircle(groundCheck.position,groundCheckRadius,groundLayer);
		anim.SetBool("onGround",onGround);



		if(Input.GetButton ("Jump") && jumpTimeStart > 0 && Time.time - jumpTimeStart <= jumpDuration) {
			rigidbody2D.AddForce (new Vector2(0,2*jumpPower*Time.fixedDeltaTime));
		}

		base.FixedUpdate();
	}

	override protected void Die()
	{
		hori = 0;
		rigidbody2D.velocity = Vector2.zero;
		currentHealth = maxHealth;
		transform.position = GameObject.Find("PlayerSpawn").transform.position;
	}


	





}
