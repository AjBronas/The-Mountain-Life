using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGravity : MonoBehaviour {

	public bool enabled = true;
	public bool disableVelocity = true;
	private float size = 100;

	private Transform transform;
	private BoxCollider collider;

	// Use this for initialization
	void Start () {
		transform = this.gameObject.transform;
		collider = this.gameObject.GetComponent<BoxCollider> ();

		size = collider.bounds.size.y / 1.95f;

	}
	
	// Update is called once per frame
	void Update () {

		if (disableVelocity) {
			Rigidbody body = this.gameObject.GetComponent<Rigidbody> ();
			if (body != null) {
				body.velocity = Vector3.zero;
				body.angularVelocity = Vector3.zero;
			}
		}
		//Debug.Log ("Draw" + transform.position);
		if (enabled && collider != null && transform != null) {

			Vector3 position = this.gameObject.transform.position;
			//position = new Vector3 (position.x, position.y, position.z);			

			RaycastHit rayHit;
			Vector3 target = position + new Vector3(0,-1,0) * (size);
			Debug.DrawLine (position, target, Color.red);
			//Debug.Log ("Draw2" + size);
			if (!Physics.Linecast (position, target, out rayHit) && rayHit.collider != collider){
				Debug.Log ("Draw3" + "falling");
				gameObject.transform.Translate(Vector3.up * Time.deltaTime * Physics.gravity.y/4, Space.World);
				//this.gameObject.transform.position.Set (position.x, position.y - 0.15f, position.z);
			}
		}

	}
}
