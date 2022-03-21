using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card: MonoBehaviour
{

}

//为了显示在Inspector面板中
[System.Serializable]
public class Decorator
{
    public string type;
    public Vector3 loc;
    public bool flip = false;
    public float scale = 1f;
}

[System.Serializable]
public class CardDefinition
{
    public string face;//存储J,Q,K信息
    public int rank;//存储点数1~13
    public List<Decorator> pips = new List<Decorator>();//用到的花色符号信息
}    

