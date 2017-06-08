using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateShape : MonoBehaviour {

	public float m_Size = 5;

	Transform playerPos;
	private GameObject cubePrefab;
	public Camera m_Camera;

	// Use this for initialization
	void Start () {
		if( m_Camera == null )
			m_Camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetButton ("Submit")) {

			if (canCreate ( m_Size )) {
				Vector3 pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
				pos += m_Camera.transform.forward * (m_Size + 1);
				MakeCube (pos.x, pos.y, pos.z, Color.blue, m_Size);
			}
		}

	}

	public bool canCreate( float range ){
		RaycastHit rayHit;
		Vector3 target = transform.position + m_Camera.transform.forward * range;
	
		if (Physics.Linecast(transform.position, target, out rayHit))
			return false;
		return true;
	}

	public GameObject MakeCube(float x, float y, float z, Color color,float size){
		if (cubePrefab == null)
			cubePrefab=Resources.Load<GameObject> ("Cube");

		Vector3 position = new Vector3 (x, y, z);
		GameObject newCube = Instantiate (cubePrefab, position, Quaternion.identity) as GameObject;
		newCube.GetComponent<Renderer> ().material.color = color;
		newCube.transform.localScale = new Vector3 (size, size, size);
		return newCube;
	}
}
