using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SlotDef 
{
    public int slotID;
    public int layerID = 0;
    public string layerName = "Default";
    public string type = "slot";
    public float x;
    public float y;
    public bool faceUp = false;

    public List<int> hiddenBy = new List<int>();
    public Vector2 xStagger;
}

public class Layout : MonoBehaviour
{
    PT_XMLReader xmlr = new PT_XMLReader();
    PT_XMLHashtable xml;
    //牌的间距
    public Vector2 multiplier;
    public List<SlotDef> SlotDefs = new List<SlotDef>();
    public SlotDef drawPile;
    public SlotDef discardPile;
    //根据LayerID来读取LayerName
    public string[] LayerIDName = new string[] { "Raw0", "Raw1", "Raw2", "Raw3", "Discard", "Draw" };

    public void ReadLayOut(string xmlText) 
    {

        xmlr.Parse(xmlText);
        xml = xmlr.xml["xml"][0];

        //读取牌间距
        multiplier.x = float.Parse(xml["multiplier"][0].att("x"));
        multiplier.y = float.Parse(xml["multiplier"][0].att("y"));

        PT_XMLHashList slots = xml["slot"];

        for (int i = 0; i < slots.Count; i++) 
        {
            PT_XMLHashtable slot = slots[i];
            SlotDef slotDef = new SlotDef();

            if (slot.HasAtt("type"))
            {
                slotDef.type = slot.att("type");
            }
            else 
            {
                slotDef.type = "slot";
            }
            //解析slot
            //slotDef.slotID = int.Parse(slot.att("id"));
            slotDef.x = float.Parse(slot.att("x"));
            slotDef.y = float.Parse(slot.att("y"));
            //slotDef.faceUp = slot.att("faceup") == "1";
            slotDef.layerID = int.Parse(slot.att("layer"));
            slotDef.layerName = LayerIDName[slotDef.layerID];

            switch (slotDef.type) 
            {
                case "slot":
                    slotDef.slotID = int.Parse(slot.att("id"));
                    slotDef.faceUp = slot.att("faceup") == "1";
                    if (slot.HasAtt("hiddenby")) 
                    {
                        string[] hBy = slot.att("hiddenby").Split(',');
                        foreach (string s in hBy)
                        {
                            int hidden = int.Parse(s);
                            slotDef.hiddenBy.Add(hidden);
                        }
                    }                    
                    SlotDefs.Add(slotDef);
                    break;
                case "drawpile":
                    slotDef.xStagger.x = float.Parse(slot.att("xstagger"));
                    SlotDefs.Add(slotDef);
                    break;
                case "discardpile":
                    SlotDefs.Add(slotDef);
                    break;
            }
        }
    }

   
    
}
