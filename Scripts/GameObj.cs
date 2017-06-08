using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameObj : MonoBehaviour {

	public int m_Score1 = 0;
	public int m_Score2 = 0;
	public float m_Time = 60;
	public Text m_Text;
	public GameObject m_Ball;
	private bool timeRunning = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (timeRunning) {
			if (m_Time > 0) {
				m_Time -= Time.deltaTime;
			} else {
				timeRunning = false;

				for (int i = 0; i < 50; i++) {
					Vector3 pos = new Vector3 (0, 25+i, 0);
					if (m_Ball != null)
						Instantiate (m_Ball.GetComponent<Rigidbody> (), pos, gameObject.transform.rotation);
				
				}
			}
		}
	}

	public void Score(int team){
		if (team == 1)
			m_Score1++;
		else
			m_Score2++;

		if(m_Text != null)
			m_Text.text = "Team1: " + m_Score1 + "     Team2: " + m_Score2;
	}
}
