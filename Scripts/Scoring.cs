using UnityEngine;
using System.Collections;

public class Scoring : MonoBehaviour {
	public const float CHECK_DELAY = 1;

	public LayerMask m_Mask;
	public int m_Team = 1;
	public GameObject m_BasketBall;
	private BoxCollider topCol;
	private BoxCollider bottomCol;

	private float waitTime = CHECK_DELAY;
	// Use this for initialization
	void Start () {
		topCol = GameObject.Find("TopCollison" + m_Team).GetComponent<BoxCollider>();
		bottomCol = GameObject.Find("BottomCollison" + m_Team).GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		//if( m_BasketBall.GetComponent<SphereCollider>().transform.
		if (waitTime > 0) {
			waitTime -= Time.deltaTime;
		}else{
				
			if (CheckTop () == true && CheckBottom () == true) {
				(GameObject.Find ("GlobalVariablesObj").GetComponent<GameObj> () as GameObj).Score (m_Team);
				GetComponent<AudioSource> ().Play ();
				waitTime = CHECK_DELAY;
			}
		}
	}
	
	private bool CheckTop(){
		Collider[] colliders = Physics.OverlapBox(topCol.transform.position, topCol.GetComponent<Renderer>().bounds.extents/2,Quaternion.identity,m_Mask);

		// Go through all the colliders...
		for (int i = 0; i < colliders.Length; i++) {
			Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();

			// If they don't have a rigidbody, go on to the next collider.
			if (!targetRigidbody)
				continue;
			return true;
		}
		return false;
	}

	private bool CheckBottom(){
		Collider[] colliders = Physics.OverlapBox(bottomCol.transform.position, bottomCol.GetComponent<Renderer>().bounds.extents/2,Quaternion.identity,m_Mask);

		// Go through all the colliders...
		for (int i = 0; i < colliders.Length; i++) {
			Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();

			// If they don't have a rigidbody, go on to the next collider.
			if (!targetRigidbody)
				continue;
			return true;
		}
		return false;
	}

}
