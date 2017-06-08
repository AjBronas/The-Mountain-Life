using UnityEngine;
using System.Collections;

public class TrigMover : MonoBehaviour {

	[System.Serializable]
	public class XInput{
		public bool m_enabled = false;
		public bool m_Sin = false;
		public bool m_Cos = false;
		public bool m_Tan = false;
		public float m_amp=1, m_frequency=1, m_phase=Mathf.PI;
	}
	public XInput x_input = new XInput();

	[System.Serializable]
	public class YInput{
		public bool m_enabled = false;
		public bool m_Sin = false;
		public bool m_Cos = false;
		public bool m_Tan = false;
		public float m_amp=1, m_frequency=1, m_phase=Mathf.PI;
	}
	public YInput y_input = new YInput();

	[System.Serializable]
	public class ZInput{
		public bool m_enabled = false;
		public bool m_Sin = false;
		public bool m_Cos = false;
		public bool m_Tan = false;
		public float m_amp=1, m_frequency=1, m_phase=Mathf.PI;
	}
	public ZInput z_input = new ZInput();


	Vector3 startPos;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
		float x = startPos.x;
		float y = startPos.y;
		float z = startPos.z;

		if (x_input.m_enabled) {
			if(x_input.m_Sin)
				x += getSin(x_input.m_amp,x_input.m_frequency,x_input.m_phase);
			if(x_input.m_Cos)
				x += getCos(x_input.m_amp,x_input.m_frequency,x_input.m_phase);
			if(x_input.m_Tan)
				x += getTan(x_input.m_amp,x_input.m_frequency,x_input.m_phase);
		}

		if (y_input.m_enabled) {
			if(y_input.m_Sin)
				y += getSin(y_input.m_amp,y_input.m_frequency,y_input.m_phase);
			if(y_input.m_Cos)
				y += getCos(y_input.m_amp,y_input.m_frequency,y_input.m_phase);
			if(y_input.m_Tan)
				y += getTan(y_input.m_amp,y_input.m_frequency,y_input.m_phase);
		}

		if (z_input.m_enabled) {
			if(z_input.m_Sin)
				z += getSin(z_input.m_amp,z_input.m_frequency,z_input.m_phase);
			if(z_input.m_Cos)
				z += getCos(z_input.m_amp,z_input.m_frequency,z_input.m_phase);
			if(z_input.m_Tan)
				z += getTan(z_input.m_amp,z_input.m_frequency,z_input.m_phase);
		}

		transform.position = new Vector3(x,y,z);
	}

	public static float getSin(float amp, float freq, float phase ){
		return amp * Mathf.Sin (Time.timeSinceLevelLoad * freq + phase);
	}

	public static float getCos(float amp, float freq, float phase ){
		return amp * Mathf.Cos (Time.timeSinceLevelLoad * freq + phase);
	}

	public static float getTan(float amp, float freq, float phase ){
		return amp * Mathf.Tan (Time.timeSinceLevelLoad * freq + phase);
	}
}
