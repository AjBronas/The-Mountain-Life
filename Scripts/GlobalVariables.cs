using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class GlobalVariables {

	/*Use this to keep record of variables such as score, time, etc*/
	public static bool mouseLock = false;
	public static int m_Score1 = 0;
	public static float m_Time = 60;
	public static bool inVehicle = false;
	public static bool hadWeaponEquipped = false;		// this is for when the player exits and enters a vehicle
	public static GameObject player;
	public static Transform defaultPlayerTransform;

	public static class PlayerVariables{
		public static bool isSprinting = false;
	}

	public static class InputVariables{
		public const string BTN_INTERACT = "Interact";
		public const string BTN_PACK 	= "Pack";
		public const string BTN_GRAB 	= "Grab";
		public const string BTN_DROP 	= "Drop";
		public const string BTN_SPRINT 	= "Sprint";
		public const string BTN_CROUCH 	= "Crouch";
		public const string BTN_RELOAD 	= "Reload";
	}


	public static void Start () {
		player = GameObject.Find ("player");
		defaultPlayerTransform = player.transform.parent;
		Debug.Log("Found player");
	}

	public static void setDriving( bool driving ){
		inVehicle = driving;
		CharacterMotor cm = player.GetComponent<CharacterMotor> ();
		CharacterController cc = player.GetComponent<CharacterController> ();
		CarController car = GameObject.Find ("Truck").GetComponent<CarController> ();
		car.StopDriving ();
		if (driving) {
			hadWeaponEquipped = player.GetComponent<Weapon> ().weapon.weaponEquiped;
			toggleWeapon (false);
		}else{
			toggleWeapon (hadWeaponEquipped);
		}

		if (driving) {
			player.transform.parent = GameObject.Find("Truck").transform;
			GameObject.Find ("Truck").GetComponent<VehicleStop> ().awake ();
		} else {
			GameObject.Find ("Truck").GetComponent<VehicleStop> ().setPosition ();
			GameObject.Find ("Truck").GetComponent<VehicleStop> ().setRotation ();
			GameObject.Find ("Truck").GetComponent<VehicleStop> ().reset();
			player.transform.parent = defaultPlayerTransform;
		}
		cm.enabled = !driving;
		cc.enabled = !driving;
	}

	public static void lockMouse( bool locked ){
		mouseLock = locked;
		if( player != null )
			player.GetComponent<MouseLook> ().setLockMouse (locked);
	}

	public static void toggleWeapon( bool on ){
		player.GetComponent<Weapon> ().toggleWeapon (on);
		for( int i = 0; i < Camera.main.transform.childCount; i++ ){
			if (Camera.main.transform.GetChild (i).tag == "Weapon") {
				//Debug.Log ("Hiding " + Camera.main.transform.GetChild (i).name);
				Camera.main.transform.GetChild (i).gameObject.SetActive (on);
			}
			if (Camera.main.transform.GetChild (i).tag == "Arms") {
				//Debug.Log ("Hiding " + Camera.main.transform.GetChild (i).name);
				Camera.main.transform.GetChild (i).gameObject.SetActive (on);
			}
		}
	}

	/*
	public static void Score(int team){
		if (team == 1)
			m_Score1++;
		else
			m_Score2++;

		GameObject score = GameObject.Find ("ScreenCanvas");
		Text scoreText = score.GetComponent<Text>();
		if(scoreText != null)
			scoreText.text = "Team1: " + m_Score1 + "     Team2: " + m_Score2;
	}
	*/
}
