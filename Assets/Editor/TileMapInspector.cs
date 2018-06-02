using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TileMap))]
public class TileMapInspector : Editor {
	
	public override void OnInspectorGUI() {
		//base.OnInspectorGUI();
		DrawDefaultInspector();

		if(GUILayout.Button("Regenerate")) {
			TileMap tileMap = (TileMap)target;
			tileMap.BuildMesh();
		}
	}
}
