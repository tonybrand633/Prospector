using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prospector : MonoBehaviour
{
    public Prospector S;
    public Deck deck;
    public TextAsset xmlText;
    // Start is called before the first frame update

    void Awake()
    {
        S = this;    
    }

    void Start()
    {
        deck = GetComponent<Deck>();
        deck.InitDeck(xmlText.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
