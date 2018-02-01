using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TGMap))]
public class TGMapInspector : Editor 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (GUILayout.Button("Regenerate"))
		{
            TGMap tileMap = (TGMap) target;
			tileMap.BuildMesh();
		}
	}
}
