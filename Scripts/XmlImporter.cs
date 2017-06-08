using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class XmlImporter : MonoBehaviour
{
	const string LOCATION = "C:\\Users\\ajbro\\Documents\\CastleDestroyer\\";
	const string AUDIOCLIPS = "AudioClips\\";
	const string WEAPONS = "weapons.txt";

	XmlDocument doc = new XmlDocument ();
	public List<Hashtable> ammoList = new List<Hashtable>();
	public List<Hashtable> effectsList = new List<Hashtable>();
	public List<Hashtable> variablesList = new List<Hashtable>();
	public List<string> weaponList = new List<string> ();
	//public Hashtable weaponList = new Hashtable();

	void Start (){
	
		//getWeaponData ();
		//readWeaponData ();
	}

	public void getWeaponData(){
		doc.Load (LOCATION + WEAPONS);
		foreach (XmlNode item in doc.DocumentElement) {
			string weapon = "";
			string name = "";
			weapon = item.Name;
			if (item.Attributes ["name"] != null)
				name = item.Attributes ["name"].InnerText;


			/*--Get Ammo--*/
			Hashtable ammoTable = new Hashtable ();
			XmlNodeList ammoNodeList = doc.GetElementsByTagName("Ammo");
			if (ammoNodeList != null && ammoNodeList.Item (0) != null && ammoNodeList.Item(0).Attributes ["type"] != null) {
				ammoTable.Add ("type", ammoNodeList.Item (0).Attributes ["type"].InnerText);	// Add ammo type attribute
			}
			foreach (XmlNode node in ammoNodeList) {
				
				if (node != null && node.ParentNode.Attributes ["name"] != null) {
					if (node.ParentNode.Attributes ["name"].InnerText == name) {
						foreach (XmlNode child in node.ChildNodes) {
							print (node.LocalName + "-> childNode: " + child.Name + ", Value: " + child.InnerText);
							ammoTable.Add (child.Name, child.InnerText);
						}
					}
				}
			}
			/*--Get Effects--*/
			Hashtable effectsTable = new Hashtable ();
			XmlNodeList effectsNodeList = doc.GetElementsByTagName("Effects");
			foreach (XmlNode node in effectsNodeList) {
				if (node != null && node.ParentNode.Attributes ["name"] != null) {
					if (node.ParentNode.Attributes ["name"].InnerText == name) {
						foreach (XmlNode child in node.ChildNodes) {
							print (node.LocalName + "-> childNode: " + child.Name + ", Value: " + child.InnerText);
							effectsTable.Add (child.Name, child.InnerText);
						}
					}
				}
			}
			/*--Get Variables--*/
			Hashtable varsTable = new Hashtable ();
			XmlNodeList varsNodeList = doc.GetElementsByTagName("Variables");
			foreach (XmlNode node in varsNodeList) {
				if (node != null && node.ParentNode.Attributes ["name"] != null) {
					if (node.ParentNode.Attributes ["name"].InnerText == name) {
						foreach (XmlNode child in node.ChildNodes) {
							print (node.LocalName + "-> childNode: " + child.Name + ", Value: " + child.InnerText);
							varsTable.Add (child.Name, child.InnerText);
						}
					}
				}
			}

			ammoList.Add (ammoTable);
			effectsList.Add (effectsTable);
			variablesList.Add (varsTable);
			print ("Title: " + weapon + ", Name: " + name);
			item.RemoveAll ();
			weaponList.Add (name);
		}

	}

	public void readWeaponData( int index ){
		if (index < weaponList.Count && index >= 0) {
			Hashtable vList = ((Hashtable)variablesList [index]);
			EquipableWeapon eWeapon = new EquipableWeapon ();
			eWeapon.setAmmoAttr (((Hashtable)ammoList [index]) ["type"].ToString (), int.Parse (((Hashtable)ammoList [index]) ["ClipSize"].ToString ()), int.Parse (((Hashtable)ammoList [index]) ["MaxClips"].ToString ()));
			//eWeapon.weapon.m_Anim = StringToAnimator(((Hashtable)variablesList [0]) ["Animation"].ToString ());

			Transform fireLoc = StringToTransform (((Hashtable)effectsList [index]) ["FireLocation"].ToString ());
			eWeapon.setEffectsAttr (fireLoc, CreateAudioSource (((Hashtable)effectsList [index]) ["FireAudio"].ToString ()), CreateAudioSource (((Hashtable)effectsList [index]) ["FireAudio"].ToString ()), CreateAudioSource (((Hashtable)effectsList [index]) ["FireAudio"].ToString ()), CreateAudioSource (((Hashtable)effectsList [index]) ["FireAudio"].ToString ()));
			Animator ani = StringToPrefab (((Hashtable)variablesList [index]) ["Animation"].ToString ());
			//Animator ani2 = CreateAnimator (((Hashtable)variablesList[index])["Animation"].ToString());
			eWeapon.setWeaponAttr (ani, vList ["AniIndex"].ToString (), null, float.Parse (vList ["FireRate"].ToString ()), float.Parse (vList ["ReloadTime"].ToString ()), float.Parse (vList ["ZoomMagn"].ToString ()), float.Parse (vList ["ZoomSpeed"].ToString ()), StringToVector (vList ["AimPosition"].ToString ()), float.Parse (vList ["ProjSpeed"].ToString ()), float.Parse (vList ["ProjArc"].ToString ()), float.Parse (vList ["Sway"].ToString ()), float.Parse (vList ["Loudness"].ToString ()), StringToVector (vList ["Scale"].ToString ()), StringToQuaternion (vList ["Rotation"].ToString ()), StringToVector (vList ["Position"].ToString ()), float.Parse(vList["Cost"].ToString()));

			GameObject player = GameObject.FindGameObjectWithTag ("Player");
			Weapon weaponScript = player.GetComponentInChildren<Weapon> ();
			weaponScript.setEquipped (eWeapon);
		}
		
	}

	public int getWeaponByName( string name ){
		for (int i = 0; i < weaponList.Count; i++) {
			if (weaponList [i].Equals(name))
				return i;
		}
		Debug.Log ("Could not find weapon by name: " + name);
		return 0;
	}

	public string getWeaponByIndex( int index ){
		return weaponList [index].ToString ();
	}

	static Vector3 StringToVector( string data ){
		Vector3 vector;
		string[] values = data.Split(',');
		if (values.Length != 3)
			throw new System.FormatException("Vector3 mismatch. Expected 3 but got " + values.Length);
		vector = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
		return vector;
	}

	static Quaternion StringToQuaternion( string data ){
		Quaternion quaternion;
		string[] values = data.Split(',');
		if (values.Length != 4)
			throw new System.FormatException("Quaternion mismatch. Expected 4 but got " + values.Length);
		quaternion = new Quaternion(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[2]));
		return quaternion;
	}

	static Animator StringToAnimator( string data ){
		Animator animator = null;
		GameObject weapon = GameObject.Find(data);
		if( weapon == null )
			print ("StringToAnimator: Could not find GameObject");
		animator = weapon.GetComponentInChildren<Animator> ();
		if( animator == null )
			print ("StringToAnimator: Could not find animator " + data);
		return animator;
	}

	static Animator StringToPrefab( string data ){

		//Object prefab = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets\\Resources\\" + data + ".prefab", typeof(GameObject));
		Object prefab = Resources.Load(data) as GameObject;
		if( prefab == null )
			print ("StringToPrefab: Could not find prefab at Resources/" + data + ".prefab");
		GameObject clone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
		return clone.GetComponent<Animator>();
	}

	static Transform StringToTransform( string data ){
		Transform transform = null;
		GameObject weapon = GameObject.Find(data);
		if( weapon == null )
			print ("StringToTransform: Could not find GameObject");
		transform = weapon.transform;
		if( transform == null )
			print ("StringToTransform: Could not find transform " + data);
		return transform;
	}

	static AudioSource StringToAudioSource( string data ){
		AudioSource audio = null;
		GameObject weapon = GameObject.Find(data);
		if( weapon == null )
			print ("StringToAudioSource: Could not find GameObject");
		audio = weapon.GetComponentInChildren<AudioSource> ();
		if( audio == null )
			print ("StringToAudioSource: Could not find audio " + data);
		return audio;
	}

	static ParticleSystem StringToParticleSystem( string data ){
		ParticleSystem particles = null;
		GameObject weapon = GameObject.Find(data);
		if( weapon == null )
			print ("StringToParticleSystem: Could not find GameObject");
		particles = weapon.GetComponentInChildren<ParticleSystem> ();
		if( particles == null )
			print ("StringToParticleSystem: Could not find particles " + data);
		return particles;
	}

	static AudioSource CreateAudioSource( string data ){
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		AudioSource audioSource = player.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
		audioSource.clip = Resources.Load(AUDIOCLIPS + data) as AudioClip;
		return audioSource;
	}

	static Animator CreateAnimator( string data ){
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		Animator animator = player.AddComponent<Animator>();
		animator = Resources.Load(data) as Animator;
		return animator;
	}


}
