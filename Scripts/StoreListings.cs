using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoreListings : MonoBehaviour {

	public List<Ammo> ammo = new List<Ammo>();
	public List<Scope> scopes = new List<Scope>();
	public List<Ammo> misc = new List<Ammo>();

	void Awake(){
		foreach( string vals in Ammo.types ){
			Ammo newAmmo = new Ammo ();
			newAmmo.setType (vals);
			ammo.Add (newAmmo);
		}
		foreach( string vals in Scope.types ){
			Scope newScope = new Scope ();
			newScope.setType (vals);
			scopes.Add (newScope);
		}
	}

}
