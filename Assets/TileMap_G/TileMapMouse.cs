using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TileMap))]
public class TileMapMouse : MonoBehaviour {
	
	TileMap _tileMap;
	
	Vector3 currentTileCoord;

	int x, z;
	
	public Transform selectionCube;
	Collider _collider;
	void Start() {
		_tileMap = GetComponent<TileMap>();
		_collider = GetComponent<Collider>();
	}

	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;
		
		if( _collider.Raycast( ray, out hitInfo, Mathf.Infinity ) ) {
			x = Mathf.FloorToInt( hitInfo.point.x / _tileMap.tileSize);
			z = Mathf.FloorToInt( hitInfo.point.z / _tileMap.tileSize)+1;
			//Debug.Log ("Tile: " + x + ", " + z);
			
			currentTileCoord.x = x;
			currentTileCoord.z = z;
			
			selectionCube.transform.position = currentTileCoord*_tileMap.tileSize;
		}
		else {
			// Hide selection cube?
		}
		
		if(Input.GetMouseButtonDown(0)) {
			Debug.Log (x + ", " + z);
		}
	}
}
