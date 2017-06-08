using UnityEngine;
using System.Collections;

public class AddDelayPhysics : MonoBehaviour {

	public float m_Time = 1;
	public float m_Drag = 0.1f;
	public float m_AngularDrag = 0.1f;
	private bool timeRunning = true;
	//private bool tempPhysics = false;
	//private float tempTime = 0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (timeRunning) {
			if (m_Time > 0) {
				m_Time -= Time.deltaTime;
			} else {
				timeRunning = false;
				m_Time = 0;
				AddPhysics ();
				enabled = false;
			}
		} 
		
	}

	void AddPhysics(){

		if(gameObject.GetComponent<Rigidbody>() == null)
			gameObject.AddComponent<Rigidbody>();
		
		gameObject.GetComponent<Rigidbody> ().drag = m_Drag;
		gameObject.GetComponent<Rigidbody> ().angularDrag = m_AngularDrag;
		gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		GameObject.FindObjectOfType<WallBuilderUtil> ().brickLoadedCount++;
		//if(gameObject.GetComponent<CheckPosition> () != null)
			//gameObject.GetComponent<CheckPosition> ().m_isEnabled = true;
	}

	/*
	void MakePhysical( float time ){

		if(gameObject.GetComponent<Rigidbody>() == null)
			gameObject.AddComponent<Rigidbody>();

		gameObject.GetComponent<Rigidbody> ().isKinematic = false;
		tempPhysics = true;
		tempTime = time;
		m_Time = time;
		tempUpdater ();
	}

	void tempUpdater(){
		if (tempPhysics) {
			if (m_Time > 0) {
				m_Time -= Time.deltaTime;
				tempUpdater ();
			} else {
				tempPhysics = false;
				m_Time = 0;
				tempTime = 0;
				gameObject.GetComponent<Rigidbody> ().isKinematic = false;
			}
		}
	}
	*/

}
