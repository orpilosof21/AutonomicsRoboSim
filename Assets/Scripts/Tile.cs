using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    unvisited,
    visited,
    obstacle,
    out_of_bounds
}


public class Tile
{
    public int row;
    public int col;
    public TileType type;

    public Tile(int row, int col, TileType type)
    {
        this.row = row;
        this.col = col;
        this.type = type;
    }

    public Tile(int row,int col)
    {
        this.row = row;
        this.col = col;
        this.type = TileType.unvisited;
    }

}
