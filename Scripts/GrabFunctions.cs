using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrabFunctions : MonoBehaviour {

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
	private Vector3 newPos = Vector3.zero;
	//private bool isBox = false;

	Collider[] colliders;
	private bool[] activeBoxes;
	//private bool[] activeSpheres;
	private bool[] activeBodies;
	private bool activeBody;
	private Renderer renderer;

	Weapon weaponScript;

	public bool isAnimal = false;
	void Start () {

		if (m_Camera == null)
			m_Camera = Camera.main;

		m_CurrentLaunchForce = m_MinLaunchForce;
		m_AimSlider.value = m_MinLaunchForce;
		m_FireButton = "Fire1";
		m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

		weaponScript = GetComponent<Weapon> ();
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
		grabbedObject = grabObject;
		if (grabObject.tag == "Animal") {
			isAnimal = true;
			Debug.Log ("GRAB->" + grabObject.name);
			for (int i = 0; i < grabObject.transform.childCount; i++) {
				Debug.Log ("   Child[" + i + "]->" + grabObject.transform.GetChild (i).name);
			}
			activateRagdoll (false);
		} else {
			isAnimal = false;
		}
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

		grabbedObjSize = renderer.bounds.size.magnitude;
		m_Fired = false;



		/*
		Rigidbody body = grabbedObject.GetComponent<Rigidbody> ();
		Rigidbody[] bodies = grabbedObject.GetComponentsInChildren<Rigidbody> ();
		activeBodies = new bool[bodies.Length];
		if (body != null && body.detectCollisions == true) {
			//body.detectCollisions = false;
			//body.useGravity = false;
			activeBody = true;
		}
		for (int i = 0; i < bodies.Length; i++) {
			if ( bodies[i].detectCollisions == true ){
				//bodies [i].detectCollisions = false;
				//body.useGravity = false;
				activeBodies[i] = true;
			}
		}*/

		//grabbedObject.GetComponent<BoxCollider> ().enabled = false;
		//FindObjectsOfType(typeof(HingeJoint)) as HingeJoint[]
		if( isAnimal )
			colliders = grabbedObject.transform.FindChild("deer_ragdoll").transform.FindChild("Armature").gameObject.GetComponentsInChildren<Collider>();
		else
			colliders = grabbedObject.gameObject.GetComponentsInChildren<Collider>();
		Debug.Log("colliders: " + colliders.Length);
		activeBoxes = new bool[colliders.Length];
		for (int i = 0; i < activeBoxes.Length; i++) {
			activeBoxes [i] = false;
		}
			
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders [i].enabled == true) {
				colliders [i].enabled = false;
				activeBoxes [i] = true;
			}
		}


		for (int i = 0; i < activeBoxes.Length; i++) {
			Debug.Log ("Boxes["+i+"]:" + activeBoxes [i]);
		}

	}

	bool CanGrab( GameObject canidate ){
		return canidate.transform.root.GetComponent<Rigidbody> () != null;
	}

	void DropObject(){

		if (grabbedObject == null)
			return;

		if (isAnimal)
			activateRagdoll (true);
		
		Rigidbody body = grabbedObject.GetComponent<Rigidbody> ();
		Rigidbody[] bodies = grabbedObject.GetComponentsInChildren<Rigidbody> ();
		/*body.velocity = Vector3.zero;
		activeBodies = new bool[bodies.Length];
		if ( activeBody == true ) {
			//body.detectCollisions = true;
			//body.useGravity = true;
			body.velocity = Vector3.zero;
		}
		for (int i = 0; i < bodies.Length; i++) {
			if ( activeBodies[i] == true ){
				//bodies [i].detectCollisions = true;
				//body.useGravity = true;
				bodies[i].velocity = Vector3.zero;
			}
		}
		*/

		grabbedObject.transform.Translate(Vector3.up * 0.25f, Space.World);
		if (grabbedObject.transform.position.y < (gameObject.transform.position.y-gameObject.GetComponent<CharacterController>().height)) {
			Debug.Log (grabbedObject.transform.position.y + " | " + gameObject.transform.position.y);
			//grabbedObject.transform.Translate(Vector3.up * 0.25f, Space.World);
			grabbedObject.transform.position = new Vector3(grabbedObject.transform.position.x, (gameObject.transform.position.y-gameObject.GetComponent<CharacterController>().height), grabbedObject.transform.position.z);
		}
		if (grabbedObject.GetComponent<Rigidbody> () != null)
			grabbedObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;

		for (int i = 0; i < colliders.Length; i++) {
			if (activeBoxes [i] == true) {
				colliders [i].enabled = true;
			}
		}

		
		grabbedObject = null;

	}

	// Update is called once per frame
	void Update () {

		if (GlobalVariables.inVehicle) {
			if (grabbedObject != null)
				DropObject ();
		} else {
			m_AimSlider.value = m_MinLaunchForce;

			/* Grab item infront of camera */
			if (Input.GetButtonDown (GlobalVariables.InputVariables.BTN_GRAB)) {
				Debug.Log ("grab");
				if (grabbedObject == null)
					TryGrabObject (GetMouseHoverObject (m_range));
				else
					DropObject ();

			}

			/* Equip grabbed item/weapon */
			if (!weaponScript.weapon.weaponEquiped && grabbedObject != null) {
				if (Input.GetButtonDown (GlobalVariables.InputVariables.BTN_PACK)) {
					// Equip weapon
					EquipGrabbedItem (grabbedObject);
				}
			}

			/* Grab equipped item/weapon */
			if (weaponScript.weapon.weaponEquiped && grabbedObject == null) {
				if (Input.GetButtonDown (GlobalVariables.InputVariables.BTN_PACK)) {
					// Grab weapon
					GrabEquippedItem ();
				}
			}

			if (grabbedObject != null) {
				if (grabbedObject.tag == "Weapon")
					grabbedObjSize = 1;
				newPos = gameObject.transform.position + m_Camera.transform.forward * grabbedObjSize;
				newPos = new Vector3 (newPos.x, newPos.y, newPos.z);
				if (isAnimal)
					grabbedObject.transform.FindChild ("deer_ragdoll").transform.FindChild ("Armature").transform.FindChild ("Control").transform.position = newPos;
				else
					grabbedObject.transform.position = newPos;
				if (isAnimal) {
					for (int i = 0; i < grabbedObject.transform.childCount; i++) {
						grabbedObject.transform.GetChild (i).transform.position = newPos;
					}
				}
				if (Input.GetButton (m_FireButton) && !m_Fired) {

					m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
					m_AimSlider.value = m_CurrentLaunchForce;
					if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) {
						m_CurrentLaunchForce = m_MaxLaunchForce;
						ThrowObject ();
					}
				} else if (Input.GetButtonUp (m_FireButton) && !m_Fired) {
					ThrowObject ();
				}
			}
		}
	}

	/* This method will unequip the current equipped weapon/item and hold it in front of them to drop or place somewhere */
	public void GrabEquippedItem(){
		GameObject weapon = weaponScript.weapon.m_Anim.gameObject;
		// Step 1: Unequip weapon/item
		weaponScript.variables.enabled = false;
		weaponScript.weapon.weaponEquiped = false;
		weapon.gameObject.AddComponent <EquipableWeapon> ();
		EquipableWeapon EW = weapon.gameObject.GetComponent<EquipableWeapon>();		// get the newly created EquipableWeapon class
		EquipableWeapon info = weaponScript.getWeaponInfo ();						// get the current weapon information
		EW.create (info.weapon, info.ammo, info.effects);							// set the newly created EquipableWeapon class to the current info
		weapon.gameObject.AddComponent<BoxCollider> ();
		BoxCollider box = weapon.gameObject.GetComponent<BoxCollider> ();
		box.size = new Vector3 (0.5f, 2, 10);
		weapon.gameObject.AddComponent<Rigidbody> ();
		weapon.transform.localEulerAngles = new Vector3(weapon.transform.localEulerAngles.x, 90, 90);

		// Step 2: Set weapon/item as grabbed item
		weapon.transform.parent = null;
		TryGrabObject (weapon);
		grabbedObjSize = 1;
	}

	public void EquipGrabbedItem( GameObject item ){
		XmlImporter importer = GameObject.Find("Spawner").GetComponentInChildren<XmlImporter> ();
		importer.readWeaponData (importer.getWeaponByName (item.name.Replace("(Clone)","")));
		weaponScript.variables.enabled = true;
		weaponScript.weapon.weaponEquiped = true;
		weaponScript.setWeaponInfo ( item.GetComponent<EquipableWeapon>() );
		/*
		weaponScript.weapon.m_Anim = item.GetComponent<Animator> ();
		Destroy (item.GetComponent<Rigidbody> ());
		Destroy (item.GetComponent<BoxCollider> ());
		weaponScript.variables.enabled = true;
		weaponScript.weapon.weaponEquiped = true;
		*/
	}

	void ThrowObject(){

		m_Fired = true;

		if (grabbedObject == null)
			return;
		if (isAnimal)
			activateRagdoll (true);

		if (grabbedObject.GetComponent<Rigidbody> () != null) {
			Vector3 vel =  (m_CurrentLaunchForce * m_Camera.transform.forward);//m_Model.GetComponent<Rigidbody>().velocity + (m_CurrentLaunchForce * m_Camera.transform.forward)
			vel.y += m_arc;

			grabbedObject.GetComponent<Rigidbody> ().velocity = vel;

			for (int i = 0; i < colliders.Length; i++) {
				if (activeBoxes [i] == true) {
					colliders [i].enabled = true;
				}
			}
		}
		grabbedObject = null;
		m_CurrentLaunchForce = m_MinLaunchForce;
	}

	Rigidbody[] connectedBodies;
	void activateRagdoll( bool enable ){
		Transform ragdoll = grabbedObject.transform.FindChild ("deer_ragdoll").transform.FindChild ("Armature");
		//ragdoll.transform.position = newPos;
		ragdoll.gameObject.SetActive (enable);
		/*
		Joint[] joints = ragdoll.GetComponentsInChildren<Joint> ();
		if (!enable) {
			connectedBodies = new Rigidbody[joints.Length];
			for (int i = 0; i < joints.Length; i++) {
				if (joints [i].connectedBody != null) {
					connectedBodies [i] = joints [i].connectedBody;
					joints [i].connectedBody = null;
				}
			}
		} else {
			for (int i = 0; i < joints.Length; i++) {
				joints [i].connectedBody = connectedBodies [i];
			}
		}
		*/


	}
}
