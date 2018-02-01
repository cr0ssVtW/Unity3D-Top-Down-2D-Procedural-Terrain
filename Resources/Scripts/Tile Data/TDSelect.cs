using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDSelect : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		

	}

	void OnGUI()
	{
		Vector2 p = new Vector2();
		Camera c = Camera.main;

		p = c.ScreenToWorldPoint(new Vector2(Mathf.Round(Input.mousePosition.x), Mathf.Round(Input.mousePosition.y)));

		GUILayout.BeginArea(new Rect(40, 400, 250, 120));
		GUILayout.Label("World position: " + p);
		GUILayout.EndArea();
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			
		}
	}

}
