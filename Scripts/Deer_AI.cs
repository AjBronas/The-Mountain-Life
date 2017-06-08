using UnityEngine;
using System.Collections;

public class Deer_AI : MonoBehaviour {

	/* Constants */
	public const string IDLE = "IDLE";
	public const string LAYING = "LAYING";
	public const string LAYING_IDLE = "LAYINGIDLE";
	public const string STANDING = "STANDING";
	public const string WALKING = "WALKING";
	public const string RUNNING = "RUNNING";
	public const int DIR_LEFT = -1;
	public const int DIR_RIGHT = 1;
	public const float COLL_RANGE = 3;

	public MeshCollider stand_Col;
	public BoxCollider layHead_Col;
	public SphereCollider layNeck_Col;
	public CapsuleCollider layBody_Col;
	private Transform centerCheck;
	private Transform rightCheck;
	private Transform leftCheck;
	private CharacterMotor motor;
	public Vector3 bulletPos;
	public float bulletBlast;

	/* States */
	private string state = IDLE;
	private float orig_speed = 1;
	private float run_speed = 2;
	public int direction = DIR_LEFT;
	public float turn_rotation = 0;
	public float turn_speed = 0;
	public float center_height = 1f;
	public float center_width = 1f;
	public float size = 1f;
	public float health = 100f;
	public bool dead = false;

	/* Timer */
	private float timer = 5f;
	private float countDown = 1f;

	/* Animations */
	public Animator m_Anim;
	private int animCode_Idle = Animator.StringToHash("Idle");
	private int animCode_Run = Animator.StringToHash("run_norm");
	private int animCode_Walk = Animator.StringToHash("walk_norm");
	private int animCode_LayIdle = Animator.StringToHash("lay_idle_001");
	private int animCode_Lay = Animator.StringToHash("laydown");
	private int animCode_Stand = Animator.StringToHash("getup");
	private int animCode_Die = Animator.StringToHash("die");

	void Awake () {

		motor = gameObject.GetComponent<CharacterMotor> ();
	}

	void Start () {

		size = Random.Range (0.5f, 0.95f);

		if (layHead_Col == null)
			layHead_Col = gameObject.GetComponentInChildren<BoxCollider> ();
		if (layNeck_Col == null)
			layNeck_Col = gameObject.GetComponentInChildren<SphereCollider> ();
		if (layBody_Col == null)
			layBody_Col = gameObject.GetComponentInChildren<CapsuleCollider> ();
		if (stand_Col == null)
			stand_Col = gameObject.GetComponent<MeshCollider> ();

		Transform[] tmp = gameObject.GetComponentsInChildren<Transform> ();
		for (int i = 0; i < tmp.Length; i++) {
			if (tmp [i].name == "centerCheck")
				centerCheck = tmp[i];
			if (tmp [i].name == "rightCheck")
				rightCheck = tmp[i];
			if (tmp [i].name == "leftCheck")
				leftCheck = tmp[i];
		}
		transform.localScale = new Vector3 (size, size, size);
		motor.movement.maxForwardSpeed += Mathf.Max(0,(size-1));
		orig_speed = motor.movement.maxForwardSpeed;
		run_speed = orig_speed * 8f;
		countDown = timer;
		playAnimation (animCode_Idle);
	}

	void Update () {
		
		if (!dead) {
			
			if (State () == WALKING) {
				Move (5);
				if (Random.Range (0, 100) < 10)		//turn
					Turn (Random.Range (5f, 45f) * direction);
				else if (Random.Range (0, 100) < 5)	//big turn
					Turn (Random.Range (90f, 145f) * direction);
				if (Random.Range (0, 100) < 5)		//change directions
					direction = (direction == DIR_LEFT ? DIR_RIGHT : DIR_LEFT);
				if (Random.Range (0f, 100f) < 0.1f)		//run
					GoRun (Random.Range (5f, 20f));
			}

			if (State () == RUNNING) {
				Move (5);
				if (Random.Range (0, 100) < 10)		//turn
					Turn (Random.Range (5f, 45f) * direction);
				else if (Random.Range (0, 100) < 5)	//big turn
					Turn (Random.Range (90f, 145f) * direction);
				if (Random.Range (0, 100) < 5)		//change directions
					direction = (direction == DIR_LEFT ? DIR_RIGHT : DIR_LEFT);
				if (Random.Range (0f, 100f) < 0.1f)		//walk
					GoWalk (Random.Range (10f, 60f));
			}

			if (countDown < 0) {
				if (State () == IDLE) {
					LayDown (Random.Range (5f, 15f));
				} else if (State () == LAYING) {
					LayIdle (Random.Range (5f, 15f));
				} else if (State () == LAYING_IDLE) {
					StandUp (Random.Range (5f, 5f));
				} else if (State () == STANDING) {
					GoWalk (Random.Range (10f, 60f));
				} else if (State () == WALKING) {
					GoIdle (Random.Range (5f, 15f));
				} else if (State () == RUNNING) {
					GoWalk (Random.Range (5f, 15f));
				}

			} else {
				countDown -= Time.deltaTime;
			}

			// alert
			if (State () != RUNNING) {
				if (Vector3.Distance (transform.position, GameObject.Find ("player").transform.position) < 10)
					GoRun (Random.Range (5f, 15f));
				if(Physics.CheckSphere (gameObject.transform.position, 5, LayerMask.GetMask ("Bullet")))
					GoRun (Random.Range (5f, 20f));
			}

		}
		
	}

	public void GoIdle( float time ){
		SetState (IDLE);
		playAnimation (animCode_Idle);
		setCollsion (STANDING);
		countDown = time;
	}

	public void LayDown( float time ){
		SetState (LAYING);
		playAnimation (animCode_Lay);
		setCollsion (LAYING);
		center_height /= 2;
		countDown = time;
	}

	private void LayIdle( float time ){
		m_Anim.SetFloat ("speed", Random.Range (0.75f, 1.5f));
		SetState (LAYING_IDLE);
		playAnimation (animCode_LayIdle);
		setCollsion (LAYING);
		center_height /= 2;
		countDown = time;
	}

	public void StandUp( float time ){
		SetState (STANDING);
		playAnimation (animCode_Stand);
		setCollsion (STANDING);
		center_height *= 2;
		countDown = time;
	}

	public void GoWalk( float time ){
		SetState (WALKING);
		playAnimation (animCode_Walk);
		setCollsion (STANDING);
		countDown = time;
		motor.movement.maxForwardSpeed = orig_speed;
	}

	public void GoRun( float time ){
		SetState (RUNNING);
		playAnimation (animCode_Run);
		setCollsion (STANDING);
		countDown = time;
		motor.movement.maxForwardSpeed = run_speed;
	}

	void Move( float forward ){

		if (CheckFrontCollision (COLL_RANGE) != null) {
			turn_speed = 1;
			Turn (Random.Range (30f, 60f ) * direction);
		}else{
			turn_speed = 0;
			turn_rotation = 0;
		}

		// Get the input vector from kayboard or analog stick
		Vector3 directionVector = new Vector3 (turn_speed*(-direction), 0, forward);

		if (directionVector != Vector3.zero) {
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;

			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min (1.0f, directionLength);

			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength *= directionLength;

			// Multiply the normalized direction vector by the modified length
			directionVector *= directionLength;
		}

		// Apply the direction to the CharacterMotor
		motor.input.direction = transform.rotation * directionVector;

	}
	void Turn( float turn ){

		turn_rotation += turn;
		transform.Rotate(new Vector3(0,-turn_rotation,0) * (Time.deltaTime/2));
	}

	void Stop(){
		
		motor.input.direction = Vector3.zero;
	}

	void Jump(){
		
		motor.input.jump = true;
	}

	public void Hurt( float damage ){

		health -= damage;
		if (health <= 0 && !dead)
			Die ();
		
	}

	void Die(){
		Stop ();
		dead = true;
		playAnimation(animCode_Die);
		setCollsion (LAYING);
		GetComponent<CharacterController> ().radius = 1.35f;
		GetComponent<CharacterController> ().enabled = false;
		Vector3 body = new Vector3 (-0.75f, 0, 0);
		Vector3 head = new Vector3 (0.55f, 0.25f, 1.65f);
		Vector3 neck = new Vector3 (0.1f, 0.25f, 1.5f);
		layHead_Col.center = head;
		layNeck_Col.center = neck;
		layBody_Col.transform.localPosition = body;
		this.GetComponent<Rigidbody> ().isKinematic = false;
		motor.enabled = false;

		// ragdoll
		activateRagdoll();

	}

	public void activateRagdoll(){
		if (!dead) {
			GetComponent<Animator> ().enabled = false;
			GetComponent<Rigidbody> ().useGravity = false;
			GetComponent<Rigidbody> ().isKinematic = false;
			GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
			GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
			gameObject.transform.FindChild ("layColl").gameObject.SetActive (false);
			Debug.Log ("DID");
			Transform ragdoll = gameObject.transform.FindChild ("Ragdoll");
			if (ragdoll != null) {
				ragdoll.gameObject.SetActive (true);
				ragdoll.transform.parent = null;
			}

			// add bullet force
			Rigidbody[] bodies = gameObject.transform.GetComponentsInChildren<Rigidbody> ();
			for (int i = 0; i < bodies.Length; i++) {
				if (bodies [i].gameObject.name == "Spine") {
					bodies [i].AddExplosionForce (bulletBlast, bulletPos, 0.02f);
					//Debug.Log ("BANG" + bodies[i].gameObject.name + ": " + bulletBlast);
				}
			}
			this.enabled = false;
		}
	}

	GameObject CheckFrontCollision( float range ){
		
		RaycastHit rayHit;

		Vector3 position = centerCheck.position;
		Vector3 target = position + (transform.forward * range);

		//check center
		if (Physics.Linecast (position, target, out rayHit)) {
			Debug.DrawLine (position, target, Color.red);
			return rayHit.collider.gameObject;
		}

		//check right side
		position = rightCheck.position;
		target = position + (transform.forward * range); target.x += center_width;
		if (Physics.Linecast (position, target, out rayHit)) {
			Debug.DrawLine (position, target, Color.red);
			return rayHit.collider.gameObject;
		}

		// check left side
		position = leftCheck.position;
		target = position + (transform.forward * range); target.x -= center_width;
		if (Physics.Linecast (position, target, out rayHit)) {
			Debug.DrawLine (position, target, Color.red);
			return rayHit.collider.gameObject;
		}
		return null;
	}

	public void SetState( string const_state ){
		Stop ();
		state = const_state;
	}
	
	public string State(){
		return state;
	}

	void setCollsion( string pose ){

		if (layBody_Col != null && stand_Col != null) {
			if (pose == LAYING) {
				layHead_Col.enabled = true;
				layNeck_Col.enabled = true;
				layBody_Col.enabled = true;
				stand_Col.enabled = false;
			} else {
				layHead_Col.enabled = false;
				layNeck_Col.enabled = false;
				layBody_Col.enabled = false;
				stand_Col.enabled = true;
			}
		} //else
			//Debug.Log ("ERR: Deer Collision");
	}

	public void playAnimation( int ani ) {
		if(m_Anim != null)
			m_Anim.Play (ani);
	}
}
