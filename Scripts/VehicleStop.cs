using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleStop : MonoBehaviour {

	const float TIMER = 3;

	public Vector3 orig_Position;
	public Vector3 orig_Rotation;
	public Rigidbody body;
	private CarController controller;

	public bool firstRun = false;
	public static float m_Time = TIMER;

	// Use this for initialization
	void Start () {
		body = this.GetComponent<Rigidbody> ();
		controller = this.GetComponent<CarController> ();
		setRotation ();
	}

	public void setPosition(){
		orig_Position = this.transform.position;
	}

	public void setRotation(){
		orig_Rotation = this.transform.localEulerAngles;
	}

	public void reset(){
		this.transform.position = orig_Position;
		this.transform.localEulerAngles = orig_Rotation;
		body.velocity = Vector3.zero;
		body.angularVelocity = Vector3.zero;
		//body.Sleep ();
		Rigidbody[] bodies = this.GetComponentsInChildren<Rigidbody> ();
		//Debug.Log ("bodies:" + bodies.Length);
		/*for (int i = 0; i < bodies.Length; i++) {
			bodies [i].velocity = Vector3.zero;
			bodies [i].angularVelocity = Vector3.zero;
			bodies [i].Sleep ();
			//Debug.Log ("VEL:" + bodies [i].gameObject.name);
		}*/
		this.transform.FindChild ("WheelsHubs").gameObject.SetActive (false);
		this.transform.FindChild ("WheelsHubs").gameObject.SetActive (true);
		controller.AccelInput = 0;
		controller.BrakeInput = 1000;
	}

	public void awake(){
		body.WakeUp();
		Rigidbody[] bodies = this.GetComponentsInChildren<Rigidbody> ();
		for (int i = 0; i < bodies.Length; i++) {
			bodies [i].WakeUp ();
		}
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log ("Vel: " + body.velocity);
		if (m_Time > 0) {
			m_Time -= Time.deltaTime;
		}else{
			if (firstRun == false) {
				setPosition ();
				firstRun = true;
			}
			if (body.velocity.y < 1 && body.velocity.y > -1 && GlobalVariables.inVehicle == false) {
				Debug.Log ("Resetting vehicle " + this.name);
				reset ();
				m_Time = 60;
			}
		}
	}
}
