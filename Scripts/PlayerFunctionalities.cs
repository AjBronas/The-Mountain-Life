using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***
 *  Author: Aj Bronas
 *  Date : Feb, 2017
 * 
 * This script is made to enhance player controls by adding sprinting, crouching, 
 * 	prone and other possiblities
 * */

public class PlayerFunctionalities : MonoBehaviour {

	public const bool USE_HEALTH_EFFECTS = true;

	public const float SPRINT_MODIFIER = 2.5f;
	public const float CROUCH_MODIFIER = 0.50f;
	public const float PRONE_MODIFIER = 0.25f;

	public const float HEIGHT_STANDING = 2;
	public const float HEIGHT_CROUCHING = 1;
	public const float HEIGHT_PRONE = 0.5f;

	public const string SPRINT = "Sprint";
	public const string CROUCH = "Crouch";
	public const string IS_STANDING = "STANDING";
	public const string IS_CROUCHING = "CROUCHING";
	public const string IS_PRONE = "PRONE";

	public const float MAX_HEALTH = 100;
	public const float HEALTH_DECAY_RATE = 0.01F;
	public const float HUNGER_DECAY_RATE = 0.005F;
	public const float THIRST_DECAY_RATE = 0.01F;
	public const float FATIGUE_DECAY_RATE = 0.005F;
	public const float HUNGER_DECAY_DELAY = 300;
	public const float THIRST_DECAY_DELAY = 180;
	public const float FATIGUE_DECAY_DELAY = 600;

	[System.NonSerialized]
	public GameObject player;
	[System.NonSerialized]
	public CharacterMotor cm;
	[System.NonSerialized]
	public CharacterController cc;

	[System.NonSerialized]
	public bool isSprinting = false;
	[System.NonSerialized]
	public bool isCrouching = false;
	[System.NonSerialized]
	public bool isProne = false;

	[System.NonSerialized]
	public string status = IS_STANDING;
	private float forward_speed = 8;
	private float sideway_speed = 4;
	private float backward_speed = 4;
	private float acceleration = 30;

	public float health = 100;
	public float hunger = 100;
	public float thirst = 100;
	public float fatigue = 100;

	[System.NonSerialized]
	public bool isDead = false;
	public bool regenerateHealth = true;

	[System.NonSerialized]
	public float hunger_Timer = HUNGER_DECAY_DELAY;
	[System.NonSerialized]
	public float thirst_Timer = THIRST_DECAY_DELAY;
	[System.NonSerialized]
	public float fatigue_Timer = FATIGUE_DECAY_DELAY;


	void Start () {
		player = GameObject.Find ("player");
		cm = player.GetComponent<CharacterMotor> ();
		cc = player.GetComponent<CharacterController> ();
		forward_speed = cm.movement.maxForwardSpeed;
		sideway_speed = cm.movement.maxSidewaysSpeed;
		backward_speed = cm.movement.maxBackwardsSpeed;
		acceleration  = cm.movement.maxGroundAcceleration;
	}
	

	void Update () {

		// Sprinting
		if (!isCrouching && !isProne && status == IS_STANDING) {
			if (Input.GetButton (GlobalVariables.InputVariables.BTN_SPRINT)) {
				if (isSprinting == false){
					cm.movement.maxForwardSpeed = forward_speed * SPRINT_MODIFIER;
					cm.movement.maxSidewaysSpeed = sideway_speed * SPRINT_MODIFIER;
					cm.movement.maxBackwardsSpeed = backward_speed * SPRINT_MODIFIER;
					cm.movement.maxGroundAcceleration = acceleration * SPRINT_MODIFIER;
					GlobalVariables.PlayerVariables.isSprinting = true;
				}
				isSprinting = true;
				status = IS_STANDING;
				Debug.Log ("Sprinting");
			} else {
				if (isSprinting == true) {
					cm.movement.maxForwardSpeed = forward_speed;
					cm.movement.maxSidewaysSpeed = sideway_speed;
					cm.movement.maxBackwardsSpeed = backward_speed;
					cm.movement.maxGroundAcceleration = acceleration;
					GlobalVariables.PlayerVariables.isSprinting = false;
				}
				isSprinting = false;
			}


		}

		// Pose
		if (Input.GetButtonDown (GlobalVariables.InputVariables.BTN_CROUCH)) {
			if (status == IS_STANDING) {
				// Crouch
				setStatus (IS_CROUCHING);
			} else if (status == IS_CROUCHING) {
				// Prone
				setStatus (IS_PRONE);
			} else if (status == IS_PRONE) {
				// Stand
				setStatus (IS_STANDING);
			}
		}

		if (USE_HEALTH_EFFECTS && !isDead) {
			updateHealth ();
		}

	}

	public void updateHealth(){

		// check player condition
		if (hunger <= 0) {
			health -= HEALTH_DECAY_RATE;
		}
		if (thirst <= 0) {
			health -= HEALTH_DECAY_RATE;
		}
		if (fatigue <= 0) {
			health -= HEALTH_DECAY_RATE;
		}


		// decay health conditions if they havent been satisfied in their respected time
		if (hunger_Timer > 0) {
			hunger_Timer -= Time.deltaTime;
		} else {
			// start decaying hunger
			if (hunger > 0) {
				hunger -= HUNGER_DECAY_RATE;
			} else {
				hunger = 0;
			}
		}
		if (thirst_Timer > 0) {
			thirst_Timer -= Time.deltaTime;
		} else {
			// start decaying thirst
			if (thirst > 0) {
				thirst -= THIRST_DECAY_RATE;
			} else {
				thirst = 0;
			}
		}
		if (fatigue_Timer > 0) {
			fatigue_Timer -= Time.deltaTime;
		} else {
			// start decaying fatigue
			if (fatigue > 0) {
				fatigue -= FATIGUE_DECAY_RATE;
			} else {
				fatigue = 0;
			}
		}


		// if all status conditionals are satisfied, regenerate health
		if (regenerateHealth) {
			if (health >= MAX_HEALTH && thirst >= MAX_HEALTH && fatigue >= MAX_HEALTH) {
				if (health < MAX_HEALTH) {
					health += HEALTH_DECAY_RATE;
				}
				health = Mathf.Min (health, MAX_HEALTH);
			}
		}

		// If health is at 0, die
		if (health <= 0) {
			die ();
		}
	}

	public void eat( float amount ){
		hunger += amount;
		hunger = Mathf.Min (hunger, MAX_HEALTH);		// limit to 100
		hunger_Timer = HUNGER_DECAY_DELAY;
	}

	public void drink( float amount ){
		thirst += amount;
		thirst = Mathf.Min (thirst, MAX_HEALTH);		// limit to 100
		thirst_Timer = THIRST_DECAY_DELAY;
	}

	public void rest( float amount ){
		fatigue += amount;
		fatigue = Mathf.Min (fatigue, MAX_HEALTH);		// limit to 100
		fatigue_Timer = FATIGUE_DECAY_DELAY;
	}

	public void die(){
		isDead = true;
		Debug.Log ( this.name + " has died");
	}

	public void setStatus( string status ){
		this.status = status;
		if (status == IS_STANDING) {
			isCrouching = false;
			isProne = false;
			cc.height = HEIGHT_STANDING;
			cm.movement.maxForwardSpeed = forward_speed;
			cm.movement.maxSidewaysSpeed = sideway_speed;
			cm.movement.maxBackwardsSpeed = backward_speed;
			cm.movement.maxGroundAcceleration = acceleration;
		} else if (status == IS_CROUCHING) {
			isCrouching = true;
			isProne = false;
			cc.height = HEIGHT_CROUCHING;
			cm.movement.maxForwardSpeed = forward_speed * CROUCH_MODIFIER;
			cm.movement.maxSidewaysSpeed = sideway_speed * CROUCH_MODIFIER;
			cm.movement.maxBackwardsSpeed = backward_speed * CROUCH_MODIFIER;
			cm.movement.maxGroundAcceleration = acceleration * CROUCH_MODIFIER;
		} else if (status == IS_PRONE) {
			isCrouching = false;
			isProne = true;
			cc.height = HEIGHT_PRONE;
			cm.movement.maxForwardSpeed = forward_speed * PRONE_MODIFIER;
			cm.movement.maxSidewaysSpeed = sideway_speed * PRONE_MODIFIER;
			cm.movement.maxBackwardsSpeed = backward_speed * PRONE_MODIFIER;
			cm.movement.maxGroundAcceleration = acceleration * PRONE_MODIFIER;
		}
		Debug.Log (status);
	}
}
