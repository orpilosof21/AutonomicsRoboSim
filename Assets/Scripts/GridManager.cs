using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private int rows;
    private int cols;
    [SerializeField]
    private float tile_size = 1.1f;
    private Tile[,] gridMap;
    private GameObject[,] tileObjects;


    [SerializeField]
    public GameObject unvisitedPrefab;
    [SerializeField]
    public GameObject visitedPrefab;
    [SerializeField]
    public GameObject obsteclePrefab;
    [SerializeField]
    public GameObject finishPrefab;

    // Start is called before the first frame update
    void Start()
    {
        ParseTheMap();
        GenerateGrid();
        VisitTheStart();
        MarkTheFinish();
    }

    private void MarkTheFinish()
    {
        GameObject temp = tileObjects[StaticVars.finishRow, StaticVars.finishCol];
        Destroy(tileObjects[StaticVars.finishRow, StaticVars.finishCol]);

        GameObject curTile = (GameObject)Instantiate(finishPrefab, transform);

        curTile.transform.position = new Vector2(temp.transform.position.x, temp.transform.position.y);

        tileObjects[StaticVars.finishRow, StaticVars.finishCol] = curTile;
    }

    private void VisitTheStart()
    {
        GameObject temp = tileObjects[StaticVars.startRow, StaticVars.startCol];
        Destroy(tileObjects[StaticVars.startRow, StaticVars.startCol]);

        GameObject curTile = (GameObject)Instantiate(visitedPrefab, transform);

        curTile.transform.position = new Vector2(temp.transform.position.x,temp.transform.position.y);

        tileObjects[StaticVars.startRow, StaticVars.startCol] = curTile;
        StaticVars.startTile = curTile;

    }

    private void ParseTheMap()
    {
        string data = StaticVars.grid;
        string[] dataLines = data.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        rows = dataLines.Length;
        cols = dataLines[0].Length;
        gridMap = new Tile[rows, cols];
        tileObjects = new GameObject[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (i==StaticVars.startRow && j == StaticVars.startCol)
                {
                    gridMap[i, j] = new Tile(i, j, TileType.visited);
                    continue;
                }
                if (dataLines[i][j] == 'X')
                {
                    gridMap[i, j] = new Tile(i, j, TileType.obstacle);
                    continue;
                }
                gridMap[i, j] = new Tile(i, j, TileType.unvisited);
            }
        }
    }

    private void GenerateGrid()
    {
        GameObject referenceTile = (GameObject)Instantiate(unvisitedPrefab);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                GameObject curTile = (GameObject)Instantiate(referenceTile, transform);
                //float posX = i * tile_size;
                //float posY = j * -tile_size;

                float posY = i * -tile_size;
                float posX = j * tile_size;

                curTile.transform.position = new Vector2(posX, posY);

                tileObjects[i, j] = curTile;
            }
        }
        Destroy(referenceTile);

        float gridW = cols * tile_size;
        float gridH = rows * tile_size;
        transform.position = new Vector2(-gridW / 2 + tile_size / 2, gridH / 2 - tile_size / 2);
    }

    public TileType GetTileType(int row, int col)
    {
        try
        {
            return gridMap[row, col].type;
        }
        catch
        {
            return TileType.out_of_bounds;
        }
    }

    public Vector3 GetTilePosition(int row,int col)
    {
        return tileObjects[row, col].transform.position;
    }

    public void MarkObstecle(int row, int col)
    {
        GameObject temp = tileObjects[row, col];
        Destroy(tileObjects[row, col]);

        GameObject curTile = (GameObject)Instantiate(obsteclePrefab, transform);

        curTile.transform.position = new Vector2(temp.transform.position.x, temp.transform.position.y);

        tileObjects[row, col] = curTile;
        gridMap[row, col].type = TileType.obstacle;
    }

    public void MarkVisited(int row, int col)
    {
        if (gridMap[row, col].type != TileType.visited)
        {
            GameObject temp = tileObjects[row, col];
            Destroy(tileObjects[row, col]);

            GameObject curTile = (GameObject)Instantiate(visitedPrefab, transform);

            curTile.transform.position = new Vector2(temp.transform.position.x, temp.transform.position.y);

            tileObjects[row, col] = curTile;
        }
    }

}


