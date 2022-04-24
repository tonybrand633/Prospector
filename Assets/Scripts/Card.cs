using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    //花色
    public string suit;

    //点数
    public int rank;

    //花色符号颜色
    public Color color = Color.black;

    //颜色的名字
    public string colorN = "Black";

    //Decorator游戏对象
    List<GameObject> decoGOs = new List<GameObject>();

    //Pips游戏对象
    List<GameObject> pipGos = new List<GameObject>();

    //卡牌背面
    public GameObject back;

    public CardDefinition def;//解析自DeckXML.xmls

    public bool FaceUp 
    {
        get 
        {
            return back.activeSelf;
        }
        set 
        {
            back.SetActive(!value);
        }
    }

}

[System.Serializable]
public class Decorator 
{
    //存储XML读取的角部点数和花色符号
    //花色
    public string type;

    //存储花色图案的位置
    public Vector3 loc;

    //是否翻转
    public bool flip;

    //缩放
    public float scale = 1f;

}

[System.Serializable]
public class CardDefinition 
{
    //每张牌的花色名称
    public string face;

    //牌的点数
    public int rank;

    //每个牌的位置/装饰
    public List<Decorator> pips = new List<Decorator>();

}
