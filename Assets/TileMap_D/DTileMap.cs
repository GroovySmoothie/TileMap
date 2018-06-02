using UnityEngine;
using System.Collections.Generic;

public class DTileMap {
	
	protected class DRoom {
		public int left;
		public int top;
		public int width;
		public int height;
		
		public bool isConnected=false;
		
		public int right {
			get {return left + width - 1;}
		}
		
		public int bottom {
			get { return top + height - 1; }
		}
		
		public int center_x {
			get { return left + width/2; }
		}
		
		public int center_y {
			get { return top + height/2; }
		}
		
		public bool CollidesWith(DRoom other) {
			if( left > other.right-1 )
				return false;
			
			if( top > other.bottom-1 )
				return false;
			
			if( right < other.left+1 )
				return false;
			
			if( bottom < other.top+1 )
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

    public DTileMap(int size_x, int size_y, int rxmin, int rxmax, int rymin, int rymax, int numRooms, int maxFails) {
		DRoom r;
		this.size_x = size_x;
		this.size_y = size_y;

        nameToArray = new Dictionary<string, int[]>();
        nameToInt = new Dictionary<string, int>();

		//Dictionary Definitions
		{   //0 = wall/background, 1 = ground, 2 = dont care
            nameToArray.Add("LeftWall",     new int[]{0,0,2,1,2,0,0,0});
            nameToArray.Add("RightWall",    new int[]{2,0,0,0,0,0,2,1});
            nameToArray.Add("TopWall",      new int[]{0,0,0,0,2,1,2,0});
            nameToArray.Add("BottomWall",   new int[]{2,1,2,0,0,0,0,0});
            nameToArray.Add("TopLeft",      new int[]{0,0,0,0,1,0,0,0});
            nameToArray.Add("TopRight",     new int[]{0,0,0,0,0,0,1,0});
            nameToArray.Add("BottomLeft",   new int[]{0,0,1,0,0,0,0,0});
            nameToArray.Add("BottomRight",  new int[]{1,0,0,0,0,0,0,0});
            nameToArray.Add("BotAndRight",  new int[]{0,0,2,1,2,1,2,0});
            nameToArray.Add("BotAndLeft",   new int[]{2,0,0,0,2,1,2,1});
            nameToArray.Add("TopAndLeft",   new int[]{2,1,2,0,0,0,2,1});
            nameToArray.Add("TopAndRight",  new int[]{2,1,2,1,2,0,0,0});
            nameToArray.Add("TBR",			new int[]{2,1,2,1,2,1,2,0});
            nameToArray.Add("TBL",			new int[]{2,1,2,0,2,1,2,1});
            nameToArray.Add("BLR",			new int[]{2,0,2,1,2,1,2,1});
            nameToArray.Add("TLR",			new int[]{2,1,2,1,2,0,2,1});
            nameToArray.Add("TopBottom",	new int[]{2,1,2,0,2,1,2,0});
            nameToArray.Add("LeftRight",	new int[]{2,0,2,1,2,0,2,1});
            nameToArray.Add("All",			new int[]{2,1,2,1,2,1,2,1});
            nameToArray.Add("Corners",		new int[]{1,0,1,0,1,0,1,0});
            nameToArray.Add("RightT",		new int[]{1,0,2,1,2,0,0,0});
            nameToArray.Add("RightB",		new int[]{0,0,2,1,2,0,1,0});
            nameToArray.Add("RightTB",		new int[]{1,0,2,1,2,0,1,0});
            nameToArray.Add("LeftT",		new int[]{2,0,1,0,0,0,2,1});
            nameToArray.Add("LeftB",		new int[]{2,0,0,0,1,0,2,1});
            nameToArray.Add("LeftTB",		new int[]{2,0,1,0,1,0,2,1});
            nameToArray.Add("BottomL",		new int[]{1,0,0,0,2,1,2,0});
            nameToArray.Add("BottomR",		new int[]{0,0,1,0,2,1,2,0});
            nameToArray.Add("BottomLR",		new int[]{1,0,1,0,2,1,2,0});
            nameToArray.Add("TopL",			new int[]{2,1,2,0,0,0,1,0});
            nameToArray.Add("TopR",			new int[]{2,1,2,0,1,0,0,0});
            nameToArray.Add("TopLR",		new int[]{2,1,2,0,1,0,1,0});
            nameToArray.Add("CornerBR",		new int[]{2,1,2,0,1,0,2,1});
            nameToArray.Add("CornerBL",		new int[]{2,1,2,1,2,0,1,0});
            nameToArray.Add("CornerTR",		new int[]{2,0,1,0,2,1,2,1});
            nameToArray.Add("CornerTL",		new int[]{1,0,2,1,2,1,2,0});

			nameToInt.Add("Unknown",		0);
			nameToInt.Add("Ground",			1);
            nameToInt.Add("Background",		2);
			nameToInt.Add("Wall",			3);
			nameToInt.Add("LeftWall",		4);
            nameToInt.Add("RightWall",		5);
            nameToInt.Add("TopWall",		6);
            nameToInt.Add("BottomWall",		7);
            nameToInt.Add("TopLeft",		8);
            nameToInt.Add("TopRight",		9);
            nameToInt.Add("BottomLeft",		10);
            nameToInt.Add("BottomRight",	11);
			nameToInt.Add("BotAndRight",	12);
            nameToInt.Add("BotAndLeft",		13);
            nameToInt.Add("TopAndLeft",		14);
            nameToInt.Add("TopAndRight",	15);
            nameToInt.Add("TBR",			16);
            nameToInt.Add("TBL",			17);
            nameToInt.Add("BLR",			18);
            nameToInt.Add("TLR",			19);
            nameToInt.Add("TopBottom",		20);
            nameToInt.Add("LeftRight",		21);
            nameToInt.Add("All",			22);
            nameToInt.Add("Corners",		23);
            nameToInt.Add("RightT",			24);
            nameToInt.Add("RightB",			25);
            nameToInt.Add("RightTB",		26);
            nameToInt.Add("LeftT",			27);
            nameToInt.Add("LeftB",			28);
            nameToInt.Add("LeftTB",			29);
            nameToInt.Add("BottomL",		30);
            nameToInt.Add("BottomR",		31);
            nameToInt.Add("BottomLR",		32);
            nameToInt.Add("TopL",			33);
            nameToInt.Add("TopR",			34);
            nameToInt.Add("TopLR",			35);
            nameToInt.Add("CornerBR",		36);
            nameToInt.Add("CornerBL",		37);
            nameToInt.Add("CornerTR",		38);
            nameToInt.Add("CornerTL",		39);
		}

        map_data = new int[size_x, size_y];

		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_y; y++) {
				map_data[x, y] = nameToInt["Background"];
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
		
		foreach(DRoom r2 in edgeRooms) {
			MakeCorridor(r2, ClosestRoom(r2));
		}

		MakeWalls();

		Debug.Log("Done Corridor!");

        DirectWalls();

		PlaceStairs(numRooms);
	}

	

	/******************************************************************/
	public int GetTileAt(int x, int y) {
		return map_data[x,y];
	}

	bool RoomCollides(DRoom r) {
		foreach(DRoom r2 in unconnectedRooms) {
			if(r.CollidesWith(r2)) {
				return true;
			}
		}
		foreach(DRoom r3 in edgeRooms) {
			if(r.CollidesWith(r3)) {
				return true;
			}
		}
		
		return false;
	}
	
	void MakeRoom(DRoom r) {
		
		for(int x=0; x < r.width; x++) {
			for(int y=0; y < r.height; y++){
				if(x==0) {
					map_data[r.left + x, r.top + y] = nameToInt["Wall"];
				}
				else if(x == r.width-1) {
					map_data[r.left + x, r.top + y] = nameToInt["Wall"];
				}
				else if(y == 0) {
					map_data[r.left + x, r.top + y] = nameToInt["Wall"];
				}
				else if(y == r.height-1) {
					map_data[r.left + x, r.top + y] = nameToInt["Wall"];
				}
				else {
					map_data[r.left+x,r.top+y] = nameToInt["Ground"];
				}
			}
		}
		
	}
	
	void MakeCorridor(DRoom r1, DRoom r2) {
		int x = r1.center_x;
		int y = r1.center_y;
		
		while( y != r2.center_y ) {
			map_data[x,y] = nameToInt["Ground"];
			
			y += y < r2.center_y ? 1 : -1;
		}

		while( x != r2.center_x) {
			map_data[x,y] = nameToInt["Ground"];
			
			x += x < r2.center_x ? 1 : -1;
		}

		r1.isConnected = true;
		r2.isConnected = true;
		
	}
	
	void MakeWalls() {
		for(int x=0; x< size_x;x++) {
			for(int y=0; y< size_y;y++) {
				if(map_data[x,y]==nameToInt["Background"] && AdjacentFloors(x,y)) {
					map_data[x,y]=nameToInt["Wall"];
				}
			}
		}
	}

	bool AdjacentFloors(int x, int y) {
		if( x > 0 && map_data[x-1,y] == nameToInt["Ground"] )
			return true;
		if( x < size_x-1 && map_data[x+1,y] == nameToInt["Ground"])
			return true;
		if( y > 0 && map_data[x,y-1] == nameToInt["Ground"])
			return true;
		if( y < size_y-1 && map_data[x,y+1] == nameToInt["Ground"])
			return true;
		if( x > 0 && y > 0 && map_data[x-1,y-1] == nameToInt["Ground"])
			return true;
		if( x < size_x-1 && y > 0 && map_data[x+1,y-1] == nameToInt["Ground"])
			return true;
		if( x > 0 && y < size_y-1 && map_data[x-1,y+1] == nameToInt["Ground"])
			return true;
		if( x < size_x-1 && y < size_y-1 && map_data[x+1,y+1] == nameToInt["Ground"])
			return true;

		return false;
	}
	
	void MoveConnectedRoom() {
		rooms.Add(unconnectedRooms[0]);
		unconnectedRooms.RemoveAt(0);
	}

	void MakeEdgeRoom(int i) {
		DRoom r = new DRoom();
		if(i == 0) {
			r.width = 5;
			r.height = 4;
			r.left = (int)Mathf.Ceil((float)(size_x - 1) / 2) - 2;
			r.top = size_y - 4;
		}
		else if(i == 1) {
			r.width = 4;
			r.height = 5;
			r.left = size_x - 4;
			r.top = (int)Mathf.Ceil((float)(size_y - 1) / 2) - 2;
		}
		else if(i == 2) {
			r.width = 5;
			r.height = 4;
			r.left = (int)Mathf.Ceil((float)(size_x - 1) / 2) - 2;
			r.top = 0;
		}
		else if(i == 3){
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
		foreach(DRoom r in rooms) {
			float distance = Mathf.Sqrt((Mathf.Abs(r.center_x - a.center_x)) ^ 2 + (Mathf.Abs(r.center_y - a.center_y)) ^ 2);
			if(minDistance == 0) {
				minDistance = distance;
				minRoom = r;
			}
			else if(distance < minDistance) {
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
		while(StairsRoom.right - StairsRoom.left < 4 || StairsRoom.bottom - StairsRoom.top < 4) {
			StairsRoom = rooms[i++];
			maxTries--;
			if(maxTries == 0) {
				Debug.Log("No Room suitable for stairs");
				return;
			}
		}
		int xStairs = Random.Range(2, StairsRoom.right - StairsRoom.left - 1) + StairsRoom.left;
		int yStairs = Random.Range(2, StairsRoom.bottom - StairsRoom.top - 1) + StairsRoom.top;
		map_data[xStairs, yStairs] = 0;
		Debug.Log("Done Stairs!");
	}

    int[] AdjFloor(int x, int y)
    {
        int[] floorArray = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        if(x != 0 && y != size_y -1)
            floorArray[0] = (map_data[x - 1, y + 1] == 1) ? 1 : 0;
        if (y != size_y-1)
            floorArray[1] = (map_data[x, y + 1] == 1) ? 1 : 0;
        if (x != size_x -1 && y != size_y -1)
            floorArray[2] = (map_data[x + 1, y + 1] == 1) ? 1 : 0;
        if (x != size_x -1) 
            floorArray[3] = (map_data[x+1, y] == 1) ? 1 : 0;
        if (x != size_x -1 && y != 0)
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
				if(map_data[x,y] == nameToInt["Wall"]) {
					foreach(KeyValuePair<string, int[]> entry in nameToArray) {
						if(CompareArrayWith2D(AdjFloor(x, y), entry.Value)) {
							map_data[x, y] = nameToInt[entry.Key];
						}
					}
                }
            }
		}
    }

    bool CompareArrayWith2D(int[] a, int[] b)
    {
		bool equal = true;
        for(int i = 0; i < 8; i++) {
			if(a[i] != b[i] && b[i] != 2) {
				equal = false;
			}
		}
		if(equal == true) {
			return true;
		}
        return false;
    }
}
