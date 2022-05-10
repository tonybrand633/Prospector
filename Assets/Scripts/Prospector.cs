using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prospector : MonoBehaviour
{
    public static Prospector S;
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

    public void CardClicked(CardProspector cd)
    {
        switch (cd.state)
        {
            case CardState.target:
                break;
            case CardState.drawpile:
                MoveToDiscard(target);
                MoveToTarget(Draw());
                UpdateDrawPile();
                break;
            case CardState.tableau:
                break;
        }
    }

    void MoveToDiscard(CardProspector cd) 
    {
        cd.state = CardState.discard;
        discardPile.Add(cd);
        cd.transform.parent = layoutAnchor;
        //定位到弃牌堆
        cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID + 0.5f);
        cd.FaceUp = true;
        cd.SetAllSpriteRendererName(layout.discardPile.layerName);
        cd.SetAllSpriteRendererOrder(-100 + discardPile.Count);
    }

    void MoveToTarget(CardProspector cd) 
    {
        if (target!=null) 
        {
            MoveToDiscard(target);
        }

        target = cd;
        cd.state = CardState.target;
        cd.transform.parent = layoutAnchor;
        //移动到目标位置
        cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID + 0.5f);
        cd.FaceUp = true;
        //设置深度排序
        cd.SetAllSpriteRendererName(layout.discardPile.layerName);
        cd.SetAllSpriteRendererOrder(0);
    }

    void UpdateDrawPile() 
    {
        CardProspector cd;
        //遍历所有储备牌，即Deck强转过来的drawPile
        for (int i = 0; i < drawPile.Count; i++)
        {
            cd = drawPile[i];
            cd.transform.parent = layoutAnchor;

            Vector2 dpStagger = layout.drawPile.xStagger;

            cd.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x),
                layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
                -layout.drawPile.layerID + 0.1f * i
                );


            cd.FaceUp = false;
            cd.state = CardState.drawpile;

            //设置深度层
            cd.SetAllSpriteRendererName(layout.drawPile.layerName);
            cd.SetAllSpriteRendererOrder(-10 * i);
        }
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

            //输出层级，和层级ID
            //Debug.Log(sDef.layerName);
            //Debug.Log(sDef.layerID);
            cp.slotDef = sDef;
            cp.SetAllSpriteRendererName(sDef.layerName);
            cp.state = CardState.tableau;

            tableau.Add(cp);
        }

        MoveToTarget(Draw());

        UpdateDrawPile();

    }
}
