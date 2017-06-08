using UnityEngine;
using System.Collections;

public class AttachScript : MonoBehaviour {

	CharacterController cc;
	CharacterMotor cm;
	//Transform playerTransform;
	public float m_Distance = 3;
	private GameObject player;
	public GameObject attachedObject;

	bool attached = false;

	float rotationY = 180F;
	public float sensitivityY = 15F;
	public float minimumY = 180F;
	public float maximumY = 360F;

	private Weapon weapon;

	// Use this for initialization
	void Start () {
	
		player = GameObject.FindGameObjectWithTag("Player");
		cc = player.GetComponent<CharacterController>();
		cm = player.GetComponent<CharacterMotor>();
		//playerTransform = attachedObject.transform.parent;

	}

	bool IsPlayerClose(){

		return Vector3.Distance (attachedObject.transform.position, player.transform.position) < m_Distance;

	}

	void setAttached( bool attached ){
		
		this.attached = attached;
		cm.enabled = !attached;
		cc.enabled = !attached;
		if(attachedObject.GetComponentInChildren<Cannon> () != null)
			attachedObject.GetComponentInChildren<Cannon> ().attached = attached;
		if(GetComponent<Weapon> () != null)
			weapon = GetComponent<Weapon> ();
		weapon.weapon.attachedToVehicle = attached;
		player.GetComponentInChildren<Camera> ().enabled = !attached;
		player.GetComponent<MouseLook> ().enabled = !attached;
		if (attached) {
			GameObject.Find("CameraTransform").AddComponent<Camera> ();
			//Camera camera = attachedObject.GetComponent<Camera> ();
			//camera.transform.position = new Vector3 (transform.position.x, transform.position.y + 1.25f, transform.position.z + -0.5f);

		} else {
			Destroy (GameObject.Find("CameraTransform").GetComponent<Camera> ());
		}
		//attachedObject.GetComponentInChildren<Camera> ().enabled = attached;
		attachedObject.GetComponent<MouseLook> ().enabled = attached;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.E) && IsPlayerClose()) {
			setAttached (!attached);
		}

		if (attached) {
			rotationY += Input.GetAxis ("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			attachedObject.GetComponentInChildren<Cannon> ().m_arc = rotationY;
			//rotationY += yOffset;
			//GameObject.Find("CannonFireTransform").transform.localEulerAngles = new Vector3 (-rotationY, transform.localEulerAngles.y, 180f);
		}
	
	}
}
