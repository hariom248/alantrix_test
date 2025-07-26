using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int gridWidth = 4;
    public int gridHeight = 4;
    
    [Header("Card and Sprites Setup")]
    public GameObject cardPrefab;
    public GameObject cardParent;
    public Sprite[] cardSprites;
    
    private List<Card> allCards = new List<Card>();
    private List<Card> flippedCards = new List<Card>();
    
    private int totalPairs;
    private int matchedPairs;

    void Start()
    {
        InitializeGame();
    }
    
    void InitializeGame()
    {
        totalPairs = gridWidth * gridHeight / 2;
        CreateCardGrid();
    }

    void CreateCardGrid()
    {
        RectTransform parentRect = cardParent.GetComponent<RectTransform>();

        float spacing = 10f;

        // Get current available size
        float availableWidth = parentRect.rect.width;
        float availableHeight = parentRect.rect.height;

        // Compute max square card size that fits
        float maxCardWidth = (availableWidth - spacing * (gridWidth - 1)) / gridWidth;
        float maxCardHeight = (availableHeight - spacing * (gridHeight - 1)) / gridHeight;
        float cardSize = Mathf.Min(maxCardWidth, maxCardHeight); // Square cards

        // Resize the parent to fit just the grid
        float targetWidth = cardSize * gridWidth + spacing * (gridWidth - 1);
        float targetHeight = cardSize * gridHeight + spacing * (gridHeight - 1);
        parentRect.sizeDelta = new Vector2(targetWidth, targetHeight);

        // Centering offsets
        float startX = -targetWidth / 2f + cardSize / 2f;
        float startY = targetHeight / 2f - cardSize / 2f;

        int cardIndex = 0;
        allCards.Clear();
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject cardObj = Instantiate(cardPrefab, cardParent.transform);
                RectTransform cardRect = cardObj.GetComponent<RectTransform>();

                cardRect.sizeDelta = new Vector2(cardSize, cardSize);

                cardRect.anchoredPosition = new Vector2(
                    startX + x * (cardSize + spacing),
                    startY - y * (cardSize + spacing)
                );

                Card card = cardObj.GetComponent<Card>();
                card.cardButton.onClick.AddListener(() => OnCardClicked(card));
                card.cardFront = cardSprites[cardIndex / 2];  
                card.cardId = cardIndex / 2;
                allCards.Add(card);

                cardIndex++;
            }
        }
    }
    
    public void OnCardClicked(Card card)
    {
        if (!card.CanClick()) return;
        // Add card to flipped list
        card.FlipCard();
        flippedCards.Add(card);
        
        // Check if we have two cards flipped
        if (flippedCards.Count == 2)
        {
            StartCoroutine(CheckForMatch());
        }
    }
    
    private IEnumerator CheckForMatch()
    {
        // Wait a moment for player to see both cards
        yield return new WaitForSeconds(1f);
        
        Card card1 = flippedCards[0];
        Card card2 = flippedCards[1];
        
        if (card1.GetCardId() == card2.GetCardId())
        {
            // Match found!
            card1.SetMatched();
            card2.SetMatched();
            matchedPairs++;

            // Check if game is complete
            if (matchedPairs >= totalPairs)
            {
                yield return new WaitForSeconds(1f);
                Debug.Log("Game Complete");
            }
        }
        else
        {
            // No match, flip cards back
            card1.FlipCard();
            card2.FlipCard();
            
        }
        
        flippedCards.Clear();
    }
} 