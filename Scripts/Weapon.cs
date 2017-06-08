using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	private Vector3 DEFAULT_LOCATION = new Vector3 (0.14f, -0.073f, 0.43f);

	private GameObject spawner;

	/* Arms */
	[System.Serializable]
	public class Arms{
		public Animator m_Anim;
		public string aniIndex = "";
		public Vector3 normPosition;
		public Quaternion normRotation;
		[System.NonSerialized]
		public Vector3 sprintingOffset = new Vector3(0,-0.073f,0.43f);
	}
	public Arms arms = new Arms();

	/* Weapon and Projectile */
	[System.Serializable]
	public class WeaponAndProj{
		[System.NonSerialized]
		public EquipableWeapon weaponInfo = new EquipableWeapon ();
		public Animator m_Anim; 
		public Transform m_FireTransform;
		[SerializeField]
		public GameObject projPrefab;
		[System.NonSerialized]
		public bool attachedToVehicle = false;
		[System.NonSerialized]
		public bool weaponEquiped = false;
	}
	public WeaponAndProj weapon = new WeaponAndProj();

	/* Effects */
	[System.Serializable]
	public class WeaponEffects{
		public Transform m_ParticleTransform;
		public ParticleSystem m_FireParticles;         // Reference to the particles that will play on fireing.
		public ParticleSystem m_SmokeParticles;			// Reference to the particles that will play after firing.
		public AudioSource m_FireAudio;                // Reference to the audio that will play on firing.
		public AudioSource m_ReloadAudio;              	// Reference to the audio that will play on reloading.
		public AudioSource m_PullbackAudio;             // Reference to the audio that will play on pulling bolt back.
		public AudioSource m_EmptyAudio;           		// Reference to the audio that will play on empty clip.
		[System.NonSerialized]
		public float m_OriginalPitch = 1;
		[System.NonSerialized]
		public float m_PitchRange = 0.2f;
	}
	public WeaponEffects effects = new WeaponEffects();

	/* Weapon variables */
	[System.Serializable]
	public class WeaponVarialbes{
		[System.NonSerialized]
		public bool canShoot = true;
		[System.NonSerialized]
		public bool firing = false;
		[System.NonSerialized]
		public bool reloading = false;
		public float fireRate = 1f;
		[System.NonSerialized]
		public float firingTimeLeft = 1f;
		public float reloadTime = 1f;
		[System.NonSerialized]
		public bool zoomedIn = false;
		public float zoomMagn = 4;
		public float zoomSpeed = 5;
		[System.NonSerialized]
		public float normFOV;
		public Vector3 normPosition;
		public Vector3 aimPosition;
		public float proj_speed = 50;
		public float proj_arc = 0;
		public Vector3 proj_size = new Vector3(1, 1, 1);
		public float weaponSway = 5f;
		[System.NonSerialized]
		public float mouseMoveNewX = 0;
		[System.NonSerialized]
		public float mouseMoveNewY = 0;
		[System.NonSerialized]
		public Quaternion normRotation;
		public float loudness = 30;
		public Vector3 scale = new Vector3();
		public Quaternion rotation;
		public float cost = 1999.00f;
		public bool enabled = true;
	}
	public WeaponVarialbes variables = new WeaponVarialbes();


	/* Ammo */
	[System.Serializable]
	public class WeaponAmmo{
		public string ammoType = ".308";
		public int bulletsLeft = 0;
		public int clipsLeft = 0;
		public int clipSize = 15;
		public int maxClips = 5;
	}
	public WeaponAmmo ammo = new WeaponAmmo();


	/* GunAnimations */
	private int animCode_Default = Animator.StringToHash("Default Take");
	private int animCode_Idle = Animator.StringToHash("idle");
	private int animCode_Shoot = Animator.StringToHash("shoot");
	private int animCode_Reload = Animator.StringToHash("reload");
	private int animCode_PullUp = Animator.StringToHash("pullup");

	/* ArmAnimations */
	private int animCode_IdleE = Animator.StringToHash("idleempty");
	private int animCode_RunE = Animator.StringToHash("runempty");

	private int previousAnimation = Animator.StringToHash("Default Take");
	private string lastAnimation = "";

	void Start () {

		spawner = GameObject.Find("Spawner");
		print (Application.dataPath + "/");

		if (arms.m_Anim != null) {
			arms.m_Anim.transform.SetParent (Camera.main.transform);
			arms.m_Anim.transform.localPosition = DEFAULT_LOCATION;
			//arms.m_Anim.transform.RotateAround (arms.m_Anim.transform.position, arms.m_Anim.transform.up, 180f);
			arms.m_Anim.transform.localRotation = new Quaternion (arms.m_Anim.transform.localRotation.x, 180, arms.m_Anim.transform.localRotation.z, arms.m_Anim.transform.localRotation.w);
			arms.m_Anim.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
		}
		if (weapon.m_Anim != null) {
			weapon.m_Anim.transform.SetParent (Camera.main.transform);
			weapon.m_Anim.transform.localPosition = DEFAULT_LOCATION;
			weapon.m_Anim.transform.localRotation = new Quaternion (weapon.m_Anim.transform.localRotation.x, 180, weapon.m_Anim.transform.localRotation.z, weapon.m_Anim.transform.localRotation.w);
			weapon.m_Anim.transform.localScale = new Vector3 (variables.scale.x, variables.scale.y, variables.scale.z);
		}
		
		ammo.bulletsLeft = ammo.clipSize;
		ammo.clipsLeft = ammo.maxClips;
		variables.firingTimeLeft = variables.fireRate;
		variables.normFOV = Camera.main.fieldOfView;
		if (weapon.m_Anim != null) {
			arms.normPosition = weapon.m_Anim.transform.localPosition;
			arms.normRotation = weapon.m_Anim.transform.localRotation;
			variables.normPosition = weapon.m_Anim.transform.localPosition;
			variables.normRotation = weapon.m_Anim.transform.localRotation;
		}

		if (weapon.projPrefab == null)
			weapon.projPrefab = Resources.Load<GameObject> ("Bullet308");
		
		if (weapon.m_FireTransform == null)
			weapon.m_FireTransform = transform;
		
		if (variables.proj_size.x == 0 && variables.proj_size.y == 0 && variables.proj_size.z == 0)
			variables.proj_size = this.weapon.projPrefab.transform.lossyScale;
 
		if(effects.m_FireAudio != null)
			effects.m_OriginalPitch = effects.m_FireAudio.pitch;

		// set animation alternate index
		animCode_Idle = Animator.StringToHash("idle"+arms.aniIndex);
		animCode_Shoot = Animator.StringToHash("shoot"+arms.aniIndex);
		animCode_Reload = Animator.StringToHash("reload"+arms.aniIndex);
		animCode_PullUp = Animator.StringToHash("pullup"+arms.aniIndex);

		/*-- Set the weapon to null, we dont want the player to start with a weapon right away--*/
		weapon.m_Anim = null;

		if (arms.m_Anim == null)
			print ("No arm animations were found.");
		if (weapon.m_Anim != null)
			weapon.weaponEquiped = true;

		if( weapon.weaponEquiped )
			playAnimation (animCode_Idle);
		else
			playAnimation (animCode_IdleE);
		XmlImporter importer = spawner.GetComponentInChildren<XmlImporter> ();
		importer.getWeaponData ();
		//importer.readWeaponData (0);
	}

	void Update () {
		if (variables.enabled) {
		
			if (weapon.weaponEquiped) {
			
				variables.mouseMoveNewX = Input.GetAxis ("Mouse X");
				variables.mouseMoveNewY = Input.GetAxis ("Mouse Y");
				float horzMovement = Input.GetAxis ("Horizontal");
				float vertMovement = Input.GetAxis ("Vertical");

				if (!weapon.attachedToVehicle || !GlobalVariables.inVehicle) {
					if (Input.GetButtonDown (GlobalVariables.InputVariables.BTN_RELOAD)) {
						if (ammo.bulletsLeft < ammo.clipSize)
							Reload ();
					}
					if (Input.GetMouseButton (0)) {
						if (ammo.bulletsLeft == 0) {
							Reload ();
						} else if (variables.canShoot && !variables.firing && !variables.reloading && ammo.bulletsLeft > 0) {
							Shoot ();
						}
					}
					if (Input.GetMouseButtonDown (1)) {

						if (!variables.reloading) {
							if (!variables.zoomedIn) {
								weapon.m_Anim.SetFloat ("speed", 0);	// stop the animation
								arms.m_Anim.SetFloat ("speed", 0);	// stop the animation
								variables.zoomedIn = true;
							} else {
								weapon.m_Anim.SetTime (0);
								weapon.m_Anim.SetFloat ("speed", 1);	// start the animation
								arms.m_Anim.SetFloat ("speed", 1);	// start the animation
								variables.zoomedIn = false;
							}
						}
					}

					// Only sometimes play the idle animation
					string clip = weapon.m_Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
					if (lastAnimation != clip) {
						lastAnimation = clip;
						if (clip.Contains ("idle")) {
							float chance = Random.Range (0.0f, 1.0f);
							if (chance < 0.75f) {
								weapon.m_Anim.SetFloat ("speed", 0);	// stop the animation
								arms.m_Anim.SetFloat ("speed", 0);	// stop the animation
							}
						} else {
							//weapon.m_Anim.SetTime (0);
							if (!variables.zoomedIn) {
								weapon.m_Anim.SetFloat ("speed", 1);	// start the animation
								arms.m_Anim.SetFloat ("speed", 1);	// start the animation
							}
						}
					}

					/* This will make the weapon move slightly back in forth to simulate unsteady hands */
					weapon.m_Anim.transform.Rotate (new Vector3 (0, Mathf.Cos (Time.timeSinceLevelLoad), 0) * (Time.deltaTime * 2));
					arms.m_Anim.transform.Rotate (new Vector3 (0, Mathf.Cos (Time.timeSinceLevelLoad), 0) * (Time.deltaTime * 2));

					/* Weapon Sway */
					float swayAmount = variables.weaponSway;
					float returnAmount = variables.weaponSway / 2;
					if (variables.zoomedIn) {
						swayAmount /= variables.weaponSway - 1;
						returnAmount *= 1.5f;
					}
					variables.mouseMoveNewX = Mathf.Clamp (variables.mouseMoveNewX, -swayAmount, swayAmount);								/* Clamp X input */
					variables.mouseMoveNewY = Mathf.Clamp (variables.mouseMoveNewY, -swayAmount, swayAmount);								/* Clamp Y input */
					weapon.m_Anim.transform.Rotate (new Vector3 (0, -variables.mouseMoveNewX, 0) * (Time.deltaTime * (swayAmount * 2)));	/* Swing horizontaly */
					arms.m_Anim.transform.Rotate (new Vector3 (0, -variables.mouseMoveNewX, 0) * (Time.deltaTime * (swayAmount * 2)));	/* Swing horizontaly */
					weapon.m_Anim.transform.Rotate (new Vector3 (variables.mouseMoveNewY, 0, 0) * (Time.deltaTime * (swayAmount * 2)));	/* Swing vertically */
					arms.m_Anim.transform.Rotate (new Vector3 (variables.mouseMoveNewY, 0, 0) * (Time.deltaTime * (swayAmount * 2)));	/* Swing vertically */
					weapon.m_Anim.transform.localRotation = Quaternion.Lerp (weapon.m_Anim.transform.localRotation, variables.normRotation, Time.deltaTime * (returnAmount));
					arms.m_Anim.transform.localRotation = Quaternion.Lerp (arms.m_Anim.transform.localRotation, arms.normRotation, Time.deltaTime * (returnAmount));

					/* Movement if walking */
					if (horzMovement != 0 || vertMovement != 0) {
						if (variables.zoomedIn) {
							weapon.m_Anim.transform.Rotate (new Vector3 (Mathf.Cos (Time.timeSinceLevelLoad), 0, 0) * (Time.deltaTime * 5));
							arms.m_Anim.transform.Rotate (new Vector3 (Mathf.Cos (Time.timeSinceLevelLoad), 0, 0) * (Time.deltaTime * 5));
						} else {
							weapon.m_Anim.transform.Rotate (new Vector3 (Mathf.Cos (Time.timeSinceLevelLoad * 2), 0, 0) * (Time.deltaTime * 5));
							arms.m_Anim.transform.Rotate (new Vector3 (Mathf.Cos (Time.timeSinceLevelLoad * 2), 0, 0) * (Time.deltaTime * 5));
						}
					}

					if (!variables.zoomedIn) {
						Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, variables.normFOV, Time.deltaTime * variables.zoomSpeed);
						weapon.m_Anim.transform.localPosition = Vector3.Lerp (weapon.m_Anim.transform.localPosition, variables.normPosition, Time.deltaTime * variables.zoomSpeed);
						arms.m_Anim.transform.localPosition = Vector3.Lerp (arms.m_Anim.transform.localPosition, arms.normPosition, Time.deltaTime * variables.zoomSpeed);
					} else {
						Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, variables.normFOV / variables.zoomMagn, Time.deltaTime * variables.zoomSpeed);
						weapon.m_Anim.transform.localPosition = Vector3.Lerp (weapon.m_Anim.transform.localPosition, variables.aimPosition, Time.deltaTime * variables.zoomSpeed);
						arms.m_Anim.transform.localPosition = Vector3.Lerp (arms.m_Anim.transform.localPosition, variables.aimPosition, Time.deltaTime * variables.zoomSpeed);
					}

				}



				/* Time between shooting */
				if (variables.firing) {

					variables.firingTimeLeft -= Time.deltaTime;

					if (variables.firingTimeLeft <= 0) {
						variables.canShoot = true;
						variables.firing = false;
						variables.firingTimeLeft = variables.fireRate;
					}
				}
				variables.mouseMoveNewX = 0;
				variables.mouseMoveNewY = 0;
			}
		}
			
		if (weapon.weaponEquiped == false){
			if (GlobalVariables.PlayerVariables.isSprinting) {
				if (previousAnimation != animCode_RunE) {
					Vector3 pos = arms.m_Anim.transform.position;
					arms.m_Anim.transform.localPosition = arms.sprintingOffset;
					playAnimation (animCode_RunE);
				}
			} else {
				if (previousAnimation != animCode_IdleE) {
					Vector3 pos = arms.normPosition;
					arms.m_Anim.transform.localPosition = arms.normPosition;
					playAnimation (animCode_IdleE);
				}
			}
		}
	}

	void Shoot(){

		if (weapon.weaponEquiped) {
			Vector3 pos = weapon.m_FireTransform.transform.position;
			pos += weapon.m_FireTransform.transform.forward * (variables.proj_size.x + 1);
			MakeProjectile (pos.x, pos.y, pos.z, Color.yellow, variables.proj_size);
			playAnimation (animCode_Shoot);
			variables.canShoot = false;
			variables.firing = true;
			ammo.bulletsLeft = (ammo.bulletsLeft == 0 ? 0 : ammo.bulletsLeft -= 1);

			/* Scare any animals away from sound */
			Collider[] colliders = Physics.OverlapSphere (gameObject.transform.position, variables.loudness, LayerMask.GetMask ("Animal"));
			if (colliders.Length > 0) {
				Deer_AI ai;
				for (int i = 0; i < colliders.Length; i++) {
					ai = colliders [i].GetComponent<Deer_AI> ();
					if (ai == null)
						ai = colliders [i].GetComponentInParent<Deer_AI> ();
					if (ai == null)
						ai = colliders [i].GetComponentInChildren<Deer_AI> ();
					if (ai != null && ai.State () != Deer_AI.RUNNING)
						ai.GoRun (Random.Range (5f, 20f));
				}
			}

		} else {
			playAnimation (animCode_PullUp);
		}
	}

	int Reload(){
		
		if (weapon.weaponEquiped) {
			if (ammo.clipsLeft > 0 && !variables.reloading) {
				variables.zoomedIn = false;
				weapon.m_Anim.SetFloat ("speed", 1);
				arms.m_Anim.SetFloat ("speed", 1);
				playAnimation (animCode_Reload);
				variables.canShoot = false;
				variables.reloading = true;

				StartCoroutine (Reloaded ());


			}
		} else {
			playAnimation (animCode_RunE);
		}

		return 0;
	}

	IEnumerator  Reloaded(){

		yield return new WaitForSeconds(variables.reloadTime);
		ammo.bulletsLeft = (ammo.clipsLeft == 0 ? 0 : ammo.clipSize);
		ammo.clipsLeft = (ammo.clipsLeft == 0 ? ammo.clipsLeft = 0 : ammo.clipsLeft -= 1);
		variables.canShoot = true;
		variables.reloading = false;



	}

	public GameObject MakeProjectile(float x, float y, float z, Color color,Vector3 size){
		
		GameObject newProj = null;

		if (weapon.projPrefab != null) {
			Vector3 position = new Vector3 (x, y, z);
			//Quaternion rotation = weapon.m_FireTransform.transform.rotation;
			newProj = Instantiate (weapon.projPrefab, position, weapon.m_FireTransform.transform.rotation) as GameObject;
			newProj.GetComponent<Renderer> ().material.color = color;
			newProj.transform.localScale = size;

			if (newProj.GetComponent<Rigidbody> () == null)
				newProj.AddComponent<Rigidbody> ();
			Vector3 vel = weapon.m_FireTransform.transform.forward * variables.proj_speed;
			vel.y += variables.proj_arc;
			newProj.GetComponent<Rigidbody> ().velocity = vel;
			Vector3 lookDir = newProj.transform.forward;
			newProj.transform.localRotation = Quaternion.LookRotation(lookDir - newProj.transform.forward*2);

			/* Explosion force 
			 * Vector3 exPos = transform.forward;
			 * exPos.y -= 1;
			 * GetComponent<Rigidbody> ().AddExplosionForce (200,exPos, 3f);
			*/

			if (effects.m_FireAudio != null) {
				effects.m_FireAudio.pitch = Random.Range (effects.m_OriginalPitch - effects.m_PitchRange, effects.m_OriginalPitch + effects.m_PitchRange);
				effects.m_FireAudio.Play ();
				if (effects.m_FireParticles != null) {
					if (effects.m_FireParticles.duration >= 0.01f) {
						effects.m_FireParticles.Simulate (0.05f);
						effects.m_FireParticles.transform.position = effects.m_ParticleTransform.transform.position;
					}

					effects.m_FireParticles.Play ();
					effects.m_FireParticles.transform.parent = null;
				}
				if (effects.m_SmokeParticles != null) {
					if (effects.m_SmokeParticles.duration >= 0.01f) {
						effects.m_SmokeParticles.Simulate (0.05f);
						effects.m_SmokeParticles.transform.position = effects.m_ParticleTransform.transform.position;
					}

					effects.m_SmokeParticles.Play ();
				}
			}
		}

		return newProj;
	}

	public void setEquipped( EquipableWeapon weapon ){
		this.ammo.ammoType = weapon.ammo.ammoType;
		this.ammo.clipSize = weapon.ammo.clipSize;
		this.ammo.maxClips = weapon.ammo.maxClips;

		this.arms.aniIndex = weapon.weapon.aniIndex;
		this.arms.m_Anim.tag = "Arms";

		this.variables.fireRate = weapon.weapon.fireRate;
		this.variables.reloadTime = weapon.weapon.reloadTime;
		this.variables.weaponSway = weapon.weapon.weaponSway;
		this.variables.proj_speed = weapon.weapon.proj_speed;
		this.variables.proj_arc = weapon.weapon.proj_arc;
		this.variables.zoomMagn = weapon.weapon.zoomMagn;
		this.variables.zoomSpeed = weapon.weapon.zoomSpeed;
		this.variables.loudness = weapon.weapon.loudness;
		this.variables.aimPosition = weapon.weapon.aimPosition;
		this.variables.cost = weapon.weapon.cost;

		this.effects.m_FireAudio = weapon.effects.m_FireAudio;
		this.effects.m_ReloadAudio = weapon.effects.m_ReloadAudio;
		this.effects.m_PullbackAudio = weapon.effects.m_PullbackAudio;
		this.effects.m_EmptyAudio = weapon.effects.m_EmptyAudio;
		this.effects.m_ParticleTransform = weapon.effects.m_ParticleTransform;
		Quaternion oldRot = new Quaternion ();
		GameObject oldGun = null;
		if(this.weapon.m_Anim != null){
			oldRot = this.weapon.m_Anim.transform.localRotation;
			oldGun = this.weapon.m_Anim.gameObject;
			oldGun.transform.parent = null;
		}

		this.weapon.m_Anim = null;

		this.weapon.m_Anim = weapon.weapon.m_Anim;
		this.weapon.m_FireTransform = weapon.weapon.m_FireTransform;

		this.weapon.m_Anim.transform.SetParent (Camera.main.transform);
		this.weapon.m_Anim.transform.localPosition = weapon.weapon.position;
		this.weapon.m_Anim.tag = "Weapon";
		Quaternion rotation = new Quaternion ((weapon.weapon.rotation.x == 0 ? oldRot.x : weapon.weapon.rotation.x), (weapon.weapon.rotation.y == 0 ? oldRot.y : weapon.weapon.rotation.y), (weapon.weapon.rotation.z == 0 ? oldRot.z : weapon.weapon.rotation.z), (weapon.weapon.rotation.w == 0 ? oldRot.w : weapon.weapon.rotation.w));

		this.weapon.m_Anim.transform.localRotation = rotation;
		this.weapon.m_Anim.transform.localScale = new Vector3 (weapon.weapon.scale.x,weapon.weapon.scale.y, weapon.weapon.scale.z);

		this.variables.scale = weapon.weapon.scale;
		this.variables.normPosition = this.weapon.m_Anim.transform.localPosition;
		this.variables.normRotation = this.weapon.m_Anim.transform.localRotation;
		ammo.bulletsLeft = ammo.clipSize;
		ammo.clipsLeft = ammo.maxClips;
		variables.firingTimeLeft = variables.fireRate;
		variables.normFOV = Camera.main.fieldOfView;
		if (weapon.weapon.projPrefab == null)
			this.weapon.projPrefab = Resources.Load<GameObject> ("Bullet308");

		if (weapon.weapon.m_FireTransform == null)
			this.weapon.m_FireTransform = transform;

		if (variables.proj_size.x == 0 && variables.proj_size.y == 0 && variables.proj_size.z == 0)
			variables.proj_size = this.weapon.projPrefab.transform.lossyScale;

		if(effects.m_FireAudio != null)
			effects.m_OriginalPitch = effects.m_FireAudio.pitch;

		this.arms.aniIndex = weapon.weapon.aniIndex;
		// set animation alternate index
		animCode_Idle = Animator.StringToHash("idle"+arms.aniIndex);
		animCode_Shoot = Animator.StringToHash("shoot"+arms.aniIndex);
		animCode_Reload = Animator.StringToHash("reload"+arms.aniIndex);
		animCode_PullUp = Animator.StringToHash("pullup"+arms.aniIndex);

		//this.arms.normPosition = this.weapon.m_Anim.transform.localPosition;
		//this.arms.normRotation = this.weapon.m_Anim.transform.localRotation;
		this.weapon.weaponEquiped = true;
		this.weapon.weaponInfo = weapon;
		Destroy (oldGun);
		playAnimation (animCode_PullUp);
	}

	void playAnimation( int ani ) {
		if(weapon.m_Anim != null)
			weapon.m_Anim.Play (ani);
		if(arms.m_Anim != null)
			arms.m_Anim.Play (ani);
		previousAnimation = ani;
	}

	public EquipableWeapon getWeaponInfo(){
		weapon.weaponInfo.setAmmoAttr (ammo.ammoType,ammo.clipSize, ammo.maxClips);
		weapon.weaponInfo.ammo.bulletsLeft = ammo.bulletsLeft;
		weapon.weaponInfo.ammo.clipsLeft = ammo.clipsLeft;

		return weapon.weaponInfo;
	}

	public void setWeaponInfo( EquipableWeapon weapon ){
		this.weapon.weaponInfo = weapon;
		this.ammo.bulletsLeft = weapon.ammo.bulletsLeft;
		this.ammo.clipsLeft = weapon.ammo.clipsLeft;
	}

	public void toggleWeapon( bool enabled ){
		//weapon.m_Anim.GetComponent<Animator>().enabled = enabled;
		//arms.m_Anim.GetComponent<Animator>().enabled = enabled;

		weapon.weaponEquiped = enabled;
		variables.enabled = enabled;
	}

}
