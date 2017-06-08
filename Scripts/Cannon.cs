using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour {

	public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
	public ParticleSystem m_SparksParticles;			// Reference to the particles that will play on ignition.
	public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.
	public AudioSource m_SparksAudio;              	  // Reference to the audio that will play on ignition.
	public float m_Size = 1;
	public float m_arc = 6;
	public float m_strength = 4;
	public float m_fireDelay = 1f;

	public GameObject m_FireTransform;
	private GameObject projPrefab;
	//[System.NonSerialized]
	public bool attached = false;

	private float m_OriginalPitch = 1;
	private float m_PitchRange = 0.2f;
	private float m_Time = 0;
	private bool ignited = false;

	public Animator m_Anim;
	private int animCode_Fire = Animator.StringToHash("Fire");

	// Use this for initialization
	void Start () {
		if (m_ExplosionAudio != null) {
			m_OriginalPitch = m_ExplosionAudio.pitch;
			m_ExplosionAudio.volume = 0.25f;
		}
		if (m_SparksAudio != null) {
			m_SparksAudio.volume = 0.65f;
		}
		if (m_SparksParticles != null) {
			m_SparksParticles.Stop ();
		}
		if (m_FireTransform == null)
			m_FireTransform = GameObject.Find ("CannonFireTransform");
		m_Time = m_fireDelay;
	}
	
	// Update is called once per frame
	void Update (){
		if (attached) {
			if (Input.GetKeyDown (KeyCode.LeftShift)) {
				if (ignited == false) {
					ignite ();
				}
			}
		}

		if (ignited) {
			if (m_Time > 0) {
				m_Time -= Time.deltaTime;
			} else {
				ignited = false;
				m_Time = m_fireDelay;
				m_SparksParticles.Pause ();
				m_SparksParticles.GetComponent<Renderer> ().enabled = false;
				m_SparksAudio.Pause ();
				fireProjectile ();
			}	
		}
		
	}

	public void ignite(){
		ignited = true;
		m_SparksParticles.Play ();
		m_SparksParticles.GetComponent<Renderer> ().enabled = true;
		if (m_SparksParticles.duration >= 1f)
			m_SparksParticles.Play ();
		
		if (m_SparksAudio != null ) {
			m_SparksAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
			m_SparksAudio.Play ();
		}
	}

	public void fireProjectile(){

		Vector3 pos = new Vector3 (m_FireTransform.transform.position.x, m_FireTransform.transform.position.y, m_FireTransform.transform.position.z);
		pos += m_FireTransform.transform.forward * (m_Size + 1);
		MakeProjectile (pos.x, pos.y, pos.z, Color.black, m_Size);
		m_Anim.Play (animCode_Fire);
	}

	public GameObject MakeProjectile(float x, float y, float z, Color color,float size){

		if (projPrefab == null)
			projPrefab=Resources.Load<GameObject> ("Cannon");
		if (size == 0)
			size = projPrefab.transform.GetComponent<Renderer> ().bounds.size.x;

		Vector3 position = new Vector3 (x, y, z);
		GameObject newProj = Instantiate (projPrefab, position, Quaternion.identity) as GameObject;
		//newProj.GetComponent<Renderer> ().material.color = color;
		newProj.transform.localScale = new Vector3 (size, size, size);
		if( newProj.GetComponent<Rigidbody>() == null)
			newProj.AddComponent<Rigidbody>();
		Vector3 vel =  m_FireTransform.transform.forward * m_strength;
		vel.y += m_arc;
		newProj.GetComponent<Rigidbody> ().velocity = vel;
		Vector3 exPos = transform.forward;
		exPos.y -= 1;
		//GetComponent<Rigidbody> ().AddExplosionForce (200,exPos, 3f);

		if (m_ExplosionAudio != null ) {
			m_ExplosionAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
			m_ExplosionAudio.Play ();
			if (m_ExplosionParticles != null) {
				if (m_ExplosionParticles.duration >= 1f) {
					m_ExplosionParticles.Simulate (0.5f);
					m_ExplosionParticles.transform.position = m_FireTransform.transform.position;
				}
			
				m_ExplosionParticles.Play();
				m_ExplosionParticles.transform.parent = null;
			}
		}

		return newProj;
	}
}
