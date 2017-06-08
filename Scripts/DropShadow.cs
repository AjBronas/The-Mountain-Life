using UnityEngine;
using System.Collections;

public class DropShadow : MonoBehaviour {

	public string resourceName = "PlayerShadow";
	private GameObject projector;
	public GameObject parent;
	public float size = 2;
	public float density = 1.5f;

	// Use this for initialization
	void Start () {
		if (projector == null)
			projector = GameObject.Find ("PlayerShadow");
		if (parent == null)
			parent = this.gameObject;
		
		//Object prefab = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets\\Resources\\" + resourceName + ".prefab", typeof(GameObject));
		Object prefab = Resources.Load(resourceName) as GameObject;
		if( prefab == null )
			print ("DropShadow: Could not find prefab at Resources\\" + resourceName);
		else
			projector = Instantiate(prefab, Vector3.zero, Quaternion.Euler(new Vector3(90,0,0))) as GameObject;
		//projector = prefab;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 position = parent.transform.position;
		projector.transform.position.Set(position.x, position.y + density, position.z);
		projector.GetComponent<Projector> ().transform.position = new Vector3(position.x, position.y + density, position.z);
		projector.GetComponent<Projector> ().orthographicSize = size;
	}
}
