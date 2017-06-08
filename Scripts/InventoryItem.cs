using UnityEngine;
using System.Collections;

public class InventoryItem {
	
	public string name = "";
	public string description = "";
	public bool equippable = false;
	public Sprite image;
	public string imageSource = "bullet308Image";
	public GameObject model;
	public string modelSource = "Bullet308";
	public float cost = 9.95f;

	void Start () {
	
	}
}
