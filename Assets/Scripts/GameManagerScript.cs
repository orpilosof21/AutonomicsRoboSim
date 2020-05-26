using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        logfile = "";
        bfsFlag = false;
        CreateGraph();
        graph.OnLocationChanged.AddListener(myAction);
        StartAlgo();
        Debug.Log("Finish");
        System.IO.File.WriteAllText(@"C:\Users\Public\TestFolder\LogFile_"+ GetTimestamp(DateTime.Now) + ".txt", logfile);

    }

    public static String GetTimestamp(DateTime value)
    {
        return value.ToString("yyyyMMddHHmmssffff");
    }

    void myAction(int n)
    {
        //StartCoroutine(Mover(n));
        int result = _robotScript.Move(n);
        logfile += "The robot is at " + StaticVars.curRow + "," + StaticVars.curCol + "\n";
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
            SceneManager.LoadScene(0);
        }
    }

    private void StartAlgo()
    {
        GameObject robot = (GameObject)Instantiate(robotPrefab);
        _robotScript = robot.GetComponent<robotScript>();
        graph.BFS(StaticVars.startRow, StaticVars.startCol, graph);
    }

    private IEnumerator Mover(int direction)
    {
        yield return new WaitForSeconds(1);
        int result = _robotScript.Move(direction);
        Debug.Log(result);
    }
}
