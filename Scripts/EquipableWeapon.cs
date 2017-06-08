using UnityEngine;
using System.Collections;

public class EquipableWeapon : MonoBehaviour  {
	[System.Serializable]
	public class WeaponAndProj{
		public Animator m_Anim;
		public Transform m_FireTransform;
		public string aniIndex = "";
		[SerializeField]
		public GameObject projPrefab;
		[System.NonSerialized]
		public bool attachedToVehicle = false;
		[System.NonSerialized]
		public bool weaponEquiped = false;
		public float reloadTime = 1f;
		public float fireRate = 1f;
		public float zoomMagn = 4;
		public float zoomSpeed = 5;
		public Vector3 normPosition;
		public Vector3 aimPosition;
		public float proj_speed = 50;
		public float proj_arc = 0;
		public Vector3 proj_size = new Vector3(1, 1, 1);
		public float weaponSway = 5f;
		public float loudness = 30;
		public Vector3 scale = new Vector3();
		public Quaternion rotation = new Quaternion();
		public Vector3 position = new Vector3();
		public float cost = 1999.00f;
	}
	public WeaponAndProj weapon = new WeaponAndProj();

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

	/* Effects */
	[System.Serializable]
	public class WeaponEffects{
		public Transform m_ParticleTransform;
		public ParticleSystem m_FireParticles;        	// Reference to the particles that will play on firing.
		public ParticleSystem m_SmokeParticles;			// Reference to the particles that will play after firing.
		public AudioSource m_FireAudio;                	// Reference to the audio that will play on firing.
		public AudioSource m_ReloadAudio;              	// Reference to the audio that will play on reloading.
		public AudioSource m_PullbackAudio;             // Reference to the audio that will play on pulling bolt back.
		public AudioSource m_EmptyAudio;           		// Reference to the audio that will play on empty clip.
		[System.NonSerialized]
		public float m_OriginalPitch = 1;
		[System.NonSerialized]
		public float m_PitchRange = 0.2f;
	}
	public WeaponEffects effects = new WeaponEffects();


	/* 'Constructor' Methods */
	public void create(WeaponAndProj weapon, WeaponAmmo ammo, WeaponEffects effects ){
		this.weapon = weapon;
		this.ammo = ammo;
		this.effects = effects;
	}

	public void setWeaponAttr( Animator m_ani, string aniIndex, Transform m_fireTrans, float fireRate, float reloadTime, float zoomMagn, float zoomSpeed, Vector3 aimPos, float projSpeed, float projArc, float sway, float loudness, Vector3 scale, Quaternion rotation, Vector3 position, float cost ){
		this.weapon.m_Anim = m_ani;
		this.weapon.aniIndex = aniIndex;
		this.weapon.m_FireTransform = m_fireTrans;
		this.weapon.fireRate = fireRate;
		this.weapon.reloadTime = reloadTime;
		this.weapon.zoomMagn = zoomMagn;
		this.weapon.zoomSpeed = zoomSpeed;
		this.weapon.aimPosition = aimPos;
		this.weapon.proj_speed = projSpeed;
		this.weapon.proj_arc = projArc;
		this.weapon.weaponSway = sway;
		this.weapon.loudness = loudness;
		this.weapon.scale = scale;
		this.weapon.rotation = rotation;
		this.weapon.position = position;
		this.weapon.cost = cost;
	}

	public void setAmmoAttr( string type, int clipSize, int maxClips ){
		this.ammo.ammoType = type;
		this.ammo.clipSize = clipSize;
		this.ammo.maxClips = maxClips;
	}

	public void setEffectsAttr( Transform fireLocation, AudioSource m_fire, AudioSource m_reload, AudioSource m_pull, AudioSource m_empty ){
		this.effects.m_ParticleTransform = fireLocation;
		this.effects.m_FireAudio = m_fire;
		this.effects.m_ReloadAudio = m_reload;
		this.effects.m_PullbackAudio = m_pull;
		this.effects.m_EmptyAudio = m_empty;
	}

	// Use this for initialization
	void Start () {
	
	}

}
