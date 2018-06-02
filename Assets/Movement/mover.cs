using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class mover : MonoBehaviour {

	/* class Movement{
		public int Priority { get; set; }
		public int Direction { get; set; }
		//0  1    2    3
		//Up Down Left Right

		public Movement(int _direction, int _priority) {
			Priority = _priority;
			Direction = _direction;
		}

		public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Movement objAsMov = obj as Movement;
            if (objAsMov == null) return false;
            else return Equals(objAsMov);
        }

		public override int GetHashCode()
        {
            return Direction;
        }

		public bool Equals(Movement mov) {
			if(mov == null) return false;
			return (this.Direction.Equals(mov.Direction));
		}
	};

	struct MovementStack {
		public List<Movement> stack;

		public void addMovement(Movement x) {
			if(stack.Contains(x)) {
				stack.Remove(x);
			}
			int i = 0;
			while (stack[i].Priority > x.Priority) {
				i++;
			}
			stack.Insert(i, x);
		}

		public void removeMovement(Movement x) {
			if(stack.Contains(x)) {
				stack.Remove(x);
			}
		}

		public bool hasDirection(int index, int dir) {
			return (stack[index].Direction == dir);
		}

		public int count() {
			return this.stack.Count;
		}
	} */

	public TileMap TMap;
    DTileMapNew1 DMap;
	public float speed = 4.0f;
	Vector3 pos;
	Transform tr;
	int xPos;
	int yPos;
	//MovementStack stack;

	//Prioritize most reecently pressed invalid movement when valid
	//If key down, If valid, place at top of stack with priority 2
	//If key down, If invalid, place at top of stack with priority 1
	//If key up, remove from stack


	void Start() {
        DMap = TMap.map;
		pos = transform.position;
        tr = transform;
		xPos = (int)Mathf.Floor(pos.x);
		yPos = (int)Mathf.Floor(pos.z)+60;
        Debug.Log(xPos + " " + yPos);
		//stack = new MovementStack();
	}

	void Update() {
		
		if(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift)) {
			speed = 10.0f;
		}
		else {
			speed = 5.0f;
		}
		/* if(Input.GetKeyUp(KeyCode.RightArrow)) {
			stack.removeMovement(new Movement(3, 0));
		}
		if(Input.GetKeyUp(KeyCode.LeftArrow)) {
			stack.removeMovement(new Movement(2, 0));
		}
		if(Input.GetKeyUp(KeyCode.UpArrow)) {
			stack.removeMovement(new Movement(0, 0));
		}
		if(Input.GetKeyUp(KeyCode.DownArrow)) {
			stack.removeMovement(new Movement(1, 0));
		}
		if(Input.GetKey(KeyCode.RightArrow)) {
			if(DMap.getMapData()[xPos+1,yPos].getWalkable())
				stack.addMovement(new Movement(3, 2));
			else
				stack.addMovement(new Movement(3, 1));
		}
		if(Input.GetKey(KeyCode.LeftArrow)) {
			if(DMap.getMapData()[xPos-1,yPos].getWalkable())
				stack.addMovement(new Movement(2, 2));
			else
				stack.addMovement(new Movement(2, 1));
		}
		if(Input.GetKey(KeyCode.UpArrow)) {
			if(DMap.getMapData()[xPos,yPos+1].getWalkable())
				stack.addMovement(new Movement(0, 2));
			else
				stack.addMovement(new Movement(0, 1));
		}
		if(Input.GetKey(KeyCode.DownArrow)) {
			if(DMap.getMapData()[xPos,yPos-1].getWalkable())
				stack.addMovement(new Movement(1, 2));
			else
				stack.addMovement(new Movement(1, 1));
		} */
		if (tr.position == pos) {
			// for(int i = 0; i < stack.stack.Count; i++) {
				if (Input.GetKey(KeyCode.RightArrow) && DMap.getMapData()[xPos+1,yPos].getWalkable()) {
					pos += Vector3.right;
					xPos++;
				}
				else if (Input.GetKey(KeyCode.LeftArrow) && DMap.getMapData()[xPos-1, yPos].getWalkable()) {
					pos += Vector3.left;
					xPos--;
				}
				else if (Input.GetKey(KeyCode.UpArrow) && DMap.getMapData()[xPos, yPos+1].getWalkable()) {
					pos += Vector3.forward;
					yPos++;
				}
				else if (Input.GetKey(KeyCode.DownArrow) && DMap.getMapData()[xPos, yPos-1].getWalkable()) {
					pos += Vector3.back;
					yPos--;
				}
			// }
		}

		transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);
	}
    void checkRoom()
    {

    }
}
