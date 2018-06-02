using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTypes {

	string name;
	int graphicID;
	bool walkable;
	int[] surroundingMatrix;

	public TileTypes(string myName, int gID, bool walk, int[] surrMx) {
		name = myName;
		walkable = walk;
		surroundingMatrix = surrMx;
		graphicID = gID;
	}

	public string getName() {
		return name;
	}

	public int getGraphicID() {
		return graphicID;
	}

	public int[] getSurrMatrix() {
		return surroundingMatrix;
	}

	public bool getWalkable() {
		return walkable;
	}

}
