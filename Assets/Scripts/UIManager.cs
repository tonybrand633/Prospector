using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static bool isReady;
    public Button[] buttons;
    void Awake()
    {
        Debug.Log("isReady:" + isReady);
        buttons = this.GetComponentsInChildren<Button>();
        if (isReady)
        {
            Destroy(this.gameObject);
        }
        else 
        {
            DontDestroyOnLoad(this);
            isReady = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.name = "Canvas";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
