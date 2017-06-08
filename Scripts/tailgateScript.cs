using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tailgateScript : MonoBehaviour {
	const float TIMER = 1;

	BoxCollider tailgate;
	BoxCollider[] stoppers;
	HingeJoint joint;
	Rigidbody body;

	bool firstRun = true;
	public static float m_Time = TIMER;

	void Start () {
		
		// Get truck colliders
		tailgate = this.GetComponentInChildren<BoxCollider> ();
		//Debug.Log ("Tailgate collider: " + tailgate.gameObject.name + "," + this.name + ", " + this.transform.parent.name);
		stoppers = this.transform.parent.transform.parent.FindChild ("Colliders").FindChild ("ColliderSides").GetComponentsInChildren<BoxCollider> ();
		//Debug.Log ("Stoppers collider: " + stoppers[0].gameObject.name);

		joint = this.GetComponent<HingeJoint> ();
		body = this.GetComponent<Rigidbody> ();
	}
	

	void Update () {
		Vector3 rot = this.transform.localEulerAngles;

		// Check if tailgate is up, if so then stop the motor and lock it in place
		if (joint.useMotor == true && rot.x > 79 && rot.x < 89){
			Debug.Log ("Tailgate Up" + rot.x);
			JointLimits limits = joint.limits;
			limits.min = 156;
			limits.bounciness = 0;
			limits.bounceMinVelocity = 0;
			limits.max = 158;
			joint.limits = limits;
			joint.useMotor = false;
			joint.useLimits = true;
		}

		// Check if tailgate is hitting a collider
		for( int i = 0; i < stoppers.Length; i++){
			if (tailgate.bounds.Intersects (stoppers [i].bounds)) {
				//Debug.Log ("Tailgate touching");
				if (firstRun) {
					/*joint.useMotor = false;
					this.GetComponent<Renderer> ().enabled = false;
					this.gameObject.SetActive (false);
					this.transform.parent.FindChild ("tailgateUP").gameObject.SetActive (true);
					firstRun = false;*/
				}
				//body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
			}
		}
	}
}
