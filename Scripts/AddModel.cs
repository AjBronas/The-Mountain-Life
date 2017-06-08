using UnityEngine;
using System.Collections;

public class AddModel : MonoBehaviour {

	public GameObject m_Model;
	// Use this for initialization
	void Start () {
		if (m_Model == null)
			m_Model = GameObject.Find ("ThirdPersonController");
		if (m_Model == null)
			enabled = false;
		
		GrabAndDrop GAD = GetComponentInParent<GrabAndDrop> ();
		GAD.m_Model = m_Model;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = new Vector3(transform.position.x,transform.position.y-transform.localScale.y,transform.position.z);
		//Vector3 rot = new Vector3(transform.rotation.x, transform.rotation.y, 0);
		m_Model.transform.position = pos;
		//m_Model.transform.localEulerAngles = rot;
	}
}
