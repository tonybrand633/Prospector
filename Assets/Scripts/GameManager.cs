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
            DontDestroyOnLoad(this);
        }
        else 
        {
            Destroy(this);
        }
        //if (canvas==null) 
        //{
        //    canvas = Resources.Load<GameObject>("Canvas");
        //    Instantiate(canvas);
        //}        
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        uiText = canvas.GetComponentsInChildren<Text>();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
