using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {

	public float m_Size = 1;
	public float m_CameraHeight = 2;
	public float m_arc = 6;
	public float m_strength = 4;
	public Camera m_Camera;
	private GameObject projPrefab;
	// Use this for initialization
	void Start () {
		if (m_Camera == null)
			m_Camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {

			Vector3 pos = new Vector3 (transform.position.x, transform.position.y+m_CameraHeight, transform.position.z);
			pos += m_Camera.transform.forward * (m_Size + 1);
			MakeProjectile (pos.x, pos.y, pos.z, Color.black, m_Size);

		}
	}

	public GameObject MakeProjectile(float x, float y, float z, Color color,float size){

		if (projPrefab == null)
			projPrefab=Resources.Load<GameObject> ("BasketBall2");
		if (size == 0)
			size = projPrefab.transform.GetComponent<Renderer> ().bounds.size.magnitude;
		
		Vector3 position = new Vector3 (x, y, z);
		GameObject newProj = Instantiate (projPrefab, position, Quaternion.identity) as GameObject;
		//newProj.GetComponent<Renderer> ().material.color = color;
		//newProj.transform.localScale = new Vector3 (size, size, size);
		if( newProj.GetComponent<Rigidbody>() == null)
			newProj.AddComponent<Rigidbody>();
		Vector3 vel =  m_Camera.transform.forward * m_strength;
		vel.y += m_arc;
		newProj.GetComponent<Rigidbody> ().velocity = vel;
		return newProj;
	}
}
