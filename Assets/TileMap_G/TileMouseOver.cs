using UnityEngine;
using System.Collections;

public class TileMouseOver : MonoBehaviour {
	
	public Color highlightColor;
	Color normalColor;
	Renderer _renderer;
	Collider _collider;
	
	void Start() {
		_renderer = GetComponent<Renderer>();
		_collider = GetComponent<Collider>();
		normalColor = _renderer.material.color;
	}
	
	// Update is called once per frame
	void Update () {
		
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;
		
		if( _collider.Raycast( ray, out hitInfo, Mathf.Infinity ) ) {
			_renderer.material.color = highlightColor;
		}
		else {
			_renderer.material.color = normalColor;
		}
		
	}
	
}
