﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class MyIntEvent : UnityEvent<int>
{
}
public class Graph
{
    public MyIntEvent OnLocationChanged;
    private int columns;
    private int rows;
    private int tiles;
    private bool obstacleHandle;
    private LinkedList<int>[] adj; //Adjacency Lists
    private string[,] data;
    private Dictionary<int, KeyValuePair<int, int>> numberToCordMap;

    public int getRows()
    {
        return rows;
    }

    public int getColumns()
    {
        return columns;
    }

    public string[,] getData()
    {
        return data;
    }

    public Graph(int columns, int rows, string[,] data)
    {
        OnLocationChanged = new MyIntEvent();
        this.obstacleHandle = false;
        this.columns = columns;
        this.rows = rows;
        this.data = data;
        ClearData();
        this.tiles = rows * columns;
        this.adj = new LinkedList<int>[tiles];
        for (int i = 0; i < tiles; ++i)
        {
            this.adj[i] = new LinkedList<int>();
        }
        this.fillTheMap();
        this.CreateAdjacencies();
    }

    private void ClearData()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (this.data[i, j] == "X")
                {
                    this.data[i, j] = "O";
                }
            }
        }
    }

    void fillTheMap()
    {
        numberToCordMap = new Dictionary<int, KeyValuePair<int, int>>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                numberToCordMap.Add(i * columns + j, new KeyValuePair<int, int>(i, j));
            }
        }
    }

    void CreateAdjacencies()
    {
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < columns; j++)
            {
                if (data[i,j] != "X")
                {
                    CreateAdjacencies(i, j);
                }
            }
        }
    }

    void CreateAdjacencies(int row, int col)
    {
        addEdge(row, col, row + 1, col);
        addEdge(row, col, row - 1, col);
        addEdge(row, col, row, col + 1);
        addEdge(row, col, row, col - 1);
    }

    void RemoveAdjacencies(int row, int col)
    {
        foreach (var item in adj)
        {
            if (item.Contains(row * columns + col)){
                item.Remove(row * columns + col);
            }
        }
        adj[row * columns + col].Clear();
    }

    // Function to add an edge into the graph
    void addEdge(int vx, int vy, int wx, int wy)
    {
        if (wx < 0 || wx >= rows || wy < 0 || wy >= columns)
        {
            return;
        }
        if (data[vx,vy]!="X" && data[wx,wy]!="X")
        {
            adj[vx * columns + vy].AddLast(wx * columns + wy);
        }
    }

    public void addObstacle(int row, int col)
    {
        obstacleHandle = true;
        data[row,col] = "X";
        RemoveAdjacencies(row, col);
    }

    public void DFS(int x,int y, Graph graph)
    {
        // Mark all the vertices as not visited 
        // (set as false by default in c#)  
        bool[] visited = new bool[columns * rows];
        for (int i = 0; i < columns * rows; i++)
        {
            visited[i] = false;
        }

        // Call the recursive helper function  
        // to print DFS traversal  
        DFSUtil(x,y, visited);
    }

    void DFSUtil(int x,int y, bool[] visited)
    {
        // Mark the current node as visited 
        // and print it  
        int s = x * rows + y;
        visited[s] = true;
        MoveToTarget(x, y);
        if (data[x, y] == "X")
        {
            return; 
        }
        data[x, y] = "V";

        // Recur for all the vertices  
        // adjacent to this vertex  
        LinkedList<int> vList = new LinkedList<int>(adj[s]);
        foreach (var n in vList)
        {
            if (StaticVars.hasReachTarget)
            {
                return;
            }
            if (!visited[n])
            {
                KeyValuePair<int, int> current = numberToCordMap[n];
                DFSUtil(current.Key,current.Value, visited);
            }
        }
    }

    public void BFS(int x,  int y, Graph graph)
    {
        int s = x * rows + y;
        // Mark all the vertices as not visited(By default
        // set as false)
        bool[] visited = new bool[columns * rows];
        for (int i = 0; i < columns * rows; i++)
        {
            visited[i] = false;
        }

        // Create a queue for BFS
        LinkedList<int> queue = new LinkedList<int>();

        // Mark the current node as visited and enqueue it
        visited[s] = true;
        data[x, y] = "V";
        queue.AddLast(s);
        UpdateCurrentLocation(x, y);

        while (queue.Count != 0)
        {
            // Dequeue a vertex from queue and print it
            s = queue.First.Value;
            queue.RemoveFirst();
            //System.out.print(s+" ");

            // Get all adjacent vertices of the dequeued vertex s
            // If a adjacent has not been visited, then mark it
            // visited and enqueue it
            //Iterator<Integer> i = adj[s].listIterator();
            var temp_iter = adj[s];
            while (temp_iter.Count != 0)
            {
                if (StaticVars.hasReachTarget)
                {
                    return;
                }
                int n = temp_iter.First.Value; // the next vertex to visit
                temp_iter.RemoveFirst();
                if (!visited[n])
                {
                    KeyValuePair<int, int> current = numberToCordMap[n];
                    MoveToTarget(current.Key, current.Value);
                    if (data[current.Key, current.Value] != "X")
                    {
                        data[current.Key, current.Value] = "V";
                    }
                    visited[n] = true;

                }
                queue.AddLast(n);
            }
        }
    }

    private static void UpdateCurrentLocation(int x, int y)
    {
        StaticVars.curRow = x;
        StaticVars.curCol = y;
    }

    private void MoveToTarget(int x_target, int y_target)
    {
        CalculateMoveList(x_target, y_target);
        int prev_x = 0, prev_y = 0;
        var moveList = StaticVars.backtrackMinSol;
        foreach (var item in moveList)
        {
            if (StaticVars.hasReachTarget)
            {
                return;
            }
            prev_x = StaticVars.curRow;
            prev_y = StaticVars.curCol;
            if (item.Item1>StaticVars.curRow && item.Item2 == StaticVars.curCol)
            {
                //move Down
                UpdateCurrentLocation(item.Item1, item.Item2);
                OnLocationChanged.Invoke(2);
                CheckForObstacle(prev_x, prev_y);
                continue;
            }
            if (item.Item1 < StaticVars.curRow && item.Item2 == StaticVars.curCol)
            {
                //move Up
                UpdateCurrentLocation(item.Item1, item.Item2);
                OnLocationChanged.Invoke(0);
                CheckForObstacle(prev_x, prev_y);
                continue;
            }
            if (item.Item1 == StaticVars.curRow && item.Item2 > StaticVars.curCol)
            {
                //move Right
                UpdateCurrentLocation(item.Item1, item.Item2);
                OnLocationChanged.Invoke(1);
                CheckForObstacle(prev_x, prev_y);
                continue;
            }
            if (item.Item1 == StaticVars.curRow && item.Item2 < StaticVars.curCol)
            {
                //move Left
                UpdateCurrentLocation(item.Item1, item.Item2);
                OnLocationChanged.Invoke(3);
                CheckForObstacle(prev_x, prev_y);
                continue;
            }
        }
    }

    private void CheckForObstacle(int prev_x, int prev_y)
    {
        if (obstacleHandle)
        {
            UpdateCurrentLocation(prev_x, prev_y);
            obstacleHandle = false;
        }
    }

    private void CalculateMoveList(int x_target, int y_target)
    {
        //throw new NotImplementedException();
        var new_data = data;
        int[,] sol = new int[rows, columns];
        List<ValueTuple<int, int>> sol2 = new List<ValueTuple<int, int>>();
        StaticVars.backtrackMinCount = int.MaxValue;
        StaticVars.backtrackMinSol = new List<(int, int)>();
        solveMazeUtil(new_data, StaticVars.curRow, StaticVars.curCol,x_target,y_target ,sol,sol2,0);
    }

    private void solveMazeUtil(string[,] maze, int x, int y,int x_target,int y_target, int[,] sol, List<ValueTuple<int, int>> sol2, int count)
    {
        if (x == x_target && y == y_target)
        {
            sol[x,y] = 1;
            sol2.Add((x, y));
            if (count < StaticVars.backtrackMinCount)
            {
                StaticVars.backtrackMinCount = count;
                StaticVars.backtrackMinSol = sol2.Select(cur_item => cur_item).ToList();
            }
            sol[x, y] = 0;
            sol2.Remove((x, y));
            return;
        }
        // Check if maze[x][y] is valid 
        if (isSafe(maze, x, y, sol) == true)
        {
            // mark x, y as part of solution path 
            sol[x, y] = 1;
            sol2.Add((x, y));

            solveMazeUtil(maze, x + 1, y, x_target, y_target, sol, sol2, count + 1);
            solveMazeUtil(maze, x - 1, y, x_target, y_target, sol, sol2, count + 1);
            solveMazeUtil(maze, x, y+1, x_target, y_target, sol, sol2, count + 1);
            solveMazeUtil(maze, x , y-1, x_target, y_target, sol, sol2, count + 1);

            sol[x, y] = 0;
            sol2.Remove((x, y));
        }
    }

    private bool isSafe(string[,] maze, int x, int y,int[,] sol)
    {
        if (x >= 0 && x < rows && y >= 0 && y < columns && maze[x,y] == "V" && sol[x, y] == 0)
            return true;
        return false;
    }
}
