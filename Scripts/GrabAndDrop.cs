using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GrabAndDrop : MonoBehaviour {

	public Camera m_Camera;
	public float cameraHeight = 2;
	public float m_range = 2;
	private GameObject grabbedObject;
	private float grabbedObjSize;
	public GameObject m_Model;

	public Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
	public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held.
	public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
	public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.

	private GrabAndDrop m_GetScript;
	private string m_FireButton;                // The input axis that is used for launching shells.
	private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
	private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
	private bool m_Fired;                       // Whether or not the shell has been launched with this button press.
	public float m_arc = 6;

	private bool isBox = false;

	private bool[] activeBoxes;
	private bool[] activeSpheres;
	private bool[] activeBodies;
	private bool activeBody;
	private Renderer renderer;
	// Use this for initialization
	void Start () {
	
		if (m_Camera == null)
			m_Camera = Camera.main;

		m_CurrentLaunchForce = m_MinLaunchForce;
		m_AimSlider.value = m_MinLaunchForce;
		m_FireButton = "Fire1";
		m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

		/*AddModel modelScript = GetComponent<AddModel> ();
		m_Model = modelScript.m_Model;*/
		
	}

	GameObject GetMouseHoverObject( float range ){

		Vector3 position = m_Camera.transform.position;
		//position = new Vector3 (position.x, position.y, position.z);			
			
		RaycastHit rayHit;
		Vector3 target = m_Camera.transform.position + m_Camera.transform.forward * range;
		if (Physics.Linecast (position, target, out rayHit))
			return rayHit.collider.gameObject;
		return null;
	}

	void TryGrabObject( GameObject grabObject ){

		if( grabObject == null || !CanGrab(grabObject) )
			return;
		grabObject = grabObject.transform.root.gameObject;
		Debug.Log ("Grabbing..." + grabObject.name);
		if (grabObject.GetComponent<BoxCollider> () != null)
			isBox = true;
		else
			isBox = false;
		Debug.Log ("Grabbing..." + grabObject.name);
		renderer = grabObject.GetComponent<Renderer> ();
		Renderer skinnedRenderer = grabObject.GetComponent<SkinnedMeshRenderer> ();
		if (renderer == null) {
			renderer = grabObject.GetComponentInChildren<Renderer> ();
		}
		if (skinnedRenderer == null) {
			skinnedRenderer = grabObject.GetComponentInChildren<SkinnedMeshRenderer> ();
		}
		if (renderer == null)
			renderer = skinnedRenderer;
		if (renderer == null)
			return;


		Debug.Log ("Grabbing...");
		grabbedObject = grabObject;
		grabbedObjSize = renderer.bounds.size.magnitude;
		m_Fired = false;

		Rigidbody body = grabbedObject.GetComponent<Rigidbody> ();
		Rigidbody[] bodies = grabbedObject.GetComponentsInChildren<Rigidbody> ();
		activeBodies = new bool[bodies.Length];
		if (body != null && body.detectCollisions == true) {
			body.detectCollisions = false;
			body.useGravity = false;
			activeBody = true;
		}
		for (int i = 0; i < bodies.Length; i++) {
			if ( bodies[i].detectCollisions == true ){
				bodies [i].detectCollisions = false;
				body.useGravity = false;
				activeBodies[i] = true;
			}
		}
		if (isBox) {
			//grabbedObject.GetComponent<BoxCollider> ().enabled = false;
			BoxCollider[] boxes = grabbedObject.GetComponentsInChildren<BoxCollider> ();
			activeBoxes = new bool[boxes.Length];
			for (int i = 0; i < boxes.Length; i++) {
				if (boxes [i].enabled == true) {
					boxes [i].enabled = false;
					activeBoxes [i] = true;
				}
			}
			SphereCollider[] spheres = grabbedObject.GetComponentsInChildren<SphereCollider> ();
			activeSpheres = new bool[spheres.Length];
			for (int i = 0; i < spheres.Length; i++) {
				if (spheres [i].enabled == true) {
					spheres [i].enabled = false;
					activeSpheres [i] = true;
				}
			}
		} else{
			//grabbedObject.GetComponent<SphereCollider> ().enabled = false;
			BoxCollider[] boxes = grabbedObject.GetComponentsInChildren<BoxCollider> ();
			activeBoxes = new bool[boxes.Length];
			for (int i = 0; i < boxes.Length; i++) {
				if (boxes [i].enabled == true) {
					boxes [i].enabled = false;
					activeBoxes [i] = true;
				}
			}
			SphereCollider[] spheres = grabbedObject.GetComponentsInChildren<SphereCollider> ();
			activeSpheres = new bool[spheres.Length];
			for (int i = 0; i < spheres.Length; i++) {
				if (spheres [i].enabled == true) {
					spheres [i].enabled = false;
					activeSpheres [i] = true;
				}
			}

		}

		for (int i = 0; i < activeBoxes.Length; i++) {
			Debug.Log ("Boxes:" + activeBoxes [i]);
		}

	}

	bool CanGrab( GameObject canidate ){
		return canidate.GetComponent<Rigidbody> () != null;
	}

	void DropObject(){

		if (grabbedObject == null)
			return;
		Rigidbody body = grabbedObject.GetComponent<Rigidbody> ();
		Rigidbody[] bodies = grabbedObject.GetComponentsInChildren<Rigidbody> ();
		activeBodies = new bool[bodies.Length];
		if ( activeBody == true ) {
			body.detectCollisions = true;
			body.useGravity = true;
			body.velocity = Vector3.zero;
		}
		for (int i = 0; i < bodies.Length; i++) {
			if ( activeBodies[i] == true ){
				bodies [i].detectCollisions = true;
				body.useGravity = true;
				bodies[i].velocity = Vector3.zero;
			}
		}

		if (grabbedObject.GetComponent<Rigidbody> () != null)
			grabbedObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		if (isBox) {
			grabbedObject.GetComponent<BoxCollider> ().enabled = true;
			BoxCollider[] boxes = grabbedObject.GetComponentsInChildren<BoxCollider> ();
			for (int i = 0; i < boxes.Length; i++) {
				if (activeBoxes [i] == true) {
					boxes [i].enabled = true;
				}
			}
			SphereCollider[] spheres = grabbedObject.GetComponentsInChildren<SphereCollider> ();
			for (int i = 0; i < spheres.Length; i++) {
				if (activeSpheres [i] == true) {
					spheres [i].enabled = true;
				}
			}
		} else {
			grabbedObject.GetComponent<SphereCollider> ().enabled = true;
			BoxCollider[] boxes = grabbedObject.GetComponentsInChildren<BoxCollider> ();
			for (int i = 0; i < boxes.Length; i++) {
				if (activeBoxes [i] == true) {
					boxes [i].enabled = true;
				}
			}
			SphereCollider[] spheres = grabbedObject.GetComponentsInChildren<SphereCollider> ();
			for (int i = 0; i < spheres.Length; i++) {
				if (activeSpheres [i] == true) {
					spheres [i].enabled = true;
				}
			}
		}
		grabbedObject = null;

	}

	// Update is called once per frame
	void Update () {

		m_AimSlider.value = m_MinLaunchForce;

		if (Input.GetButtonDown("Grab")) {
			Debug.Log ("grab");
			if (grabbedObject == null)
				TryGrabObject (GetMouseHoverObject (m_range));
			else
				DropObject ();

		}

		if (grabbedObject != null) {
			Vector3 newPos = gameObject.transform.position + m_Camera.transform.forward * grabbedObjSize;
			newPos = new Vector3 (newPos.x, newPos.y+cameraHeight, newPos.z);
			grabbedObject.transform.position = newPos;

			if (Input.GetButton(m_FireButton) && !m_Fired) {

				m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
				m_AimSlider.value = m_CurrentLaunchForce;
				if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired){
					m_CurrentLaunchForce = m_MaxLaunchForce;
					ThrowObject();
				}
			} else if (Input.GetButtonUp (m_FireButton) && !m_Fired){
				ThrowObject();
			}
		}
			
	}

	void ThrowObject(){

		m_Fired = true;

		if (grabbedObject == null)
			return;


		if (grabbedObject.GetComponent<Rigidbody> () != null) {
			Vector3 vel =  (m_CurrentLaunchForce * m_Camera.transform.forward);//m_Model.GetComponent<Rigidbody>().velocity + (m_CurrentLaunchForce * m_Camera.transform.forward)
			vel.y += m_arc;

			grabbedObject.GetComponent<Rigidbody> ().velocity = vel;
			if( isBox )
				grabbedObject.GetComponent<BoxCollider> ().enabled = true;
			else
				grabbedObject.GetComponent<SphereCollider> ().enabled = true;
		}
		grabbedObject = null;
		m_CurrentLaunchForce = m_MinLaunchForce;
	}
}
