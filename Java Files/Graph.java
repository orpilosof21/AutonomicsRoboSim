import javafx.util.Pair;

import java.util.*;
import java.util.stream.Collectors;

public class Graph {
    private int columns;
    private int rows;
    private int tiles;
    private boolean obstacleHandle;
    private LinkedList<Integer>[] adj; //Adjacency Lists
    private String[][]data;
    private Dictionary<Integer, Pair<Integer,Integer>> numberToCordMap;

    public int getRows() {
        return rows;
    }

    public int getColumns() {
        return columns;
    }

    public String[][] getData() {
        return data;
    }

    public Graph(int columns, int rows, String[][] data)
    {
        this.obstacleHandle = false;
        this.columns = columns;
        this.rows = rows;
        this.data = data;
        this.tiles = rows * columns;
        this.adj = new LinkedList[tiles];
        for (int i = 0; i < tiles; ++i)
        {
            this.adj[i] = new LinkedList();
        }
        this.fillTheMap();
        this.CreateAdjacencies();
    }

    void fillTheMap()
    {
        numberToCordMap= new Hashtable<Integer, Pair<Integer,Integer>>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                numberToCordMap.put(i*columns+j, new Pair(i,j));
            }
        }
    }

    void CreateAdjacencies()
    {
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < columns; j++)
            {
                if (data[i][j] != "X")
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
        for (var item : adj)
        {
            if (item.contains(row * columns + col)){
                item.remove(row * columns + col);
            }
        }
        adj[row * columns + col].clear();
    }

    void addEdge(int vx, int vy, int wx, int wy)
    {
        if (wx < 0 || wx >= rows || wy < 0 || wy >= columns)
        {
            return;
        }
        if (!data[vx][vy].equals("X") && !data[wx][wy].equals("X"))
        {
            adj[vx * columns + vy].add(wx * columns + wy);
        }
    }

    public void addObstacle(int row, int col)
    {
        obstacleHandle = true;
        data[row][col] = "X";
        RemoveAdjacencies(row, col);
    }

    public void DFS(int x,int y, Graph graph)
    {
        // Mark all the vertices as not visited
        boolean[] visited = new boolean[columns * rows];
        for (int i = 0; i < columns * rows; i++)
        {
            visited[i] = false;
        }

        // Call the recursive helper function
        // to print DFS traversal
        DFSUtil(x,y, visited);
    }

    void DFSUtil(int x,int y, boolean[] visited)
    {
        // Mark the current node as visited
        // and print it
        int s = x * rows + y;
        visited[s] = true;
        MoveToTarget(x, y);
        if (data[x][y].equals("X"))
        {
            return;
        }
        data[x][y] = "V";

        // Recur for all the vertices
        // adjacent to this vertex
        LinkedList<Integer> vList = new LinkedList<Integer>(adj[s]);
        for (var n : vList)
        {
            if (StaticVars.hasReachTarget)
            {
                return;
            }
            if (!visited[n])
            {
                Pair<Integer,Integer> current = numberToCordMap.get(n);
                DFSUtil(current.getKey(),current.getValue(), visited);
            }
        }
    }

    public void BFS(int x,  int y, Graph graph)
    {
        int s = x * rows + y;
        // Mark all the vertices as not visited(By default
        // set as false)
        boolean visited[] = new boolean[columns * rows];
        for (int i = 0; i < columns * rows; i++)
        {
            visited[i] = false;
        }

        // Create a queue for BFS
        LinkedList<Integer> queue = new LinkedList<Integer>();

        // Mark the current node as visited and enqueue it
        visited[s] = true;
        data[x][y] = "V";
        queue.add(s);
        UpdateCurrentLocation(x, y);

        while (queue.size() != 0)
        {
            // Dequeue a vertex from queue and print it
            s = queue.poll();

            // Get all adjacent vertices of the dequeued vertex s
            // If a adjacent has not been visited, then mark it
            // visited and enqueue it
            Iterator<Integer> i = adj[s].listIterator();
            while (i.hasNext())
            {
                if (StaticVars.hasReachTarget)
                {
                    return;
                }
                int n = i.next();
                if (!visited[n])
                {
                    Pair<Integer,Integer> current = numberToCordMap.get(n);
                    var cur_x = current.getKey();
                    var cur_y = current.getValue();
                    MoveToTarget(cur_x, cur_y);
                    if (!(data[cur_x][cur_y].equals("X")))
                    {
                        data[cur_x][cur_y] = "V";
                    }
                    visited[n] = true;

                }
                queue.add(n);
            }
        }
    }

    private void MoveToTarget(Integer x_target, Integer y_target) {
        CalculateMoveList(x_target, y_target);
        int prev_x = 0, prev_y = 0;
        var moveList = StaticVars.backtrackMinSol;
        for (var item : moveList)
        {
            if (StaticVars.hasReachTarget)
            {
                return;
            }
            prev_x = StaticVars.curRow;
            prev_y = StaticVars.curCol;
            if (item.getKey()>StaticVars.curRow && item.getValue() == StaticVars.curCol)
            {
                //move Down
                UpdateCurrentLocation(item.getKey(), item.getValue());
                // TODO This is where we tell the car to which direction to go
                //OnLocationChanged.Invoke(2);
                CheckForObstacle(prev_x, prev_y);
                continue;
            }
            if (item.getKey() < StaticVars.curRow && item.getValue() == StaticVars.curCol)
            {
                //move Up
                UpdateCurrentLocation(item.getKey(), item.getValue());
                // TODO This is where we tell the car to which direction to go
                //OnLocationChanged.Invoke(0);
                CheckForObstacle(prev_x, prev_y);
                continue;
            }
            if (item.getKey() == StaticVars.curRow && item.getValue() > StaticVars.curCol)
            {
                //move Right
                UpdateCurrentLocation(item.getKey(), item.getValue());
                // TODO This is where we tell the car to which direction to go
                //OnLocationChanged.Invoke(1);
                CheckForObstacle(prev_x, prev_y);
                continue;
            }
            if (item.getKey() == StaticVars.curRow && item.getValue() < StaticVars.curCol)
            {
                //move Left
                UpdateCurrentLocation(item.getKey(), item.getValue());
                // TODO This is where we tell the car to which direction to go
                //OnLocationChanged.Invoke(3);
                CheckForObstacle(prev_x, prev_y);
                continue;
            }
        }
    }

    private void CalculateMoveList(Integer x_target, Integer y_target) {
        var new_data = data;
        int[][] sol = new int[rows][columns];
        List<Pair<Integer, Integer>> sol2 = new ArrayList<>();
        StaticVars.backtrackMinCount = Integer.MAX_VALUE;
        StaticVars.backtrackMinSol = new ArrayList<>();
        solveMazeUtil(new_data, StaticVars.curRow, StaticVars.curCol,x_target,y_target ,sol,sol2,0);
    }

    private void solveMazeUtil(String[][] maze, int x, int y,int x_target,int y_target, int[][] sol, List<Pair<Integer, Integer>> sol2, int count)
    {
        if (x == x_target && y == y_target)
        {
            sol[x][y] = 1;
            var entry = new Pair<>(x, y);
            sol2.add(entry);
            if (count < StaticVars.backtrackMinCount)
            {
                StaticVars.backtrackMinCount = count;
                StaticVars.backtrackMinSol = sol2.stream().collect(Collectors.toList());
                //StaticVars.backtrackMinSol = sol2.Select(cur_item => cur_item).ToList();
            }
            sol[x][y] = 0;
            sol2.remove(entry);
            return;
        }
        // Check if maze[x][y] is valid
        if (isSafe(maze, x, y, sol) == true)
        {
            // mark x, y as part of solution path
            sol[x][y] = 1;
            var entry = new Pair<>(x, y);
            sol2.add(entry);

            solveMazeUtil(maze, x + 1, y, x_target, y_target, sol, sol2, count + 1);
            solveMazeUtil(maze, x - 1, y, x_target, y_target, sol, sol2, count + 1);
            solveMazeUtil(maze, x, y+1, x_target, y_target, sol, sol2, count + 1);
            solveMazeUtil(maze, x , y-1, x_target, y_target, sol, sol2, count + 1);

            sol[x][y] = 0;
            sol2.remove(entry);
        }
    }

    private boolean isSafe(String[][] maze, int x, int y,int[][] sol)
    {
        if (x >= 0 && x < rows && y >= 0 && y < columns && maze[x][y].equals("V") && sol[x][y] == 0) {
            return true;
        }
        return false;
    }

    private void CheckForObstacle(int prev_x, int prev_y) {
        if (obstacleHandle)
        {
            UpdateCurrentLocation(prev_x, prev_y);
            obstacleHandle = false;
        }
    }

    private void UpdateCurrentLocation(int x, int y) {
        StaticVars.curRow = x;
        StaticVars.curCol = y;
    }
}
