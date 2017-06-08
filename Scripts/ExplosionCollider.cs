using UnityEngine;
using System.Collections;

public class ExplosionCollider : MonoBehaviour {

	public LayerMask m_WallMask;                        // Used to filter what the explosion affects, this should be set to "Players".
	public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
	public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.
	public float m_MaxDamage = 100f;                    // The amount of damage done if the explosion is centred on a tank.
	public float m_ExplosionForce = 300f;              // The amount of force added to a tank at the centre of the explosion.
	public float m_MaxLifeTime = 30f;                    // The time in seconds before the shell is removed.
	public float m_ExplosionRadius = 0.5f;                // The maximum distance away from the explosion tanks can be and are still affected.
	public float m_minVelocity = 1f;

	private float m_OriginalPitch = 1;
	private float m_PitchRange = 0.2f;

	private void Start (){
		if (m_ExplosionAudio != null)
			m_OriginalPitch = m_ExplosionAudio.pitch;
		// If it isn't destroyed by then, destroy the shell after it's lifetime.
		Destroy (gameObject, m_MaxLifeTime);
	}

	void OnCollisionEnter(Collision col){

		Collider[] c_colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_WallMask); //Use m_WallMask argument to only get wall
		bool playSound = false;

		// Go through all the colliders...
		for (int i = 0; i < c_colliders.Length; i++) {
			if (c_colliders [i].name.Contains("Brick")) {
				Rigidbody targetRigidbody = c_colliders [i].GetComponent<Rigidbody> ();
				//if (m_ExplosionAudio != null)
				//	m_ExplosionParticles.transform.parent = transform;
				if (!targetRigidbody)
					continue;
				float magnitude = gameObject.GetComponent<Rigidbody> ().velocity.magnitude;
				//Debug.Log ("Magnitude: " + magnitude);
				if (magnitude > m_minVelocity) {
					playSound = true;
					targetRigidbody.isKinematic = false;
					float explosion = (m_ExplosionForce * (Mathf.Clamp (magnitude, m_minVelocity, 3f))) / 2;
					targetRigidbody.AddExplosionForce (explosion, transform.position, m_ExplosionRadius);
					if(m_ExplosionAudio != null)
						m_ExplosionAudio.volume = (Mathf.Clamp (magnitude/2, 0f, 1f))/2;
				}
			}
		}
		if (m_ExplosionAudio != null && playSound == true && m_ExplosionAudio.isPlaying == false) {
			m_ExplosionAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
			m_ExplosionAudio.Play ();
			if (m_ExplosionParticles != null) {
				m_ExplosionParticles.transform.parent = null;

				if (m_ExplosionParticles.duration >= 1f) {
					m_ExplosionParticles.Simulate (0.5f);
					m_ExplosionParticles.transform.position = gameObject.transform.position;
				}
			
				m_ExplosionParticles.Play();
			}
		}
	}

}
