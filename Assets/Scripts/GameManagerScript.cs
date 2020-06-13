using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


public class GameManagerScript : MonoBehaviour
{
    enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    [SerializeField]
    public GameObject robotPrefab;
    private robotScript _robotScript;
    private Graph graph;
    private int curRow;
    private int curCol;
    private bool bfsFlag;
    private string logfile;
    // Start is called before the first frame update
    void Start()
    {
        StaticVars.hasReachTarget = false;
        logfile = "";
        bfsFlag = false;
        CreateGraph();
        graph.OnLocationChanged.AddListener(myAction);
        if (!(StaticVars.startRow == StaticVars.finishRow && StaticVars.startCol == StaticVars.finishCol))
        {
            StartAlgo();
        }
        FinishTheRun();

    }



    private void FinishTheRun()
    {
        Debug.Log("Finish");
        logfile += "\n\n\n";
        logfile += ParseTheGridData(graph.getData());
        System.IO.Directory.CreateDirectory(@"C:\Users\Public\AutonomicLogs");
        System.IO.File.WriteAllText(@"C:\Users\Public\AutonomicLogs\LogFile_" + GetTimestamp(DateTime.Now) + ".txt", logfile);
    }

    private string ParseTheGridData(string[,] v)
    {
        string str = "";
        int count = 0;
        foreach (var item in v)
        {
            str += item;
            count++;
            if (count % graph.getColumns() == 0)
                str += "\n";
        }
        return str;
    }

    public static String GetTimestamp(DateTime value)
    {
        return value.ToString("yyyyMMddHHmmssffff");
    }

    void myAction(int n)
    {
        //StartCoroutine(Mover(n));
        int result = _robotScript.Move(n);
        WriteToLogFile(result);
        if (result == 1)
        {
            StaticVars.hasReachTarget = true;
        }
        if (result == -1)
        {
            graph.addObstacle(StaticVars.curRow, StaticVars.curCol);
        }
    }

    private void WriteToLogFile(int result)
    {
        switch (result)
        {
            case -3:
                logfile += "UNEXPECTED ERROR\n";
                break;
            case -2:
                logfile += "Out of bounds at: " + StaticVars.curRow + "," + StaticVars.curCol + "\n";
                break;
            case -1:
                logfile += "The robot has encountred an obstacle at: " + StaticVars.curRow + "," + StaticVars.curCol + "\n";
                break;
            case 0:
                logfile += "The robot has successfully moved to: " + StaticVars.curRow + "," + StaticVars.curCol + "\n";
                break;
            case 1:
                logfile += "The robot has reached the target at: " + StaticVars.curRow + "," + StaticVars.curCol + "\n";
                break;
            default:
                logfile += "UNEXPECTED CODE\n";
                break;
        }
    }

    IEnumerator waitFor1Seconds()
    {
        Debug.Log("start wait");
        yield return new WaitForSeconds(1);
        Debug.Log("finish wait");
    }

    private void CreateGraph()
    {
        string data = StaticVars.grid;
        string[] dataLines = data.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        int rows = dataLines.Length;
        int cols = dataLines[0].Length;
        string[,] dataForGraph = new string[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                dataForGraph[i, j] = dataLines[i][j].ToString();
            }
        }
        graph = new Graph(cols, rows, dataForGraph);
    }

    private void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            ResetStaticVars();
            SceneManager.LoadScene(0);
        }
    }

    private void ResetStaticVars()
    {
        StaticVars.startRow = -1;
        StaticVars.startCol = -1;
        StaticVars.finishRow = -1;
        StaticVars.finishCol = -1;
        StaticVars.curRow = -1;
        StaticVars.curCol = -1;

        StaticVars.grid = "";
        StaticVars.algo = "";

        StaticVars.hasReachTarget = false;

        StaticVars.startTile = null;
}

    private void StartAlgo()
    {
        GameObject robot = (GameObject)Instantiate(robotPrefab);
        _robotScript = robot.GetComponent<robotScript>();
        switch (StaticVars.algo)
        {
            case "BFS":
                graph.BFS(StaticVars.startRow, StaticVars.startCol, graph);
                break;
            case "DFS":
                graph.DFS(StaticVars.startRow, StaticVars.startCol, graph);
                break;
            default:
                throw new Exception("No Algo");
        }
    }

    private IEnumerator Mover(int direction)
    {
        yield return new WaitForSeconds(1);
        int result = _robotScript.Move(direction);
        Debug.Log(result);
    }
}
