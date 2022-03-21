using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public bool ___________________;

    public XMLReader xmlr;

    public void InitDeck(string deckXMLText) 
    {
        ReadDeck(deckXMLText);
    }

    public void ReadDeck(string deckXMLText) 
    {
        xmlr = new XMLReader();
        xmlr.Parse(deckXMLText);

    }
}
