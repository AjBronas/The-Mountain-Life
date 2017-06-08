using UnityEngine;
using System.Collections;

public class Ammo : InventoryItem {
	public const string RIFLE1 = "5.56", RIFLE2 = ".308";
	public static string[] types = { RIFLE1, RIFLE2 };

	// Cost: 5.56:9.95, .308:18.99
	public string type = "5.56";
	public float baseDamage = 0.75f;
	public float costPerBox = 9.95f;
	public int boxSize = 20;


	void Start () {
		equippable = false;
	}

	public void setType( string ammo ){
		this.name = ammo;
		this.type = ammo;
		if (ammo == RIFLE1) {
			this.baseDamage = 0.75f;
			this.costPerBox = 9.95f;
			this.cost = 9.95f;
			this.boxSize = 20;
			this.imageSource = "bullet308Image";
			this.modelSource = "Bullet308";
		} else if (ammo == RIFLE2) {
			this.baseDamage = 1;
			this.costPerBox = 18.99f;
			this.cost = 18.99f;
			this.boxSize = 20;
			this.imageSource = "bullet308Image";
			this.modelSource = "Bullet308";
		}
	}

	public float getCost(){
		return cost;
	}

}
