using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prospector : MonoBehaviour
{
    public Prospector S;
    public Deck deck;
    public TextAsset xmlText;

    public Layout layout;
    public TextAsset layoutText;

    //用于设置布局的变量
    public Vector3 layoutCenter;
    public float xoffset = 3f;
    public float yoffset = -2.5f;
    public Transform layoutAnchor;

    public CardProspector target;
    public List<CardProspector> tableau;
    public List<CardProspector> discardPile;



    public List<CardProspector> drawPile;

    // Start is called before the first frame update

    void Awake()
    {
        S = this;    
    }

    void Start()
    {
        //初始化记录纸牌
        deck = GetComponent<Deck>();
        deck.InitDeck(xmlText.text);
        deck.ShuffCard(ref deck.cards);
        drawPile = ConvertCardToCardProspector(deck.cards);
        //初始化记录布局
        layout = GetComponent<Layout>();
        layout.ReadLayOut(layoutText.text);
        
        LayGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //转换纸牌类型
    List<CardProspector> ConvertCardToCardProspector(List<Card>cards) 
    {
        List<CardProspector> cardProspectors = new List<CardProspector>();
        CardProspector CP;
        foreach  (Card card in cards)
        {
            CP = card as CardProspector;
            cardProspectors.Add(CP);
        }
        return cardProspectors;
    }

    //我的回合！抽卡！
    CardProspector Draw() 
    {
        //随机抽走了布局的卡片
        CardProspector card = drawPile[0];        
        drawPile.RemoveAt(0);
        //Debug.Log(drawPile.Count);
        return card;
    }

    //创建游戏布局
    void LayGame() 
    {
        if (layoutAnchor==null) 
        {
            GameObject anchorGameobject = new GameObject("_anchorGameobject");
            layoutAnchor = anchorGameobject.transform;
            //定位
            layoutAnchor.transform.position = layoutCenter;
        }

        CardProspector cp;
        foreach (SlotDef sDef in layout.SlotDefs)
        {
            cp = Draw();
            cp.FaceUp = sDef.faceUp;
            cp.transform.parent = layoutAnchor;
            cp.transform.localPosition = new Vector3(layout.multiplier.x * sDef.x, layout.multiplier.y * sDef.y, -sDef.layerID);

            cp.LayerID = sDef.layerID;
            cp.slotDef = sDef;
            cp.SetAllSpriteRendererName(sDef.layerName);
            cp.state = CardState.tableau;

            tableau.Add(cp);

        }

    }
}
