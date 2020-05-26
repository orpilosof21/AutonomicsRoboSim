using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitBtn : MonoBehaviour
{
    private Button quitBtn;
    // Start is called before the first frame update
    void Start()
    {
        quitBtn = GetComponent<Button>();
        quitBtn.onClick.AddListener(QuitApp);
    }

    private void QuitApp()
    {
        Application.Quit(); 
    }
}
