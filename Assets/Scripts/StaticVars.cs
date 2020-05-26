using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticVars : MonoBehaviour
{
    public static int startRow;
    public static int startCol;

    public static int finishRow;
    public static int finishCol;

    public static int curRow;
    public static int curCol;

    public static string grid;
    public static string algo;

    public static int backtrackMinCount;
    public static List<ValueTuple<int, int>> backtrackMinSol;

    public static GameObject startTile;
}
