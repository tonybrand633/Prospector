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


    public SpriteRenderer[] spriteRenderers;

    void Start()
    {
        SetAllSpriteRendererOrder(0);        
    }

    public bool FaceUp 
    {
        get 
        {
            return !back.activeSelf;
        }
        set 
        {
            back.SetActive(!value);
        }
    }


    public virtual void OnMouseUpAsButton() 
    {
        //print(name);
    }

    public void GetAllSpriteRenderer ()
    {
        if (spriteRenderers==null||spriteRenderers.Length==0) 
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
    }

    public void SetAllSpriteRendererName(string LName) 
    {
        GetAllSpriteRenderer();
        foreach (SpriteRenderer sr in spriteRenderers)
        {            
            sr.sortingLayerName = LName;

        }
    }

    public void SetAllSpriteRendererOrder(int OIndex) 
    {
        GetAllSpriteRenderer();
        foreach (SpriteRenderer sr in spriteRenderers) 
        {            
            if (sr.gameObject.name==this.gameObject.name) 
            {
                sr.sortingOrder = OIndex;
                continue;
            }
            switch (sr.gameObject.name) 
            {
                case "back":
                    sr.sortingOrder = OIndex + 2;
                    break;
                case "face":
                default:
                    sr.sortingOrder = OIndex + 1;
                    break;
            }
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

public class CardDefinition 
{
    //每张牌的花色名称
    public string face;

    //牌的点数
    public int rank;

    //每个牌的位置/装饰
    public List<Decorator> pips = new List<Decorator>();

}
