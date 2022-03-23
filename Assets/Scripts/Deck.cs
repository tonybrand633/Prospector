using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    //花色
    public Sprite suitClub; //梅花
    public Sprite suitDiamond; //方片
    public Sprite suitHeart; //红桃
    public Sprite suitSpade; //黑桃
    public Sprite[] faceSprites; //花牌
    public Sprite[] rankSprites; //点数
    public Sprite cardBack; //背面普通
    public Sprite cardBackGold; //背面金色
    public Sprite cardFront; //正面普通
    public Sprite cardFrontGold; //正面金色

    //预设
    public GameObject prefabSprite; 
    public GameObject prefabCard;

    public bool ___________________;

    public XMLReader xmlr;

    public List<string> cardNames;
    public List<Card> cards;
    public List<Decorator> decorators;
    public List<CardDefinition> cardDefs;
    public Transform deckAnchor;
    public Dictionary<string, Sprite> dictSuit;

    public void InitDeck(string deckXMLText) 
    {
        //以下语句为层级面板中的所有Card游戏对象创建一个锚点
        if (GameObject.Find("_Deck")==null) 
        {
            GameObject anchorGo = new GameObject("_Deck");
            deckAnchor = anchorGo.transform;
        }

        //创建字典，用缩写来对应对应的Sprite
        dictSuit = new Dictionary<string, Sprite>()
        {
            { "C",suitClub},
            { "D",suitDiamond},
            { "H",suitHeart},
            { "S",suitSpade}
        };

        //读取xml存储至decorators,cardDefinition
        ReadDeck(deckXMLText);
        MakeCard();
        
    }

    public void ReadDeck(string deckXMLText) 
    {
        xmlr = new XMLReader();
        xmlr.Parse(deckXMLText);
        //string res = "";
        //res = xmlr.res["xml"][0]["decorator"][1].GetAttr("type");
        //Debug.Log(res);

        //可以理解为卡牌的背景?
        decorators = new List<Decorator>();

        XMLHashList decoHashList = xmlr.res["xml"][0]["decorator"];
        
        for (int i = 0; i < decoHashList.Count; i++)
        {
            Decorator deco = new Decorator();

            deco.flip = float.Parse(decoHashList[i].GetAttr("flip"))==1f;
            deco.type = decoHashList[i].GetAttr("type");
            deco.scale = float.Parse(decoHashList[i].GetAttr("scale"));

            deco.loc.x = float.Parse(decoHashList[i].GetAttr("x"));
            deco.loc.y = float.Parse(decoHashList[i].GetAttr("y"));
            deco.loc.z = float.Parse(decoHashList[i].GetAttr("z"));
            decorators.Add(deco);
        }


        //CardDefinition
        cardDefs = new List<CardDefinition>();

        XMLHashList cardDefiList = xmlr.res["xml"][0]["card"];

        for (int i = 0; i < cardDefiList.Count; i++)
        {
            Decorator deco = new Decorator();
            CardDefinition cDef = new CardDefinition();

            //存储rank
            cDef.rank = int.Parse(cardDefiList[i].GetAttr("rank"));

            XMLHashList xPips = cardDefiList[i]["pip"];
            if (xPips!=null) 
            {
                for (int j = 0; j < xPips.Count; j++)
                {
                    //遍历所有的pips标签
                    deco = new Decorator();

                    deco.type = "pip";
                    deco.flip = xPips[j].GetAttr("flip") == "1";
                    deco.loc.x = float.Parse(xPips[j].GetAttr("x"));
                    deco.loc.y = float.Parse(xPips[j].GetAttr("y"));
                    deco.loc.z = float.Parse(xPips[j].GetAttr("z"));
                    if (xPips[j].HasAttr("scale")) 
                    {
                        deco.scale = float.Parse(xPips[j].GetAttr("scale"));
                    }
                    cDef.pips.Add(deco);
                }                
            }
            if (cardDefiList[i].HasAttr("face")) 
            {
                cDef.face = cardDefiList[i].GetAttr("face");
            }

            cardDefs.Add(cDef);
        }
    }

    //根据点数（1~13）分别代表着A-K，获取对应的CardDefinition（牌面布局定义）
    public CardDefinition GetCardDefinitionByRank(int rnk) 
    {
        foreach (CardDefinition cd in cardDefs) 
        {
            if (cd.rank == rnk) 
            {
                return cd;
            }
        }
        return null;
    }

    //创建游戏对象
    public void MakeCard() 
    {
        cardNames = new List<string>();
        string[] letters = new string[] { "C", "D", "H", "S" };
        foreach (string s in letters)
        {
            for (int i = 0; i < 13; i++)
            {
                //所有牌的名字，C1,D12类似这种
                cardNames.Add(s + (i + 1));
            }
        }

        //该List存储所有纸牌
        cards = new List<Card>();
        Sprite tS = null;
        GameObject tGo = null;
        SpriteRenderer tSR = null;

        //遍历前面得到的所有纸牌名称
        for (int i = 0; i < cardNames.Count; i++)
        {
            //创建一个对象
            GameObject cgo = Instantiate(prefabCard) as GameObject;
            cgo.transform.parent = deckAnchor;
            Card card = cgo.GetComponent<Card>();

            //这个数学能力太强了
            cgo.transform.localPosition = new Vector3((i%13*3),i/13*4,0);

            //设置纸牌的基本属性
            card.name = cardNames[i];
            card.suit = card.name[0].ToString();
            card.rank = int.Parse(card.name.Substring(1));

            if (card.suit == "D" || card.suit == "H") 
            {
                card.color = Color.red;
                card.colN = "Red";
            }

            card.def = GetCardDefinitionByRank(card.rank);

            //添加角码
            foreach (Decorator deco in decorators)
            {
                if (deco.type == "suit")
                {
                    tGo = Instantiate(prefabSprite) as GameObject;
                    tSR = tGo.GetComponent<SpriteRenderer>();
                    tSR.sprite = dictSuit[card.suit];
                }
                else 
                {
                    tGo = Instantiate(prefabSprite) as GameObject;
                    tSR = tGo.GetComponent<SpriteRenderer>();
                    tS = rankSprites[card.rank];
                    tSR.sprite = tS;
                    tSR.color = card.color;
                }
                //让角码显示在纸牌上
                tSR.sortingOrder = 1;
                tGo.transform.parent = cgo.transform;
                //根据XML设置位置
                tGo.transform.localPosition = deco.loc;
                if (deco.flip) 
                {
                    tGo.transform.rotation = Quaternion.Euler(0, 0, 180f);
                }

                if (deco.scale!=1) 
                {
                    tGo.transform.localScale = Vector3.one * deco.scale;
                }

                tGo.name = deco.type;
                card.decoGos.Add(tGo);
            }

            //添加中间的花色符号

            cards.Add(card);
        }
    }
}
