using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickLose : MonoBehaviour
{
    public Prospector temp;
    void Awake()
    {
        
    }

    void Start()
    {
        temp = GameObject.Find("Main Camera").GetComponent<Prospector>();        
        Button btn = GetComponent<Button>();
        UIEventHandler uIEventHandler = btn.gameObject.AddComponent<UIEventHandler>();
        uIEventHandler.OnClickLose += temp.GameOver;
        //uIEventHandler.OnClickLose += temp.GameOver;
        //uIEventHandler.OnExit += MouseExit;
        //uIEventHandler.OnEnter += MouseEnter;
        //btn.onClick.AddListener(ClickWin);          
    }
}
