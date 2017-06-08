using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
//using UnityEditor;

using System.Collections;

public class MenuBuilder : MonoBehaviour {

	public const string LEFT = "Left", RIGHT = "Right", CENTER = "Center", FAR_LEFT = "FarLeft", FAR_RIGHT = "FarRight";
	protected GameObject canvas;
	private Vector3 nextPosition;
	public Button buttonObject;
	public Text textObject;
	public Image imageObject;
	public XmlImporter xml;
	public StoreListings store;
	protected float height;
	protected float width;
	protected string lastPosition;

	private Vector3 startPosition;

	// Use this for initialization
	void Start () {
		lastPosition = CENTER;
		height = buttonObject.GetComponent<RectTransform> ().rect.height;
		width = buttonObject.GetComponent<RectTransform> ().rect.width;
		nextPosition += Vector3.up * height*4;
		nextPosition += Vector3.right * width*1f;
		MovePosition (CENTER);
		startPosition = nextPosition;
		canvas = GameObject.Find ("Canvas");
		xml = GameObject.Find ("Spawner").GetComponent<XmlImporter> ();
		store = GameObject.Find ("Spawner").GetComponent<StoreListings> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void openStore(){
		Weapon weapon = GameObject.FindGameObjectWithTag("Player").GetComponent<Weapon> ();
		weapon.variables.enabled = false;
		GameObject canvas = GameObject.Find ("StoreCanvas");
		canvas.GetComponent<Canvas> ().enabled = true;
	}

	public void closeStore(){
		Weapon weapon = GameObject.FindGameObjectWithTag("Player").GetComponent<Weapon> ();
		weapon.variables.enabled = true;
		GameObject canvas = GameObject.Find ("StoreCanvas");
		canvas.GetComponent<Canvas> ().enabled = false;
	}

	public void FindCanvas( string name ){
		canvas = GameObject.Find (name);
	}

	public void clearStore(){
		Image[] imageObjs = canvas.GetComponentsInChildren<Image> ();
		for (int i = 0; i < imageObjs.GetLength(0); i++) {
			Destroy(imageObjs[i].gameObject);
		}
		Text[] textObjs = canvas.GetComponentsInChildren<Text> ();
		for (int i = 0; i < textObjs.GetLength(0); i++) {
			Destroy(textObjs[i].gameObject);
		}
		Button[] buttonObjs = canvas.GetComponentsInChildren<Button> ();
		for (int i = 0; i < buttonObjs.GetLength(0); i++) {
			Destroy(buttonObjs[i].gameObject);
		}
		nextPosition = startPosition;
	}

	public void fillBasic(){
		clearStore ();
		// Rifles
		AddImage("Rifles"+"Image","m48Image2",MenuBuilder.FAR_LEFT);
		AddText("Rifles"+"Text","Rifles",MenuBuilder.LEFT);
		AddText("Rifles"+"TextCost","Purchase a new weapon...",MenuBuilder.CENTER);
		AddButton ("Rifles"+"Button", "View", () =>{this.fillRifles();}, null, 0, MenuBuilder.RIGHT );
		MovePositionDown ();

		// Ammo
		AddImage("Ammo"+"Image","bullet308Image",MenuBuilder.FAR_LEFT);
		AddText("Ammo"+"Text","Ammo",MenuBuilder.LEFT);
		AddText("Ammo"+"TextCost","Purchase ammo...",MenuBuilder.CENTER);
		AddButton ("Ammo"+"Button", "View", () =>{this.fillAmmo();}, null, 0, MenuBuilder.RIGHT );
		MovePositionDown ();

		// Scopes
		AddImage("Scopes"+"Image","scopeImage01",MenuBuilder.FAR_LEFT);
		AddText("Scopes"+"Text","Scopes",MenuBuilder.LEFT);
		AddText("Scopes"+"TextCost","Purchase a scope...",MenuBuilder.CENTER);
		AddButton ("Scopes"+"Button", "View", () =>{this.fillScopes();}, null, 0, MenuBuilder.RIGHT );
		MovePositionDown ();

	}

	public void fillRifles(){
		clearStore ();
		for( int i = 0; i < xml.weaponList.Count; i++ ){
			string name = xml.weaponList [i].ToString();
			string cost = xml.variablesList [i]["Cost"].ToString ();
			//Button prefab = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets\\Resources\\" + "purchaseB" + ".prefab", typeof(Button));
			AddImage(name+"Image","bullet308Image",MenuBuilder.FAR_LEFT);
			AddText(name+"Text",name,MenuBuilder.LEFT);
			AddText(name+"TextCost","$" + cost,MenuBuilder.CENTER);

			System.Action<int> calls = xml.readWeaponData;
			AddButton (name+"Button", "Purchase", null, calls, i, MenuBuilder.RIGHT );
			MovePositionDown ();
		}
	}

	public void fillAmmo(){
		clearStore ();
		for( int i = 0; i < store.ammo.Count; i++ ){
			string name = store.ammo [i].name;
			string cost = "$"+store.ammo [i].getCost();
			AddImage(name+"Image","bullet308Image",MenuBuilder.FAR_LEFT);
			AddText(name+"Text",name,MenuBuilder.LEFT);
			AddText(name+"TextCost", cost,MenuBuilder.CENTER);

			System.Action<int> calls = xml.readWeaponData;
			AddButton (name+"Button", "Purchase", null, calls, i, MenuBuilder.RIGHT );
			MovePositionDown ();
		}
	}

	public void fillScopes(){
		clearStore ();
		for( int i = 0; i < store.scopes.Count; i++ ){
			string name = store.scopes [i].name;
			string cost = "$"+store.scopes [i].getCost();
			AddImage(name+"Image","bullet308Image",MenuBuilder.FAR_LEFT);
			AddText(name+"Text",name,MenuBuilder.LEFT);
			AddText(name+"TextCost",cost,MenuBuilder.CENTER);

			System.Action<int> calls = xml.readWeaponData;
			AddButton (name+"Button", "Purchase", null, calls, i, MenuBuilder.RIGHT );
			MovePositionDown ();
		}
	}

	public void fillMisc(){
		clearStore ();
	}

	public void MovePositionDown(){
		nextPosition += Vector3.down * height*1.5f;
	}

	public void MovePosition( string position ){
		if (lastPosition == CENTER) {
			if (position == LEFT)
				nextPosition += Vector3.left * width*1f;
			if (position == RIGHT)
				nextPosition += Vector3.right * width*1f;
			if (position == FAR_LEFT)
				nextPosition += Vector3.left * width*2f;
			if (position == FAR_RIGHT)
				nextPosition += Vector3.right * width*2f;
		}
		if (lastPosition == LEFT) {
			if (position == CENTER)
				nextPosition += Vector3.right * width*1f;
			if (position == RIGHT)
				nextPosition += Vector3.right * width*2f;
			if (position == FAR_LEFT)
				nextPosition += Vector3.left * width*1f;
			if (position == FAR_RIGHT)
				nextPosition += Vector3.right * width*3f;
		}
		if (lastPosition == RIGHT) {
			if (position == LEFT)
				nextPosition += Vector3.left * width*2f;
			if (position == CENTER)
				nextPosition += Vector3.left * width*1f;
			if (position == FAR_LEFT)
				nextPosition += Vector3.left * width*3;
			if (position == FAR_RIGHT)
				nextPosition += Vector3.right * width*1f;
		}
		if (lastPosition == FAR_LEFT) {
			if (position == CENTER)
				nextPosition += Vector3.right * width*2f;
			if (position == LEFT)
				nextPosition += Vector3.right * width*1f;
			if (position == RIGHT)
				nextPosition += Vector3.right * width*3f;
			if (position == FAR_RIGHT)
				nextPosition += Vector3.right * width*4f;
		}
		if (lastPosition == FAR_RIGHT) {
			if (position == LEFT)
				nextPosition += Vector3.left * width*3f;
			if (position == CENTER)
				nextPosition += Vector3.left * width*2f;
			if (position == RIGHT)
				nextPosition += Vector3.left * width*1f;
			if (position == FAR_LEFT)
				nextPosition += Vector3.left * width*4f;
		}
	}

	public void DoMethod(System.Action<int> method, int index){
		method(index);
	}

	public void AddButton(string name, string val, UnityAction callback, System.Action<int> callbackInt, int value, string position){
		Button button = Instantiate(buttonObject);
		button.name = name;
		if( callback == null )
			button.onClick.AddListener(delegate{callbackInt(value);});
		else
			button.onClick.AddListener(callback);

		Text text = button.GetComponentInChildren<Text>();
		if (text)
			text.text = val;

		RectTransform transform = button.GetComponent<RectTransform>();
		transform.SetParent(canvas.transform);
		transform.localScale = new Vector3 (1, 1, 1);

//		transform.anchorMin = new Vector2(0.45f, 0.45f);
//		transform.anchorMax = new Vector2(0.55f, 0.55f);
		height = transform.rect.height;
		width = transform.rect.width;
		MovePosition (position);
		transform.anchoredPosition = nextPosition;
		lastPosition = position;
	}

	public void AddText(string name, string value, string position){
		Text textComponent = Instantiate(textObject);
		textComponent.name = name;
		textComponent.text = value;

		RectTransform transform = textComponent.GetComponent<RectTransform>();
		transform.SetParent(canvas.transform);
		transform.localScale = new Vector3 (1, 1, 1);

//		transform.anchorMin = new Vector2(0.45f, 0.45f);
//		transform.anchorMax = new Vector2(0.55f, 0.55f);
		height = transform.rect.height;
		width = transform.rect.width;
		MovePosition (position);
		transform.anchoredPosition = nextPosition;
		transform.anchoredPosition = new Vector2 (transform.anchoredPosition.x, transform.anchoredPosition.y - height/4);
		lastPosition = position;
	}

	public void AddImage(string name, string image, string position){
		Image imageComponent = Instantiate(imageObject);
		imageComponent.name = name;
		imageComponent.GetComponent<Image> ().sprite = (Resources.Load<Sprite>(image));

		RectTransform transform = imageComponent.GetComponent<RectTransform>();
		transform.SetParent(canvas.transform);
		transform.localScale = new Vector3 (1, 1, 1);

//		transform.anchorMin = new Vector2(0.45f, 0.45f);
//		transform.anchorMax = new Vector2(0.55f, 0.55f);
		height = transform.rect.height;
		width = transform.rect.width;
		MovePosition (position);
		transform.anchoredPosition = nextPosition;
		lastPosition = position;
	}

	public void AddTextButton(Text textPrefab, string name, UnityAction callback){
		Text textObject = Instantiate(textPrefab);
		textObject.name = name;

		textObject.gameObject.AddComponent<Button>();
		textObject.gameObject.GetComponent<Button>().onClick.AddListener(callback);

		textObject.text = name;

		RectTransform transform = textObject.GetComponent<RectTransform>();
		transform.SetParent(canvas.transform);
		transform.anchoredPosition = nextPosition;
		nextPosition += Vector3.down * transform.rect.height;
	}
}
