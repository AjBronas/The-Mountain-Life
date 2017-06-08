using UnityEngine;
using System.Collections;

public class Scope : InventoryItem {
	public const string SCOPE1 = "AimTec Dawn", SCOPE2 = "AimTec Royce";
	public static string[] types = { SCOPE1, SCOPE2 };

	public string type = "AimTec Dawn";
	public float zoomMagn = 4;
	public Vector3 scopePosition;
	public string[] compatibility = { "M48" };

	void Start () {
		equippable = true;
	}

	public void setType( string scope ){
		this.name = scope;
		this.type = scope;
		if (scope == SCOPE1) {
			this.zoomMagn = 4;
			this.cost = 199.99f;
			this.imageSource = "bullet308Image";
			this.modelSource = "Bullet308";
			this.scopePosition = new Vector3 (0, 0, 0);
			this.compatibility = new string[]{ "M48" };
		} else if (scope == SCOPE2) {
			this.zoomMagn = 6;
			this.cost = 278.99f;
			this.imageSource = "bullet308Image";
			this.modelSource = "Bullet308";
			this.scopePosition = new Vector3 (0.14f, 0, 0);
			this.compatibility = new string[]{ "GEWEHR 48" };
		}
	}

	public float getCost(){
		return cost;
	}

}
