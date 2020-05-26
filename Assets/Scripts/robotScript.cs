using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robotScript : MonoBehaviour
{
    enum Direction
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    GridManager gridManager;

    Dictionary<Direction, int> RotationDict;

    int curRow;
    int curCol;
    Direction curDir = Direction.UP;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject gridHolder = GameObject.Find("GridHolder");
        //RotationDict = new Dictionary<Direction, int>();
        //RotationDict.Add(Direction.UP, 0);
        //RotationDict.Add(Direction.RIGHT, 270);
        //RotationDict.Add(Direction.DOWN, 180);
        //RotationDict.Add(Direction.LEFT, 90);
        /*curRow = StaticVars.startRow;
        curCol = StaticVars.startCol;
        curDir = Direction.UP;
        RotationDict.Add(Direction.UP, 0);
        RotationDict.Add(Direction.RIGHT, 270);
        RotationDict.Add(Direction.DOWN, 180);
        RotationDict.Add(Direction.LEFT, 90);

        this.transform.rotation = Quaternion.Euler(0,0,RotationDict[Direction.UP]);
        this.transform.position = StaticVars.startTile.transform.position;*/
    }

    private void Awake()
    {
        RotationDict = new Dictionary<Direction, int>();
        RotationDict.Add(Direction.UP, 0);
        RotationDict.Add(Direction.RIGHT, 270);
        RotationDict.Add(Direction.DOWN, 180);
        RotationDict.Add(Direction.LEFT, 90);
        GameObject gridHolder = GameObject.Find("GridHolder");
        gridManager = gridHolder.GetComponent<GridManager>();
        curRow = StaticVars.startRow;
        curCol = StaticVars.startCol;
        curDir = Direction.UP;
        
        //this.transform.rotation = Quaternion.Euler(0, 0, RotationDict[Direction.UP]);
        this.transform.position = StaticVars.startTile.transform.position;
    }

    public int Move(int direction)
    {
        switch (direction) {
            case (int)Direction.UP:
                return MoveAux(-1, 0, Direction.UP);
            case (int)Direction.RIGHT:
                return MoveAux(0, 1, Direction.RIGHT);
            case (int)Direction.DOWN:
                return MoveAux(1, 0, Direction.DOWN);
            case (int)Direction.LEFT:
                return MoveAux(0, -1, Direction.LEFT);
            default:
                return -3;
        }
    }

    private int MoveAux(int x,int y,Direction newDirection)
    {
        if (curDir != newDirection)
        {
            rotateToDirection(newDirection);
        }
        var cur_tile = gridManager.GetTileType(curRow + x, curCol + y);
        if (cur_tile == TileType.obstacle)
        {
            gridManager.MarkObstecle(curRow + x, curCol + y);
            return -1;
        }
        if (cur_tile == TileType.out_of_bounds)
        {
            return -2;
        }
        curRow += x;
        curCol += y;
        return MoveToNewPosition(curRow, curCol);
    }

    private int MoveToNewPosition(int curRow, int curCol)
    {
        //yield return new WaitForSeconds(1);
        this.transform.position = gridManager.GetTilePosition(curRow, curCol);
        if (curRow==StaticVars.finishRow && curCol == StaticVars.finishCol)
        {
            return 1;
        }
        gridManager.MarkVisited(curRow, curCol);
        return 0;
        //yield return new WaitForSeconds(1);

    }

    private void rotateToDirection(Direction newDirection)
    {
        curDir = newDirection;
        this.transform.rotation = Quaternion.Euler(0, 0, RotationDict[curDir]);
    }
}
