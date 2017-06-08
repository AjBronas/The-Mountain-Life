using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public const string DEER = "DEER";
	public const string DOE = "DOE";

	public GameObject[] spawnPoints;
	GameObject animal;

	// Use this for initialization
	void Start () {

		for (int i = 0; i < spawnPoints.Length; i++) {
			Spawn (DEER, spawnPoints [i].transform.position, Color.white);
		}
		for (int i = 0; i < 15; i++) {
			Vector3 pos = new Vector3(Random.Range(-45,45), 2.1f, Random.Range(-45,45) );
			Spawn (DEER, pos, Color.white);
		}

		// Set rope projection mode and max depenatration velocity
		GameObject rope = GameObject.Find("rope/Armature");
		//GameObject bones = rope.transform.FindChild
		Rigidbody[] ropeBody = rope.GetComponentsInChildren<Rigidbody>();
		//HingeJoint[] ropeJoints = rope.GetComponentsInChildren<HingeJoint>();
		//Debug.Log ("Count:" + ropeBody.Length);
		for (int i = 0; i < ropeBody.Length; i++) {
			ropeBody [i].maxDepenetrationVelocity /= 2;
			//Debug.Log ("Depenetration:" + ropeBody[i].maxDepenetrationVelocity);
		}

	}

	public GameObject Spawn( string type, Vector3 position, Color color ){

		if (animal == null) {
			if( type == DEER )
				animal = Resources.Load<GameObject> ("deer_Complete");
			else if( type == DOE )
				animal = Resources.Load<GameObject> ("deer_Complete");
		}

		GameObject newAnimal = Instantiate (animal, position, Quaternion.identity) as GameObject;
		if( color != Color.white )
			newAnimal.GetComponentInChildren<SkinnedMeshRenderer> ().material.color = color;

		return newAnimal;
	}

}
