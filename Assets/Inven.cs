using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inven : MonoBehaviour
{
    public GameObject towerWindow;
    public Button button;

    private void Start()
    {
        button.onClick.AddListener(CloseTowerWindow);
    }
    public void CloseTowerWindow()
    {
        towerWindow.SetActive(!towerWindow.activeSelf);
    }
   
}
