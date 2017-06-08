using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public LayerMask m_AnimalMask;                      // Used to filter what the explosion affects, this should be set to "Animal".
	public ParticleSystem m_FleshImpactParticles;       // Reference to the particles that will play on animal impact.
	public ParticleSystem m_CloudImpactParticles;       // Reference to the particles that will play on animal impact.
	public ParticleSystem m_OtherImpactParticles;       // Reference to the particles that will play on ground/wall/etc impact.
	public AudioSource m_ImpactAudio;                	// Reference to the audio that will play on impact.
	public float m_MaxDamage = 100f;                    // The amount of damage done if the explosion is centred on a tank.
	public float m_ImpactForce = 1000f;              		// The amount of force added at the centre of the explosion.
	public float m_MaxLifeTime = 7.5f;                    // The time in seconds before the shell is removed.
	public float m_ImpactRadius = 0.02f;                // The maximum distance away from the explosion tanks can be and are still affected.

	private float m_OriginalPitch = 1;
	private float m_PitchRange = 0.2f;

	private Rigidbody r_body;
	public float bulletWeight = 0;

	void Start () {
		r_body = GetComponent<Rigidbody> ();

		if (m_ImpactAudio != null)
			m_OriginalPitch = m_ImpactAudio.pitch;
		if (m_FleshImpactParticles == null)
			m_FleshImpactParticles = GameObject.Find ("b_flesh").GetComponentInChildren<ParticleSystem>();
		if (m_CloudImpactParticles == null)
			m_CloudImpactParticles = GameObject.Find ("b_cloud").GetComponentInChildren<ParticleSystem>();
		if (m_OtherImpactParticles == null)
			m_OtherImpactParticles = GameObject.Find ("b_Impact").GetComponentInChildren<ParticleSystem>();
		
		Destroy (gameObject, m_MaxLifeTime);
	}

	void Update(){
		Vector3 pos = r_body.position;
		//r_body.position = new Vector3(pos.x, pos.y - (Physics.gravity.magnitude*bulletWeight), pos.z);
		if (r_body.velocity.magnitude < 50)
			r_body.velocity = r_body.transform.up * -25;

		m_ImpactForce -= 1;
		
	}

	void OnCollisionEnter(Collision col){

		Collider[] c_colliders = Physics.OverlapSphere (transform.position, m_ImpactRadius); //Use m_AnimalMask argument to only get animals

		// Go through all the colliders...
		for (int i = 0; i < c_colliders.Length; i++) {
			if (c_colliders [i].tag.Contains ("Animal")) {
				Deer_AI ai = c_colliders [i].gameObject.GetComponentInParent<Deer_AI> ();
				if (ai != null) {
					ai.bulletPos = transform.position;
					ai.bulletBlast = m_ImpactForce;
					ai.activateRagdoll ();
				}
				Rigidbody targetRigidbody = c_colliders [i].GetComponent<Rigidbody> ();
				if (targetRigidbody == null)
					targetRigidbody = c_colliders [i].GetComponentInParent<Rigidbody> ();
				if (targetRigidbody == null)
					targetRigidbody = c_colliders [i].GetComponentInChildren<Rigidbody> ();
				if (targetRigidbody == null)
					continue;

				targetRigidbody.AddExplosionForce (m_ImpactForce, transform.position, m_ImpactRadius);

				if (m_CloudImpactParticles != null) {
					m_CloudImpactParticles.transform.position = gameObject.transform.position;
					if (m_CloudImpactParticles.duration > 0.01f) {
						m_CloudImpactParticles.Simulate (0.01f);
						m_CloudImpactParticles.transform.position = gameObject.transform.position;
					}
					m_CloudImpactParticles.Play ();
					m_CloudImpactParticles.startLifetime = m_CloudImpactParticles.startLifetime;
				}

				if (m_FleshImpactParticles != null) {
					m_FleshImpactParticles.transform.position = gameObject.transform.position;
					if (m_FleshImpactParticles.duration > 0.01f) {
						m_FleshImpactParticles.Simulate (0.01f);
						m_FleshImpactParticles.transform.position = gameObject.transform.position;
					}
					m_FleshImpactParticles.Play ();
					m_FleshImpactParticles.startLifetime = m_FleshImpactParticles.startLifetime;

					if(c_colliders [i].GetComponent<Deer_AI> () != null)
						c_colliders [i].GetComponent<Deer_AI> ().Hurt( m_MaxDamage );
					else if(c_colliders [i].GetComponentInParent<Deer_AI> () != null)
						c_colliders [i].GetComponentInParent<Deer_AI> ().Hurt( m_MaxDamage );
					else if(c_colliders [i].GetComponentInChildren<Deer_AI> () != null)
						c_colliders [i].GetComponentInChildren<Deer_AI> ().Hurt( m_MaxDamage );
				}
			} else {
				
				Rigidbody targetRigidbody = c_colliders [i].GetComponent<Rigidbody> ();

				if (!targetRigidbody)
					targetRigidbody = c_colliders [i].GetComponentInParent<Rigidbody> ();

				if (!targetRigidbody)
					continue;
				
				if (c_colliders [i].name.Contains("Brick"))
					targetRigidbody.isKinematic = false;
				
				targetRigidbody.AddExplosionForce (m_ImpactForce, transform.position, m_ImpactRadius);

				if (m_OtherImpactParticles != null) {

					m_OtherImpactParticles.transform.position = gameObject.transform.position;
					if (m_OtherImpactParticles.duration > 0.01f) {
						m_OtherImpactParticles.Simulate (0.01f);
						m_OtherImpactParticles.transform.position = gameObject.transform.position;
					}
					m_OtherImpactParticles.Play ();
					m_OtherImpactParticles.startLifetime = m_OtherImpactParticles.startLifetime;
				}

			}

			if (m_ImpactAudio != null) {
				m_ImpactAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
				m_ImpactAudio.Play ();
			}


			Destroy (gameObject);
		}

	}
}
