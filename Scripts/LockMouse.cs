using UnityEngine;
using System.Collections;

public class LockMouse : MonoBehaviour {

	bool isLocked = true;

	void setLockMouse( bool isLocked ){
		this.isLocked = isLocked;
		if (isLocked) {
			Cursor.lockState = CursorLockMode.Locked;
		} else {
			Cursor.lockState = CursorLockMode.None;
		}
		Cursor.visible = !isLocked;
		GlobalVariables.lockMouse (isLocked);
	}

	void Start() {

		setLockMouse (true);

	}

	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyDown(KeyCode.Escape)){
			setLockMouse(!isLocked);
		}
	}
}
