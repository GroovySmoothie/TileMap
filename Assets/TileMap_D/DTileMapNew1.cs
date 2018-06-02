using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTileMapNew1 {

	protected class DRoom
	{
		public int left;
		public int top;
		public int width;
		public int height;

		public bool isConnected = false;

		public int right {
			get { return left + width - 1; }
		}

		public int bottom {
			get { return top + height - 1; }
		}

		public int center_x {
			get { return left + width / 2; }
		}

		public int center_y {
			get { return top + height / 2; }
		}

		public bool CollidesWith(DRoom other) {
			if (left > other.right - 1)
				return false;

			if (top > other.bottom - 1)
				return false;

			if (right < other.left + 1)
				return false;

			if (bottom < other.top + 1)
				return false;

			return true;
		}


	}

	int size_x, size_y;
	TileTypes[,] map_data;

	List<DRoom> rooms;
	List<DRoom> unconnectedRooms;
	List<DRoom> edgeRooms;

	TileTypes[] tiles;

	public DTileMapNew1(int size_x, int size_y, int rxmin, int rxmax, int rymin, int rymax, int numRooms, int maxFails) {
		this.size_x = size_x;
		this.size_y = size_y;

		//wall = 0, floor = 1, dontcare = 2
		tiles = new TileTypes[] {
			new TileTypes("Stairs",		    0,	true,  new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 }),
			new TileTypes("Ground",			1,	true,  new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 }),
			new TileTypes("Background",		2,  false, new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 }),
			new TileTypes("Wall",			3,  false, new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 }),
			new TileTypes("leftWall",		4,  false, new int[]{ 0, 0, 2, 1, 2, 0, 0, 0 }),
			new TileTypes("RightWall",		5,  false, new int[]{ 2, 0, 0, 0, 0, 0, 2, 1 }),
			new TileTypes("TopWall",		6,  false, new int[]{ 0, 0, 0, 0, 2, 1, 2, 0 }),
			new TileTypes("BottomWall",		7,  false, new int[]{ 2, 1, 2, 0, 0, 0, 0, 0 }),
			new TileTypes("TopLeft",		8,  false, new int[]{ 0, 0, 0, 0, 1, 0, 0, 0 }),
			new TileTypes("TopRight",		9,  false, new int[]{ 0, 0, 0, 0, 0, 0, 1, 0 }),
			new TileTypes("BottomLeft",		10, false, new int[]{ 0, 0, 1, 0, 0, 0, 0, 0 }),
			new TileTypes("BottomRight",	11, false, new int[]{ 1, 0, 0, 0, 0, 0, 0, 0 }),
			new TileTypes("BotAndRight",	12, false, new int[]{ 0, 0, 2, 1, 2, 1, 2, 0 }),
			new TileTypes("BotAndLeft",		13, false, new int[]{ 2, 0, 0, 0, 2, 1, 2, 1 }),
			new TileTypes("TopAndLeft",		14, false, new int[]{ 2, 1, 2, 0, 0, 0, 2, 1 }),
			new TileTypes("TopAndRight",	15, false, new int[]{ 2, 1, 2, 1, 2, 0, 0, 0 }),
			new TileTypes("TBR",			16, false, new int[]{ 2, 1, 2, 1, 2, 1, 2, 0 }),
			new TileTypes("TBL",			17, false, new int[]{ 2, 1, 2, 0, 2, 1, 2, 1 }),
			new TileTypes("BLR",			18, false, new int[]{ 2, 0, 2, 1, 2, 1, 2, 1 }),
			new TileTypes("TLR",			19, false, new int[]{ 2, 1, 2, 1, 2, 0, 2, 1 }),
			new TileTypes("TopBottom",		20, false, new int[]{ 2, 1, 2, 0, 2, 1, 2, 0 }),
			new TileTypes("LeftRight",		21, false, new int[]{ 2, 0, 2, 1, 2, 0, 2, 1 }),
			new TileTypes("All",			22, false, new int[]{ 2, 1, 2, 1, 2, 1, 2, 1 }),
			new TileTypes("Corners",		23, false, new int[]{ 1, 0, 1, 0, 1, 0, 1, 0 }),
			new TileTypes("RightT",			24, false, new int[]{ 1, 0, 2, 1, 2, 0, 0, 0 }),
			new TileTypes("RightB",			25, false, new int[]{ 0, 0, 2, 1, 2, 0, 1, 0 }),
			new TileTypes("RightTB",		26, false, new int[]{ 1, 0, 2, 1, 2, 0, 1, 0 }),
			new TileTypes("LeftT",			27, false, new int[]{ 2, 0, 1, 0, 0, 0, 2, 1 }),
			new TileTypes("LeftB",			28, false, new int[]{ 2, 0, 0, 0, 1, 0, 2, 1 }),
			new TileTypes("LeftTB",			29, false, new int[]{ 2, 0, 1, 0, 1, 0, 2, 1 }),
			new TileTypes("BottomL",		30, false, new int[]{ 1, 0, 0, 0, 2, 1, 2, 0 }),
			new TileTypes("BottomR",		31, false, new int[]{ 0, 0, 1, 0, 2, 1, 2, 0 }),
			new TileTypes("BottomLR",		32, false, new int[]{ 1, 0, 1, 0, 2, 1, 2, 0 }),
			new TileTypes("TopL",			33, false, new int[]{ 2, 1, 2, 0, 0, 0, 1, 0 }),
			new TileTypes("TopR",			34, false, new int[]{ 2, 1, 2, 0, 1, 0, 0, 0 }),
			new TileTypes("TopLR",			35, false, new int[]{ 2, 1, 2, 0, 1, 0, 1, 0 }),
			new TileTypes("CornerBR",		36, false, new int[]{ 2, 1, 2, 0, 1, 0, 2, 1 }),
			new TileTypes("CornerBL",		37, false, new int[]{ 2, 1, 2, 1, 2, 0, 1, 0 }),
			new TileTypes("CornerTR",		38, false, new int[]{ 2, 0, 1, 0, 2, 1, 2, 1 }),
			new TileTypes("CornerTL",		39, false, new int[]{ 1, 0, 2, 1, 2, 1, 2, 0 }),
			new TileTypes("CBLR",    		40, false, new int[]{ 0, 0, 0, 0, 1, 0, 1, 0 }),
			new TileTypes("CLTB",    		41, false, new int[]{ 1, 0, 0, 0, 0, 0, 1, 0 }),
			new TileTypes("CTLR",    		42, false, new int[]{ 1, 0, 1, 0, 0, 0, 0, 0 }),
			new TileTypes("CRTB",    		43, false, new int[]{ 0, 0, 1, 0, 1, 0, 0, 0 }),
            new TileTypes("Background1",	44, false, new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 }),
			new TileTypes("Background2",	45, false, new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 }),
			new TileTypes("Ground1",		46, true,  new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 }),
			new TileTypes("Ground2",		47, true,  new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 })
            //when ground and background is re ordered fix up loop boundaries in direct walls
        };

		map_data = new TileTypes[size_x, size_y];

		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_y; y++) {
                map_data[x, y] = randomBackground();
			}
		}

		rooms = new List<DRoom>();
		unconnectedRooms = new List<DRoom>();
		edgeRooms = new List<DRoom>();

		{
			//int i = Random.Range(0, 4);
			//int j = (Random.Range(1, 4) + i) % 4;
			//MakeEdgeRoom(0);
			MakeEdgeRoom(2);
		}

		while (unconnectedRooms.Count < numRooms) {
			int rsx = Random.Range(rxmin, rxmax + 1);
			int rsy = Random.Range(rymin, rymax + 1);

			DRoom r = new DRoom();
			r.left = Random.Range(0, size_x - rsx);
			r.top = Random.Range(0, size_y - rsy);
			r.width = rsx;
			r.height = rsy;

			if (!RoomCollides(r)) {
				unconnectedRooms.Add(r);
			}
			else {
				maxFails--;
				if (maxFails <= 0)
					break;
			}

		}

		foreach (DRoom r2 in unconnectedRooms) {
			MakeRoom(r2);
		}

		Debug.Log("Done Rooms!");

		MoveConnectedRoom();
		while (unconnectedRooms.Count > 0) {
			MakeCorridor(unconnectedRooms[0], ClosestRoom(unconnectedRooms[0]));
			MoveConnectedRoom();
		}

		foreach (DRoom r2 in edgeRooms) {
			MakeCorridor(r2, ClosestRoom(r2));
		}

		MakeWalls();

		Debug.Log("Done Corridor!");

		DirectWalls();

		PlaceStairs(numRooms);
	}


	/******************************************************************/
	public TileTypes GetTileAt(int x, int y) {
		return map_data[x, y];
	}

	bool RoomCollides(DRoom r) {
		foreach (DRoom r2 in unconnectedRooms) {
			if (r.CollidesWith(r2)) {
				return true;
			}
		}
		foreach (DRoom r3 in edgeRooms) {
			if (r.CollidesWith(r3)) {
				return true;
			}
		}

		return false;
	}

	void MakeRoom(DRoom r) {

		for (int x = 0; x < r.width; x++) {
			for (int y = 0; y < r.height; y++) {
				if (x == 0) {
					map_data[r.left + x, r.top + y] = lookupByName("Wall");
				}
				else if (x == r.width - 1) {
					map_data[r.left + x, r.top + y] = lookupByName("Wall");
				}
				else if (y == 0) {
					map_data[r.left + x, r.top + y] = lookupByName("Wall");
				}
				else if (y == r.height - 1) {
					map_data[r.left + x, r.top + y] = lookupByName("Wall");
				}
				else {
                    map_data[r.left + x, r.top + y] = randomGround();
				}
			}
		}

	}

	void MakeCorridor(DRoom r1, DRoom r2) {
		int x = Random.Range(r1.left+1, r1.right);
		int y = Random.Range(r1.top+1, r1.bottom);
		//int x = r1.center_x;
		//int y = r1.center_y;

		int x2 = Random.Range(r2.left+1, r2.right);
		int y2 = Random.Range(r2.top+1, r2.bottom);
		while (y != y2) {
            map_data[x, y] = randomGround();

			y += y < y2 ? 1 : -1;
		}

		while (x != x2) {
			map_data[x, y] = randomGround();

			x += x < x2 ? 1 : -1;
		}

		r1.isConnected = true;
		r2.isConnected = true;

	}

	void MakeWalls() {
		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_y; y++) {
				if (map_data[x, y].getWalkable() == false && AdjacentFloors(x, y)) {
					map_data[x, y] = lookupByName("Wall");
				}
			}
		}
	}

	bool AdjacentFloors(int x, int y) {
		if (x > 0 && map_data[x - 1, y].getWalkable() == true)
			return true;
		if (x < size_x - 1 && map_data[x + 1, y].getWalkable() == true)
			return true;
		if (y > 0 && map_data[x, y - 1].getWalkable() == true)
			return true;
		if (y < size_y - 1 && map_data[x, y + 1].getWalkable() == true)
			return true;
		if (x > 0 && y > 0 && map_data[x - 1, y - 1].getWalkable() == true)
			return true;
		if (x < size_x - 1 && y > 0 && map_data[x + 1, y - 1].getWalkable() == true)
			return true;
		if (x > 0 && y < size_y - 1 && map_data[x - 1, y + 1].getWalkable() == true)
			return true;
		if (x < size_x - 1 && y < size_y - 1 && map_data[x + 1, y + 1].getWalkable() == true)
			return true;

		return false;
	}

	void MoveConnectedRoom() {
		rooms.Add(unconnectedRooms[0]);
		unconnectedRooms.RemoveAt(0);
	}

	void MakeEdgeRoom(int i) {
		DRoom r = new DRoom();
		if (i == 0) {
			r.width = 5;
			r.height = 4;
			r.left = (int)Mathf.Ceil((float)(size_x - 1) / 2) - 2;
			r.top = size_y - 4;
		}
		else if (i == 1) {
			r.width = 4;
			r.height = 5;
			r.left = size_x - 4;
			r.top = (int)Mathf.Ceil((float)(size_y - 1) / 2) - 2;
		}
		else if (i == 2) {
			r.width = 5;
			r.height = 4;
			r.left = (int)Mathf.Ceil((float)(size_x - 1) / 2) - 2;
			r.top = 0;
		}
		else if (i == 3) {
			r.width = 4;
			r.height = 5;
			r.left = 0;
			r.top = (int)Mathf.Ceil((float)(size_y - 1) / 2) - 2;
		}
		else {
			Debug.Log("Invalid EdgeRoom number");
		}

		MakeRoom(r);
		edgeRooms.Add(r);
	}

	DRoom ClosestRoom(DRoom a) {
		DRoom minRoom = rooms[0];
		float minDistance = 0;
		foreach (DRoom r in rooms) {
			float distance = Mathf.Sqrt((Mathf.Abs(r.center_x - a.center_x)) ^ 2 + (Mathf.Abs(r.center_y - a.center_y)) ^ 2);
			if (minDistance == 0) {
				minDistance = distance;
				minRoom = r;
			}
			else if (distance < minDistance) {
				minDistance = distance;
				minRoom = r;
			}
		}
		return minRoom;
	}

	void PlaceStairs(int tries) {
		int i = 0;
		DRoom StairsRoom = rooms[i];
		int maxTries = tries;
		while (StairsRoom.right - StairsRoom.left < 4 || StairsRoom.bottom - StairsRoom.top < 4) {
			StairsRoom = rooms[i++];
			maxTries--;
			if (maxTries == 0) {
				Debug.Log("No Room suitable for stairs");
				return;
			}
		}
		int xStairs = Random.Range(2, StairsRoom.right - StairsRoom.left - 1) + StairsRoom.left;
		int yStairs = Random.Range(2, StairsRoom.bottom - StairsRoom.top - 1) + StairsRoom.top;
		map_data[xStairs, yStairs] = lookupByName("Stairs");
		Debug.Log("Done Stairs!");
	}

	int[] AdjFloor(int x, int y) {
		int[] floorArray = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
		if (x != 0 && y != size_y - 1)
			floorArray[0] = (map_data[x - 1, y + 1].getWalkable() == true) ? 1 : 0;
		if (y != size_y - 1)
			floorArray[1] = (map_data[x, y + 1].getWalkable() == true) ? 1 : 0;
		if (x != size_x - 1 && y != size_y - 1)
			floorArray[2] = (map_data[x + 1, y + 1].getWalkable() == true) ? 1 : 0;
		if (x != size_x - 1)
			floorArray[3] = (map_data[x + 1, y].getWalkable() == true) ? 1 : 0;
		if (x != size_x - 1 && y != 0)
			floorArray[4] = (map_data[x + 1, y - 1].getWalkable() == true) ? 1 : 0;
		if (y != 0)
			floorArray[5] = (map_data[x, y - 1].getWalkable() == true) ? 1 : 0;
		if (x != 0 && y != 0)
			floorArray[6] = (map_data[x - 1, y - 1].getWalkable() == true) ? 1 : 0;
		if (x != 0)
			floorArray[7] = (map_data[x - 1, y].getWalkable() == true) ? 1 : 0;

		return floorArray;
	}

	void DirectWalls() {
		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_y; y++) {
				if (map_data[x, y].getWalkable() == false) {
					for(int i = 4; i < tiles.Length - 4; i++) {
						if (CompareArrayWith2D(AdjFloor(x, y), tiles[i].getSurrMatrix())) {
							map_data[x, y] = tiles[i];
						}
					}
				}
			}
		}
	}

	bool CompareArrayWith2D(int[] a, int[] b) {
		bool equal = true;
		for (int i = 0; i < 8; i++) {
			if (a[i] != b[i] && b[i] != 2) {
				equal = false;
			}
		}
		if (equal == true) {
			return true;
		}
		return false;
	}

	public TileTypes lookupByName(string name) {
		for (int i = 0; i < tiles.Length; i++) {
			if (name == tiles[i].getName()) {
				return tiles[i];
			}
		}
		return null;
	}

	public TileTypes[,] getMapData() {
		return map_data;
	}

    TileTypes randomBackground() {
        int rand = Random.Range(0, 100);
        if (rand < 90)
        {
            return lookupByName("Background");
        }
        else if (rand > 95)
        {
            return lookupByName("Background1");
        }
        else
        {
            return lookupByName("Background2");
        }
    }

    TileTypes randomGround()
    {
        int rand = Random.Range(0, 100);
        if (rand < 92)
        {
            return lookupByName("Ground");
        }
        else if (rand > 96)
        {
            return lookupByName("Ground1");
        }
        else
        {
            return lookupByName("Ground2");
        }
    }

}
