using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WallBuilderUtil : MonoBehaviour {

	public bool m_CreateOnStart = true;
	public float m_StartXPosition = 0;
	public float m_StartYPosition = 0;
	public float m_StartZPosition = 0;
	public int m_Width = 10;
	public int m_Height = 10;
	public int m_Length = 1;
	public bool m_usePrefabSize = true;
	public float m_BrickWidth = 1;
	public float m_BrickHeigth = 0.6f;
	public float m_BrickLength = 0.7f;
	public float m_positionAdjustments = 0f;
	public GameObject m_Brick;

	[System.NonSerialized]
	public int brickLoadedCount = 0;
	private int brickCount = 0;
	private bool m_Loaded = false;
	private GameObject loadingScreen;

	// Use this for initialization
	void Start () {

		loadingScreen = GameObject.Find ("LoadingScreen");

		//Quaternion rotation = new Quaternion (90, 0, 90, 0);
		Renderer brick;
		if (m_Brick == null)
			brick = Resources.Load<GameObject> ("Brick").GetComponent<Renderer> ();
		else
			brick = m_Brick.GetComponent<Renderer> ();

		if(m_CreateOnStart)
			CreateBuilding(brick, m_StartXPosition, m_StartYPosition, m_StartZPosition, m_Width, m_Height, m_Length, 0, Quaternion.identity);
	
		/*
		CreateWall( m_StartXPosition, m_StartYPosition, m_StartZPosition, m_Width, m_Height, m_Length, 0, Quaternion.identity);
		CreateWall (m_StartXPosition, m_StartYPosition, m_StartZPosition, m_Width, m_Height, m_Length, 1, rotation);
		CreateWall (m_StartXPosition, m_StartYPosition, m_StartZPosition + getWallWidth(brick, m_Width, true), m_Width, m_Height, m_Length, 1, Quaternion.identity);
		CreateWall (m_StartXPosition + getWallWidth(brick, m_Width, true), m_StartYPosition, m_StartZPosition, m_Width, m_Height, m_Length, 0, rotation);
		*/
	}
	
	// Update is called once per frame
	void Update () {

		if (m_Loaded == false) {
			if(loadingScreen.GetComponentInChildren<Text>() != null)
				loadingScreen.GetComponentInChildren<Text> ().text = "Loading... " + ((float)brickLoadedCount/(float)brickCount*100f) + "%";
			if (brickLoadedCount >= brickCount) {
				
				//Loaded
				Debug.Log ("Loaded");
				m_Loaded = true;
				if(loadingScreen != null)
					loadingScreen.SetActive(false);
			}
		}
	}

	public void CreateWall( float x, float y, float z, int width, int height, int length, int odd, Quaternion rotation ){

		float b_Width = m_BrickWidth;
		float b_Heigth = m_BrickHeigth;
		float b_Length = m_BrickLength;

		if (m_Brick == null)
			m_Brick = Resources.Load<GameObject> ("Brick");

		for (int w = 0; w < width; w++) {
			for (int h = 0; h < height; h++) {
				for (int l = 0; l < length; l++) {
					Vector3 position;
					if( rotation == Quaternion.identity )
						position = new Vector3 (x+(w*(m_Brick.GetComponent<Renderer>().bounds.size.x+m_positionAdjustments)+(h%2==odd? 0 : m_Brick.GetComponent<Renderer>().bounds.size.x/2)), y+(h*(m_Brick.GetComponent<Renderer>().bounds.size.y+m_positionAdjustments)), z+(l*(m_Brick.GetComponent<Renderer>().bounds.size.z+m_positionAdjustments)));
					else
						position = new Vector3 ( x+(l*(m_Brick.GetComponent<Renderer>().bounds.size.z+m_positionAdjustments)), y+(h*(m_Brick.GetComponent<Renderer>().bounds.size.y+m_positionAdjustments)),z+(w*(m_Brick.GetComponent<Renderer>().bounds.size.x+m_positionAdjustments)+(h%2==odd? 0 : m_Brick.GetComponent<Renderer>().bounds.size.x/2)));
					
					GameObject newProj = Instantiate (m_Brick, position, rotation) as GameObject;
					brickLoadedCount++;
					brickCount+=2;
					//newProj.GetComponent<Renderer> ().material.color = color;
					if (m_usePrefabSize == false)
						newProj.transform.localScale = new Vector3 (b_Width, b_Heigth, b_Length);
				}
			}
		}
	}

	public void CreateWall( Vector3 pos, int width, int height, int length, int odd, Quaternion rotation ){

		CreateWall (pos.x, pos.y, pos.z, width, height, length, odd, rotation);
	}

	public void CreateBuilding( Renderer brick, float x, float y, float z, int width, int height, int length, int odd, Quaternion rotation ){

		Quaternion rot = new Quaternion (90, 0, 90, 0);

		CreateWall (x, y, z, width, height, length, odd, rotation);
		CreateWall (x, y, z, width, height, length, odd+1, rot);
		CreateWall (x, y, z + getWallWidth(brick, m_Width, true), width, height, length, odd+1, rotation);
		CreateWall (x + getWallWidth(brick, m_Width, true), y, z, width, height, length, odd, rot);



	
	}

	public float getWallWidth( Renderer brick, int width, bool interweaved  ){
		return brick.bounds.size.x * width - (interweaved == false ? 0 : brick.bounds.size.z / 2);
	}

	public float getWallLength( Renderer brick, int width, bool interweaved ){
		return brick.bounds.size.z * width - (interweaved == false ? 0 : brick.bounds.size.z / 2);
	}
}
