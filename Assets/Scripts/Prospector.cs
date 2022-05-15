using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    public float timer = 2f;
    public float timeBefore;

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
                checkWin();
                break;
            case CardState.tableau:

                bool validMatch = true;
                if (cd.FaceUp == false) validMatch = false;
                if (!AdjacentRank(cd, target)) validMatch = false;
                if (!validMatch)
                {
                    return;
                }
                //Debug.Log("TouchTableU");
                tableau.Remove(cd);
                MoveToTarget(cd);
                confirmCover();
                checkWin();
                break;
        }
    }

    public void checkWin() 
    {
        bool Over = true;
        //bool gameOver = false;
        foreach (CardProspector cd in tableau) 
        {            
            if (cd.FaceUp == true) 
            {
                if (cd.rank - 1 == target.rank || cd.rank + 1 == target.rank)
                {             
                    Over = false;
                    break;
                    //return false;
                }                
            }            
        }

        if (Over)
        {
            Debug.Log("You need Draw A New Card");
        }
        else 
        {
            Debug.Log("Not over Yet");                     
        }

        if (tableau.Count==0) 
        {
            
            //gameOver = true;
            GameOver(true);
                  
        }
        if (drawPile.Count<=0) 
        {
           
            //gameOver = false;
            GameOver(false);                        
        }
    }

    void GameOver(bool gameOver) 
    {
        if (gameOver)
        {
            Debug.Log("You Win!!!!!");
        }
        else 
        {
            Debug.Log("You Lose");
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void confirmCover() 
    {
        foreach (CardProspector temp in tableau) 
        {
            bool up = true;
            foreach (CardProspector cover in temp.hiddenBy) 
            {
                if (cover.state == CardState.tableau) 
                {
                    up = false;
                }
            }
            temp.FaceUp = up;
        }
    }

    public bool AdjacentRank(CardProspector cd,CardProspector target) 
    {
        //两个中有一个朝上，即返回false,感叹号代入到变量去思考
        if (!cd.FaceUp||!target.FaceUp) 
        {
            return false;
        }

        if (Mathf.Abs(cd.rank - target.rank)==1) 
        {
            return true;
        }
        if (cd.rank == 1 && target.rank == 13) return true;
        if (cd.rank == 13 && target.rank == 1) return true;

        return false;
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

    CardProspector FindCardBySlotID(int slotID) 
    {
        foreach (CardProspector cp in tableau) 
        {
            if (cp.slotID == slotID) 
            {
                return cp;
            }
        }
        return null;
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
        float timer = 0;
        foreach (SlotDef sDef in layout.SlotDefs)
        {
            if (sDef.type == "drawpile"||sDef.type == "discardpile") 
            {
                continue;
            }
            cp = Draw();
            cp.FaceUp = sDef.faceUp;
            cp.transform.parent = layoutAnchor;
            IEnumerator putCard = PutCard(cp, sDef,timer);
            //使用协程发牌
            StartCoroutine(putCard);
            //cp.transform.localPosition = new Vector3(layout.multiplier.x * sDef.x, layout.multiplier.y * sDef.y, -sDef.layerID);
            cp.LayerID = sDef.layerID;
            cp.slotID = sDef.slotID;

            //输出层级，和层级ID
            //Debug.Log(sDef.layerName);
            //Debug.Log(sDef.layerID);
            cp.slotDef = sDef;
            cp.SetAllSpriteRendererName(sDef.layerName);
            cp.state = CardState.tableau;

            tableau.Add(cp);
            timer += 0.1f;
        }

        //添加hiddenBy关系
        foreach (CardProspector tcp in tableau)
        {
            foreach (int hiddenID in tcp.slotDef.hiddenBy)
            {
                CardProspector hiddenCp =  FindCardBySlotID(hiddenID);
                tcp.hiddenBy.Add(hiddenCp);
            }
        }

        //移动一张牌到目标区域（弃牌区）
        //对牌堆进行一次排列
        MoveToTarget(Draw());
        UpdateDrawPile();
    }

    IEnumerator PutCard(CardProspector cp,SlotDef slotDef,float timer) 
    {
        yield return new WaitForSeconds(1f+timer);
        
        cp.transform.localPosition = new Vector3(layout.multiplier.x * slotDef.x, layout.multiplier.y * slotDef.y, -slotDef.layerID);        
    }
}
