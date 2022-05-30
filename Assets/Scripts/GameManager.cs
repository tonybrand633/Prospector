using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager S;
    public GameObject canvas;
    public Text[] uiText;

    void Awake()
    {
        
        if (S == null)
        {
            S = this;            
        }
        
        //if (GameObject.Find("Canvas")!=null)
        //{
        //    Debug.Log("Don't Find Canvas");
        //    canvas = Resources.Load<GameObject>("Canvas");
        //    Instantiate(canvas);
        //    canvas.name = "Canvas";
        //}
    }

    // Start is called before the first frame update
    void Start()
    {
        
        //uiText = canvas.GetComponentsInChildren<Text>();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
