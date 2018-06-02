using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour {
	
	public int mapWidth = 60;
	public int mapHeight = 60;
	public int roomWidthMin = 6, roomWidthMax = 14;
	public int roomHeightMin = 6, roomHeightMax = 14;
	public int numRooms = 16;
	public int maxFails = 40;
	
	public string IMPORTANT;
	public Texture2D terrainTiles;
	public float tileSize = 1.0f;
	public int tileResolution = 32;

	public DTileMapNew1 map;

	// Use this for initialization
	void Start () {
		Debug.Log("[][][][][][][][][][][][][][]");
		BuildMesh();
	}
	
	Color[][] ChopUpTiles() {
		int numTilesPerRow = terrainTiles.width / tileResolution;
		int numRows = terrainTiles.height / tileResolution;
		
		Color[][] tiles = new Color[numTilesPerRow*numRows][];
		
		for(int y = 0; y < numRows; y++) {
			for(int x=0; x<numTilesPerRow; x++) {
				tiles[(numRows-y-1)*numTilesPerRow + x] = terrainTiles.GetPixels( x*tileResolution , y*tileResolution, tileResolution, tileResolution );
			}
		}

		return tiles;
	}
	
	public void BuildTexture() {
		map = new DTileMapNew1(mapWidth, mapHeight, roomWidthMin, roomWidthMax, roomHeightMin, roomHeightMax, numRooms, maxFails);
		
		int texWidth = mapWidth * tileResolution;
		int texHeight = mapHeight * tileResolution;
		Texture2D texture = new Texture2D(texWidth, texHeight);
		
		Color[][] tiles = ChopUpTiles();
		
		for(int y=0; y < mapHeight; y++) {
			for(int x=0; x < mapWidth; x++) {
				Color[] p = tiles[ map.GetTileAt(x,y).getGraphicID() ];
				texture.SetPixels(x*tileResolution, y*tileResolution, tileResolution, tileResolution, p);
			}
		}
		
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply();
		
		MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
		mesh_renderer.sharedMaterials[0].mainTexture = texture;
		
		Debug.Log ("Done Texture!");
	}
	
	public void BuildMesh() {
		int numTiles = mapWidth * mapHeight;
		int numTris = numTiles * 2;
		
		int vsize_x = mapWidth + 1;
		int vsize_z = mapHeight + 1;
		int numVerts = vsize_x * vsize_z;
		
		// Generate the mesh data
		Vector3[] vertices = new Vector3[ numVerts ];
		Vector3[] normals = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];
		
		int[] triangles = new int[ numTris * 3 ];

		int x, z;
		for(z=0; z < vsize_z; z++) {
			for(x=0; x < vsize_x; x++) {
				vertices[ z * vsize_x + x ] = new Vector3( x*tileSize, 0, -z*tileSize );
				normals[ z * vsize_x + x ] = Vector3.up;
				uv[ z * vsize_x + x ] = new Vector2( (float)x / mapWidth, 1f - (float)z / mapHeight );
			}
		}
		Debug.Log ("Done Verts!");
		
		for(z=0; z < mapHeight; z++) {
			for(x=0; x < mapWidth; x++) {
				int squareIndex = z * mapWidth + x;
				int triOffset = squareIndex * 6;
				triangles[triOffset + 0] = z * vsize_x + x + 		   0;
				triangles[triOffset + 2] = z * vsize_x + x + vsize_x + 0;
				triangles[triOffset + 1] = z * vsize_x + x + vsize_x + 1;
				
				triangles[triOffset + 3] = z * vsize_x + x + 		   0;
				triangles[triOffset + 5] = z * vsize_x + x + vsize_x + 1;
				triangles[triOffset + 4] = z * vsize_x + x + 		   1;
			}
		}
		
		Debug.Log ("Done Triangles!");
		
		// Create a new Mesh and populate with the data
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
		
		// Assign our mesh to our filter/renderer/collider
		MeshFilter mesh_filter = GetComponent<MeshFilter>();
		MeshCollider mesh_collider = GetComponent<MeshCollider>();
		
		mesh_filter.mesh = mesh;
		mesh_collider.sharedMesh = mesh;
		Debug.Log ("Done Mesh!");
		
		BuildTexture();
	}
	
	
}
