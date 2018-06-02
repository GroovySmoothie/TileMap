using UnityEngine;
using System.Collections.Generic;

public class DTileMapNEW
{

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
	int[,] map_data;

	List<DRoom> rooms;
	List<DRoom> unconnectedRooms;
	List<DRoom> edgeRooms;

	Dictionary<string, int[]> nameToArray;
	Dictionary<string, int> nameToInt;

	public DTileMapNEW(int size_x, int size_y, int rxmin, int rxmax, int rymin, int rymax, int numRooms, int maxFails) {
		DRoom r;
		this.size_x = size_x;
		this.size_y = size_y;

		nameToArray = new Dictionary<string, int[]>();
		nameToInt = new Dictionary<string, int>();
		{   //0 = wall/background, 1 = ground, 2 = dont care
			nameToArray.Add("LeftWall", new int[] { 0, 0, 2, 1, 2, 0, 0, 0 });
			nameToArray.Add("RightWall", new int[] { 2, 0, 0, 0, 0, 0, 2, 1 });
			nameToArray.Add("TopWall", new int[] { 0, 0, 0, 0, 2, 1, 2, 0 });
			nameToArray.Add("BottomWall", new int[] { 2, 1, 2, 0, 0, 0, 0, 0 });
			nameToArray.Add("TopLeft", new int[] { 0, 0, 0, 0, 1, 0, 0, 0 });
			nameToArray.Add("TopRight", new int[] { 0, 0, 0, 0, 0, 0, 1, 0 });
			nameToArray.Add("BottomLeft", new int[] { 0, 0, 1, 0, 0, 0, 0, 0 });
			nameToArray.Add("BottomRight", new int[] { 1, 0, 0, 0, 0, 0, 0, 0 });
			nameToArray.Add("BotAndRight", new int[] { 0, 0, 2, 1, 2, 1, 2, 0 });
			nameToArray.Add("BotAndLeft", new int[] { 2, 0, 0, 0, 2, 1, 2, 1 });
			nameToArray.Add("TopAndLeft", new int[] { 2, 1, 2, 0, 0, 0, 2, 1 });
			nameToArray.Add("TopAndRight", new int[] { 2, 1, 2, 1, 2, 0, 0, 0 });

			nameToInt.Add("LeftWall", 2);
			nameToInt.Add("RightWall", 3);
			nameToInt.Add("TopWall", 4);
			nameToInt.Add("BottomWall", 5);
			nameToInt.Add("TopLeft", 6);
			nameToInt.Add("TopRight", 7);
			nameToInt.Add("BottomLeft", 8);
			nameToInt.Add("BottomRight", 9);
			nameToInt.Add("BotAndRight", 12);
			nameToInt.Add("BotAndLeft", 13);
			nameToInt.Add("TopAndLeft", 14);
			nameToInt.Add("TopAndRight", 15);
		}


		map_data = new int[size_x, size_y];

		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_y; y++) {
				map_data[x, y] = 10;
			}
		}

		rooms = new List<DRoom>();
		unconnectedRooms = new List<DRoom>();
		edgeRooms = new List<DRoom>();

		{
			int i = Random.Range(0, 4);
			int j = (Random.Range(0, 3) + i + 1) % 4;
			MakeEdgeRoom(i);
			MakeEdgeRoom(j);
		}

		while (unconnectedRooms.Count < numRooms) {
			int rsx = Random.Range(rxmin, rxmax + 1);
			int rsy = Random.Range(rymin, rymax + 1);

			r = new DRoom();
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

		PlaceStairs(numRooms);

		DirectWalls();
	}



	/******************************************************************/
	public int GetTileAt(int x, int y) {
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
					map_data[r.left + x, r.top + y] = 11;
				}
				else if (x == r.width - 1) {
					map_data[r.left + x, r.top + y] = 11;
				}
				else if (y == 0) {
					map_data[r.left + x, r.top + y] = 11;
				}
				else if (y == r.height - 1) {
					map_data[r.left + x, r.top + y] = 11;
				}
				else {
					map_data[r.left + x, r.top + y] = 1;
				}
			}
		}

	}

	void MakeCorridor(DRoom r1, DRoom r2) {
		int x = r1.center_x;
		int y = r1.center_y;

		while (y != r2.center_y) {
			map_data[x, y] = 1;

			y += y < r2.center_y ? 1 : -1;
		}

		while (x != r2.center_x) {
			map_data[x, y] = 1;

			x += x < r2.center_x ? 1 : -1;
		}

		r1.isConnected = true;
		r2.isConnected = true;

	}

	void MakeWalls() {
		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_y; y++) {
				if (map_data[x, y] == 10 && AdjacentFloors(x, y)) {
					map_data[x, y] = 11;
				}
			}
		}
	}

	bool AdjacentFloors(int x, int y) {
		if (x > 0 && map_data[x - 1, y] == 1)
			return true;
		if (x < size_x - 1 && map_data[x + 1, y] == 1)
			return true;
		if (y > 0 && map_data[x, y - 1] == 1)
			return true;
		if (y < size_y - 1 && map_data[x, y + 1] == 1)
			return true;
		if (x > 0 && y > 0 && map_data[x - 1, y - 1] == 1)
			return true;
		if (x < size_x - 1 && y > 0 && map_data[x + 1, y - 1] == 1)
			return true;
		if (x > 0 && y < size_y - 1 && map_data[x - 1, y + 1] == 1)
			return true;
		if (x < size_x - 1 && y < size_y - 1 && map_data[x + 1, y + 1] == 1)
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
		map_data[xStairs, yStairs] = 0;
		Debug.Log("Done Stairs!");
	}

	int[] AdjFloor(int x, int y) {
		int[] floorArray = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
		if (x != 0 && y != size_y - 1)
			floorArray[0] = (map_data[x - 1, y + 1] == 1) ? 1 : 0;
		if (y != size_y - 1)
			floorArray[1] = (map_data[x, y + 1] == 1) ? 1 : 0;
		if (x != size_x - 1 && y != size_y - 1)
			floorArray[2] = (map_data[x + 1, y + 1] == 1) ? 1 : 0;
		if (x != size_x - 1)
			floorArray[3] = (map_data[x + 1, y] == 1) ? 1 : 0;
		if (x != size_x - 1 && y != 0)
			floorArray[4] = (map_data[x + 1, y - 1] == 1) ? 1 : 0;
		if (y != 0)
			floorArray[5] = (map_data[x, y - 1] == 1) ? 1 : 0;
		if (x != 0 && y != 0)
			floorArray[6] = (map_data[x - 1, y - 1] == 1) ? 1 : 0;
		if (x != 0)
			floorArray[7] = (map_data[x - 1, y] == 1) ? 1 : 0;

		return floorArray;
	}

	void DirectWalls() {
		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_y; y++) {
				if (map_data[x, y] == 11) {
					if (CompareArrayWith2D(AdjFloor(x, y), nameToArray["LeftWall"]))
						map_data[x, y] = nameToInt["LeftWall"];
					else if (CompareArrayWith2D(AdjFloor(x, y), nameToArray["RightWall"]))
						map_data[x, y] = nameToInt["RightWall"];
					else if (CompareArrayWith2D(AdjFloor(x, y), nameToArray["TopWall"]))
						map_data[x, y] = nameToInt["TopWall"];
					else if (CompareArrayWith2D(AdjFloor(x, y), nameToArray["BottomWall"]))
						map_data[x, y] = nameToInt["BottomWall"];
					else if (CompareArrayWith2D(AdjFloor(x, y), nameToArray["TopLeft"]))
						map_data[x, y] = nameToInt["TopLeft"];
					else if (CompareArrayWith2D(AdjFloor(x, y), nameToArray["TopRight"]))
						map_data[x, y] = nameToInt["TopRight"];
					else if (CompareArrayWith2D(AdjFloor(x, y), nameToArray["BottomLeft"]))
						map_data[x, y] = nameToInt["BottomLeft"];
					else if (CompareArrayWith2D(AdjFloor(x, y), nameToArray["BottomRight"]))
						map_data[x, y] = nameToInt["BottomRight"];
					else if (CompareArrayWith2D(AdjFloor(x, y), nameToArray["BotAndRight"]))
						map_data[x, y] = nameToInt["BotAndRight"];
					else if (CompareArrayWith2D(AdjFloor(x, y), nameToArray["BotAndLeft"]))
						map_data[x, y] = nameToInt["BotAndLeft"];
					else if (CompareArrayWith2D(AdjFloor(x, y), nameToArray["TopAndLeft"]))
						map_data[x, y] = nameToInt["TopAndLeft"];
					else if (CompareArrayWith2D(AdjFloor(x, y), nameToArray["TopAndRight"]))
						map_data[x, y] = nameToInt["TopAndRight"];
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
}