using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card: MonoBehaviour
{
    public string suit; //牌的花色
    public int rank; //牌的点数
    public Color color = Color.black; //花色符号的颜色
    public string colN = "Black"; //颜色的名称

    //存储所有的decorator游戏物体
    public List<GameObject> decoGos = new List<GameObject>();
    //存储所有的pips游戏物体
    public List<GameObject> pipGos = new List<GameObject>();

    public GameObject back; //纸牌背面
    public CardDefinition def; //来自解析DeckXML
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

