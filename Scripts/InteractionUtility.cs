using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class InteractionUtility : MonoBehaviour {

	public float m_Range = 3;
	private GameObject iObject;
	private Weapon weaponScript;
	private XmlImporter xmlScript;

	// Use this for initialization
	void Start () {
		weaponScript = GameObject.FindGameObjectWithTag ("Player").GetComponent<Weapon>();
		xmlScript = GameObject.Find ("Spawner").GetComponent<XmlImporter>();

	}
	
	// Update is called once per frame
	void Update () {

		/* This will print to the logger what you are looking at
		GameObject body = GetMouseHoverObject (m_Range);
		if (body != null && body.tag != "Untagged") {
			Debug.Log (body.tag);
		}
		*/

		if (Input.GetButtonDown (GlobalVariables.InputVariables.BTN_INTERACT)) {
			if (GlobalVariables.inVehicle){
				GlobalVariables.setDriving (false);
				return;
			}
			TryInteract (GetMouseHoverObject (m_Range));
		}
	}

	private GameObject GetMouseHoverObject( float range ){

		Vector3 position = Camera.main.transform.position;
		RaycastHit rayHit;
		Vector3 target = Camera.main.transform.position + Camera.main.transform.forward * range;
		if (Physics.Linecast (position, target, out rayHit))
			return rayHit.collider.gameObject;
		return null;
	}

	private void TryInteract( GameObject grabObject ){

		if( grabObject == null || !CanInteract(grabObject) )
			return;

		iObject = grabObject;
		if (iObject.tag == "Weapon") {
			xmlScript.readWeaponData (xmlScript.getWeaponByName (iObject.name));
		} else if (iObject.tag == "Store") {
			GameObject spawner = GameObject.Find ("Spawner");
			XmlImporter xml = spawner.GetComponent<XmlImporter> ();
			MenuBuilder menu = spawner.GetComponent<MenuBuilder> ();
			menu.openStore ();
			menu.FindCanvas("StoreContent");
			menu.fillBasic ();
		} else if (iObject.tag == "VehicleInteraction") {
			GlobalVariables.setDriving (true);
			//print (iObject.name + ", " + iObject.transform.parent.name + ", " + iObject.transform.parent.transform.FindChild("VehicleCam").name);
			GlobalVariables.player.transform.position = iObject.transform.parent.transform.FindChild ("VehicleCam").transform.position;
			print ("In Vehicle: " + GlobalVariables.inVehicle);
		} else if (iObject.name == "interactTailgate") {
			Transform tailgate = iObject.transform.parent.FindChild ("TailGate").FindChild ("tailgate");
			Transform tailgateUP = iObject.transform.parent.FindChild ("TailGate").FindChild ("tailgateUP");
			HingeJoint joint = tailgate.GetComponent<HingeJoint> ();
			Debug.Log ("Interacting with tailgate");
			JointLimits limits = joint.limits;
			limits.min = -30;
			limits.bounciness = 0;
			limits.bounceMinVelocity = 0;
			limits.max = 156;
			joint.limits = limits;
			//joint.useMotor = true;
			//joint.useLimits = false;
			/*bool isUP = !joint.useMotor;
			tailgateUP.gameObject.SetActive (isUP);
			tailgateUP.GetComponent<Renderer> ().enabled = isUP;
			tailgate.gameObject.SetActive (!isUP);
			tailgate.GetComponent<Renderer> ().enabled = !isUP;*/
			joint.useMotor = !joint.useMotor;
		}
	}

	private bool CanInteract( GameObject canidate ){
		bool result = false;
		if (canidate.GetComponent<Rigidbody> () != null || canidate.GetComponentInParent<Rigidbody> () != null || canidate.tag == "Weapon" )
			result = true;
		return result;
	}

}
