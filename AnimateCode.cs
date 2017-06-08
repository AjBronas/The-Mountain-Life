using UnityEngine;
using System.Collections;

public class AnimateCode : MonoBehaviour {

	public Animator m_Anim;
	int yourAwesomeAnimation = Animator.StringToHash("Fire");
	// Use this for initialization
	void Start () {
		//m_Anim.Stop ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			//UnityEngine.Experimental.Director.Playable p = new UnityEngine.Experimental.Director.Playable ();
			m_Anim.Play(yourAwesomeAnimation);
			//m_Anim.Play (p);
		}
	}
}
