using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    //花色
    public Sprite suitClub;//梅花
    public Sprite suitDiamond;//方片
    public Sprite suitHeart;//红心
    public Sprite suitSpade;//黑桃

    public Sprite[] faceSprites;
    public Sprite[] rankSprites;


    public Sprite cardback;
    public Sprite cardbackGold;
    public Sprite cardfront;
    public Sprite cardfrontGold;

    //预制
    public GameObject prefabSprite;
    public GameObject prefabCard;



    public bool ___________;

    public PT_XMLReader xmlr;
    public List<string> cardNames;
    public List<Card> cards;
    public List<Decorator> decorators;
    public List<CardDefinition> cardDefs;

    public Transform DeckAnchor;
    public Dictionary<string, Sprite> deckSuits;

    public void InitDeck(string XMLText) 
    {       
        if (GameObject.Find("_Deck") == null)
        {
            GameObject _Deck = new GameObject("_Deck");
            //_Deck.transform.position = new Vector3(2, 2, 2);
            DeckAnchor = _Deck.transform;
        }

        deckSuits = new Dictionary<string, Sprite>()
        {
            { "C",suitClub},
            { "D",suitDiamond},
            { "H",suitHeart},
            { "S",suitSpade}
        };
        ReadDeck(XMLText);
    }

    public void ReadDeck(string xmlText) 
    {
        xmlr = new PT_XMLReader();
        xmlr.Parse(xmlText);


        //string s = "x:";
        //s += xmlr.xml["xml"][0]["card"][0]["pip"][0].att("x");
        //s += "y:";
        //s+= s += xmlr.xml["xml"][0]["card"][0]["pip"][0].att("y");
        //s += "z:";
        //s += xmlr.xml["xml"][0]["card"][0]["pip"][0].att("z");
        //s += "flip:";
        //s += xmlr.xml["xml"][0]["card"][0]["pip"][0].att("flip");
        //s += "scale:";
        //s += xmlr.xml["xml"][0]["card"][0]["pip"][0].att("scale");

        //Debug.Log(s);
        //读取所有角码
        decorators = new List<Decorator>();
        PT_XMLHashList Decos = xmlr.xml["xml"][0]["decorator"];

        Decorator deco;
        //添加decorator信息至decorators
        for (int i = 0; i < Decos.Count; i++)
        {
            deco = new Decorator();

            string type = Decos[i].att("type");
            deco.type = type;

            deco.loc.x = float.Parse(Decos[i].att("x"));
            deco.loc.y = float.Parse(Decos[i].att("y"));
            deco.loc.z = float.Parse(Decos[i].att("z"));            

            deco.flip = Decos[i].att("flip") == "1";

            deco.scale = float.Parse(Decos[i].att("scale"));
            decorators.Add(deco);
        }

        cardDefs = new List<CardDefinition>();
        PT_XMLHashList PT_Cards = xmlr.xml["xml"][0]["card"];
        CardDefinition cardDef;

        for (int i = 0; i < PT_Cards.Count; i++)
        {
            cardDef = new CardDefinition();

            cardDef.rank = int.Parse(PT_Cards[i].att("rank"));
            if (PT_Cards[i]["pip"]!=null)
            {
                PT_XMLHashList Pips = PT_Cards[i]["pip"];
                if (Pips.Count!=0) 
                {
                    for (int j = 0; j < Pips.Count; j++)
                    {
                        deco = new Decorator();
                        deco.loc.x = float.Parse(Pips[j].att("x"));
                        deco.loc.y = float.Parse(Pips[j].att("y"));
                        deco.loc.z = float.Parse(Pips[j].att("z"));

                        deco.flip = Pips[j].att("flip") == "1";
                        if (Pips[j].HasAtt("scale"))
                        {
                            deco.scale = float.Parse(Pips[j].att("scale"));
                        }
                        cardDef.pips.Add(deco);
                    }
                }                
            }
            
            
            if (PT_Cards[i].HasAtt("face")) 
            {
                cardDef.face = PT_Cards[i].att("face");
            }
            cardDefs.Add(cardDef);
        }
        MakeCards();
        ShuffCard(ref cards);
    }

    public CardDefinition GetDefByRank(int rank) 
    {
        foreach (CardDefinition def in cardDefs)
        {
            if (def.rank==rank) 
            {
                return def;
            }
        }
        return null;
    }

    public Sprite GetFace(string faceName) 
    {
        foreach (Sprite target in faceSprites)
        {
            if (target.name == faceName) 
            {
                return target;
            }
        }
        return null;
    }

    public void MakeCards() 
    {
        //cardNames = new List<string>();
        List<string> letters = new List<string>() { "C", "D", "H", "S" };

        foreach (string l in letters)
        {
            for (int i = 0; i < 13; i++)
            {
                cardNames.Add(l+(i + 1));
            }
        }

        cards = new List<Card>();
        Sprite suitS;
        SpriteRenderer sr;
        for (int i = 0; i < cardNames.Count; i++)
        {
            GameObject cardGo = Instantiate(prefabCard);
            cardGo.transform.parent = DeckAnchor;
            Card card = cardGo.GetComponent<Card>();
            cardGo.transform.position = new Vector3(i%13*3,i/13*4,0);
            cardGo.name = cardNames[i];
            string cardName = cardNames[i];
            card.suit = cardName[0].ToString();
            card.rank = int.Parse(cardName.Substring(1));
            suitS = deckSuits[card.suit];            
            if (card.suit=="D"||card.suit=="H") 
            {
                card.color = Color.red;
                card.colorN = "Red";
            }

            foreach (Decorator decoSuit in decorators)
            {
                GameObject decoS = Instantiate(prefabSprite);
                sr = decoS.GetComponent<SpriteRenderer>();
                decoS.transform.parent = cardGo.transform;

                if (decoSuit.type == "letter")
                {
                    sr.sprite = rankSprites[i%13+1];
                    sr.sortingLayerName = "letter";
                    //sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);

                    //Debug.Log(sr.color.gamma);
                    if (card.colorN == "Red")
                    {
                        sr.color = Color.red;
                    }
                    else
                    {
                        sr.color = Color.black;
                    }

                    sr.flipY = decoSuit.flip;
                    decoS.transform.localPosition = decoSuit.loc;
                    decoS.transform.localScale = Vector3.one * decoSuit.scale;
                    decoS.name = decoSuit.type;

                } else if (decoSuit.type == "suit") 
                {
                    sr.sprite = suitS;
                    sr.flipY = decoSuit.flip;
                    decoS.transform.localPosition = decoSuit.loc;
                    decoS.transform.localScale = Vector3.one * decoSuit.scale;
                    decoS.name = decoSuit.type;
                    sr.sortingLayerName = "deco";
                }
            }


            CardDefinition thisDef = GetDefByRank(card.rank);

            //int index = 0;
            foreach (Decorator deco in thisDef.pips) 
            {
                GameObject decoGo = Instantiate(prefabSprite);
                decoGo.transform.parent = cardGo.transform;
                sr = decoGo.GetComponent<SpriteRenderer>();
                sr.sprite = suitS;
                decoGo.transform.localPosition = deco.loc;
                decoGo.transform.localScale = Vector3.one * deco.scale;
                decoGo.name = "deco";
                sr.flipY = deco.flip;
                sr.sortingLayerName = "deco";
                //decoGo.transform.localScale = deco.scale;

            }
            if (thisDef.face != "")
            {
                string utilName = "FaceCard_";
                GameObject faceGo = Instantiate(prefabSprite);
                string spriteName = utilName + card.rank + card.suit;
                
                Sprite ts = GetFace(spriteName);
                sr = faceGo.GetComponent<SpriteRenderer>();
                sr.sprite = ts;
                faceGo.transform.parent = cardGo.transform;
                faceGo.transform.localPosition = Vector3.zero;
                faceGo.name = "faceGo";
                sr.sortingLayerName = "deco";
            }

            GameObject back = Instantiate(prefabSprite);
            sr = back.GetComponent<SpriteRenderer>();
            sr.sprite = cardback;            
            back.transform.parent = cardGo.transform;
            back.transform.localPosition = Vector3.zero;
            sr.sortingLayerName = "back";
            card.back = back;
            card.FaceUp = true;

            cards.Add(card);
        }
    }

    public void ShuffCard(ref List<Card>cards) 
    {
        List<Card> tempCard = new List<Card>();
        int index;
        
        while (cards.Count>0) 
        {
            index = Random.Range(0, cards.Count);
            tempCard.Add(cards[index]);
            cards.RemoveAt(index);
        }
        cards = tempCard;
    }
}
