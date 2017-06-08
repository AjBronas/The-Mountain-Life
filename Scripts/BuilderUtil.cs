using UnityEngine;
using System.Collections;

public class BuilderUtil : MonoBehaviour {

	private WallBuilderUtil builder;
	public Transform		buildTransform;
	public BoxCollider 		collisionExtents;

	private bool 		inBuildMode 	= false;
	public 	GameObject 	obj_prefab;
	private Quaternion 	obj_rotation 	= Quaternion.identity;
	private int 		obj_odd			= 0;
	private Transform 	obj_origSize;
	private Vector3 	position 		= Vector3.zero;
	private int countX = 5, countY = 5, countZ = 1;


	void Start () {

		builder = gameObject.AddComponent<WallBuilderUtil> ();
		if (buildTransform == null)
			buildTransform = GameObject.Find ("BuildTransform").transform;
		
		if (collisionExtents == null)
			collisionExtents = GameObject.Find("BuildCollision").GetComponent<BoxCollider>();
		
		if (obj_prefab == null)
			obj_prefab = Resources.Load<GameObject> ("build_Wall");

		obj_origSize = obj_prefab.transform;
	}
	

	void Update () {
		if (obj_prefab == null)
			obj_prefab = Resources.Load<GameObject> ("build_Wall");
		
		if(Input.GetKeyDown (KeyCode.Tab)){		// go into build mode

			inBuildMode = !inBuildMode;
			if (inBuildMode) {
				position = transform.position + (transform.forward * 2);
				position.y = 0.5f;
				obj_prefab = Instantiate (obj_prefab, position, obj_rotation) as GameObject;
				obj_prefab.GetComponent<Renderer> ().enabled = true;
				obj_prefab.transform.localScale = new Vector3 ((obj_prefab.GetComponent<Renderer> ().bounds.extents.x / 2) + (obj_prefab.transform.localScale.x * countX), obj_prefab.transform.localScale.y * countY, obj_prefab.transform.localScale.z * countZ);
			} else {

				obj_prefab.transform.localScale = obj_origSize.localScale;
				obj_prefab.GetComponent<Renderer> ().enabled = false;
			}
			Debug.Log ("Build mode: " + inBuildMode);
		}

		// if in build mode...
		if (inBuildMode) {
			if(Input.GetKeyDown (KeyCode.R)){		// rotate object
				
				if( obj_rotation == Quaternion.identity)
					obj_rotation = new Quaternion (90, 0,90, 0);
				else
					obj_rotation = Quaternion.identity;
				
				obj_prefab.transform.rotation = obj_rotation;
				Debug.Log ("Rotating");
			}

			if(Input.GetKeyDown (KeyCode.Mouse0)){		// place object
				if (buildWall ())
					Debug.Log ("Placed");
				obj_odd = obj_odd == 0 ? 1 : 0;

			}
			position = transform.position + (transform.forward*2);
			position.y = 0.4f + obj_prefab.transform.localScale.y/2;
			obj_prefab.transform.position = position;
		
		}
	}

	private bool buildWall(){

		bool built = false;
		// check collision
		//Collider[] colliders = Physics.OverlapBox(buildTransform, topCol.GetComponent<Renderer>().bounds.extents/2,Quaternion.identity,m_Mask);
		Vector3 colPosition = position;
		colPosition.y += 0.5f;
		Collider[] colliders = Physics.OverlapBox (colPosition, obj_prefab.GetComponent<Renderer> ().bounds.extents / 2, Quaternion.identity);

		for (int i = 0; i < colliders.Length; i++) {
			Debug.Log ("Hit: " + colliders [i].name);
		}
		if ((colliders.Length == 0) || (colliders.Length == 1 && colliders [0].name == collisionExtents.name) || (colliders.Length == 1 && colliders [0].name == "Ground") ) {
			builder.CreateWall (position.x - (obj_rotation==Quaternion.identity? (obj_prefab.transform.localScale.x/2) : 0),0.5f,position.z - (obj_rotation==Quaternion.identity? 0 : (obj_prefab.transform.localScale.x/2)), countX, countY, countZ, obj_odd, obj_rotation);
			built = true;
		}

		return built;
	}
}
