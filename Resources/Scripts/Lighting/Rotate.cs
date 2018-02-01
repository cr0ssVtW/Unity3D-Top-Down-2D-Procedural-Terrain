using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
	public Transform theLight;
	public float speed = 2f;
	public float duration = 1.0f;
	public Color color0 = Color.white;
	public Color color1 = Color.blue;
	Light light;
	// Use this for initialization
	void Start ()
	{
		if (theLight == null)
		{
			Debug.LogError("No transform found for Rotate script -- now disabled.");
			this.enabled = false;
		}

		light = GetComponent<Light>();

	}

	// Update is called once per frame
	void Update ()
	{
		theLight.Rotate(Vector3.left, speed * Time.deltaTime);
		float t = Mathf.PingPong(Time.time, duration) / duration;
		light.color = Color.Lerp(color0, color1, t);
	}
}
