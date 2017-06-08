using UnityEngine;
using System.Collections;

public class CheckPosition : MonoBehaviour {

	public float m_MinHeight = 0;
	public float m_MaxHeight = 100;
	public float resetHeight = 1;
	public bool useSize = true;
	public float maxVelocity = 15;
	public float minVelocity = 0.0001f;
	public bool m_isEnabled = true;
	private Vector3 startPosition;
	private int resetCount = 0;
	private int completeResetCount = 0;
	private bool canCoRoutine = true;
	//private Vector3 startVelocity;
	//private Vector3 startAngVelocity;

	// Use this for initialization
	void Start () {
		startPosition = transform.position;
		//if (GetComponent<Rigidbody> () != null) {
		//	startVelocity = GetComponent<Rigidbody> ().velocity;
		//	startAngVelocity = GetComponent<Rigidbody> ().angularVelocity;
		//}
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<Rigidbody> () != null) {
			if (m_isEnabled == true) {
				if (shouldReset ())
					resetPosition ();
				Rigidbody body = GetComponent<Rigidbody> ();
				//body.velocity = Vector3.ClampMagnitude (body.velocity, maxVelocity);
				//Debug.Log ("Velocity: " + body.velocity.magnitude);
				if (body.velocity.magnitude < minVelocity) {
					body.velocity = Vector3.zero;
					body.angularVelocity = Vector3.zero;
				}
				gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
			}
		}
	}

	bool shouldReset(){
		return transform.position.y < m_MinHeight || transform.position.y > m_MaxHeight;
	}

	void resetPosition(){

		if( resetCount > 5 ){
			resetPositionCompletely ();
			return;
		}

		if (useSize) {
			gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			transform.position = new Vector3 (transform.position.x, transform.GetComponent<Renderer> ().bounds.size.magnitude, transform.position.z);
		} else {
			gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			transform.position = new Vector3 (transform.position.x, resetHeight, transform.position.z);
		}

		Debug.Log (gameObject.name + " was reset. " + resetCount);
		resetCount++;
		if(canCoRoutine)
			StartCoroutine("resetCountFromTime");
	}

	void resetPositionCompletely(){

		StopCoroutine("resetCountFromTime");
		gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		transform.position = startPosition;
		if (shouldReset () || completeResetCount > 1) {
			destroy ();
		}
		resetCount = 0;
		completeResetCount++;
	}

	IEnumerator resetCountFromTime() {
		int tempCount = completeResetCount;

		Debug.Log (gameObject.name + " resetting count in 5 sec..");
		canCoRoutine = false;

		yield return new WaitForSeconds(5f);

		if (completeResetCount > tempCount)
			destroy ();
		resetCount = 0;

		canCoRoutine = true;
	}

	void destroy(){

		Debug.Log (gameObject.name + " was removed.");
		Destroy (gameObject);
	}

}
