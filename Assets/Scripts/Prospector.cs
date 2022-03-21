using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prospector : MonoBehaviour
{
    public static Prospector singlton;
    public Deck deck;
    public TextAsset deckXML;

    void Awake()
    {
        singlton = this;    
    }

    // Start is called before the first frame update
    void Start()
    {
        deck = this.GetComponent<Deck>();
        deck.InitDeck(deckXML.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
