using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{
    [Header("Card Settings")]
    public GameObject cardPrefab;
    public Sprite[] cardSprites;
    
    public List<Card> GenerateCards(int totalCards, Action<Card> onCardClick, List<CardState> savedStates = null)
    {
        List<Card> cards = new List<Card>();
        List<int> ids = savedStates == null 
            ? GenerateShuffledIds(totalCards) 
            : Enumerable.Range(0, totalCards).ToList();

        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab);
            Card card = cardObj.GetComponent<Card>();
            
            int id = savedStates == null ? ids[i] : savedStates[i].cardId;
            card.CardId = id;
            
            if (cardSprites.Length > id)
            {
                card.cardFront = cardSprites[id];
            }
            
            if (savedStates != null && savedStates[i].cardVisualState == CardVisualState.Matched)
            {
                card.FlipAndMatch();
            }
            
            card.cardButton.onClick.AddListener(() => onCardClick(card));
            cards.Add(card);
        }
        
        return cards;
    }
    
    private List<int> GenerateShuffledIds(int total)
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < total / 2; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }
        
        // Fisher-Yates shuffle
        for (int i = 0; i < ids.Count; i++)
        {
            int j = UnityEngine.Random.Range(0, ids.Count);
            (ids[i], ids[j]) = (ids[j], ids[i]);
        }
        
        return ids;
    }
} 