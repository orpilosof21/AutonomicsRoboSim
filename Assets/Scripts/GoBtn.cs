﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoBtn : MonoBehaviour
{
    private Button goBtn;

    [SerializeField]
    private InputField rowStart;
    [SerializeField]
    private InputField colStart;
    [SerializeField]
    private InputField rowFinish;
    [SerializeField]
    private InputField colFinish;
    [SerializeField]
    private Dropdown algoSelect;
    [SerializeField]
    private InputField gridMap;
    // Start is called before the first frame update
    void Start()
    {
        goBtn = GetComponent<Button>();
        goBtn.onClick.AddListener(StartSim);
    }

    private void StartSim()
    {
        StaticVars.startRow = int.Parse(rowStart.text);
        StaticVars.startCol = int.Parse(colStart.text);
        StaticVars.finishRow = int.Parse(rowFinish.text);
        StaticVars.finishCol = int.Parse(colFinish.text);
        StaticVars.algo = algoSelect.options[algoSelect.value].text;
        StaticVars.grid = gridMap.text;
        SceneManager.LoadScene(1);
    }
}
