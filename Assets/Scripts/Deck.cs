using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public bool ___________________;

    public XMLReader xmlr;

    public List<string> cardName;
    public List<Card> cards;
    public List<Decorator> decorators;
    public List<CardDefinition> cardDefs;
    public Transform deckAnchor;
    public Dictionary<string, Sprite> dictSuit;

    public void InitDeck(string deckXMLText) 
    {
        ReadDeck(deckXMLText);
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
}
