using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static bool isReady;

    void Awake()
    {
        if (isReady)
        {
            Destroy(this);
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
