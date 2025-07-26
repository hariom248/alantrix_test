// CardSpawner.cs
using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class CardSpawner : MonoBehaviour
{
    [Header("Card Settings")]
    public GameObject cardPrefab;
    public RectTransform cardParent;
    public float spacing = 20f;
    public Sprite[] cardSprites;

    private int gridWidth;
    private int gridHeight;
    private List<Card> allCards = new List<Card>();

    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;

    public void GenerateBoard(int width, int height, Action<Card> OnCardClick)
    {
        ClearBoard();

        gridWidth = width;
        gridHeight = height;
        int totalCards = gridWidth * gridHeight;

        RectTransform parentRect = cardParent.GetComponent<RectTransform>();

        // Get current available size
        float availableWidth = parentRect.rect.width;
        float availableHeight = parentRect.rect.height;

        // Compute max square card size that fits
        float cardSize = Mathf.Min(
            (availableWidth - spacing * (gridWidth - 1)) / gridWidth,
            (availableHeight - spacing * (gridHeight - 1)) / gridHeight
        );

        // Resize the parent to fit just the grid
        float targetWidth = cardSize * gridWidth + spacing * (gridWidth - 1);
        float targetHeight = cardSize * gridHeight + spacing * (gridHeight - 1);
        parentRect.sizeDelta = new Vector2(targetWidth, targetHeight);

        // Centering offsets
        float startX = -targetWidth / 2f + cardSize / 2f;
        float startY = targetHeight / 2f - cardSize / 2f;

        List<int> ids = GenerateShuffledIds(totalCards);

        for (int y = 0, index = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++, index++)
            {
                GameObject obj = Instantiate(cardPrefab, cardParent);
                RectTransform cardRect = obj.GetComponent<RectTransform>();

                cardRect.sizeDelta = new Vector2(cardSize, cardSize);
                cardRect.anchoredPosition = new Vector2(
                    startX + x * (cardSize + spacing),
                    startY - y * (cardSize + spacing)
                );

                Card card = obj.GetComponent<Card>();
                card.cardId = ids[index];
                card.cardIndex = index;
                card.cardButton.onClick.AddListener(() => OnCardClick(card));
                if (cardSprites != null && cardSprites.Length > ids[index])
                {
                    card.cardFront = cardSprites[ids[index]];
                }
                allCards.Add(card);
            }
        }
    }

    void ClearBoard()
    {
        foreach (Transform child in cardParent)
            Destroy(child.gameObject);

        allCards.Clear();
    }

    List<int> GenerateShuffledIds(int total)
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < total / 2; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }
        for (int i = 0; i < ids.Count; i++)
        {
            int j = Random.Range(0, ids.Count);
            (ids[i], ids[j]) = (ids[j], ids[i]);
        }
        return ids;
    }

    public void LoadFromSave(GameSaveData saveData, Action<Card> OnCardClick)
    {
        ClearBoard();

        gridWidth = saveData.gridWidth;
        gridHeight = saveData.gridHeight;
        int totalCards = gridWidth * gridHeight;

        RectTransform parentRect = cardParent.GetComponent<RectTransform>();

        float availableWidth = parentRect.rect.width;
        float availableHeight = parentRect.rect.height;

        float cardSize = Mathf.Min(
            (availableWidth - spacing * (gridWidth - 1)) / gridWidth,
            (availableHeight - spacing * (gridHeight - 1)) / gridHeight
        );

        float targetWidth = cardSize * gridWidth + spacing * (gridWidth - 1);
        float targetHeight = cardSize * gridHeight + spacing * (gridHeight - 1);
        parentRect.sizeDelta = new Vector2(targetWidth, targetHeight);

        float startX = -targetWidth / 2f + cardSize / 2f;
        float startY = targetHeight / 2f - cardSize / 2f;

        for (int i = 0; i < saveData.cardStates.Count; i++)
        {
            int x = i % gridWidth;
            int y = i / gridWidth;

            GameObject obj = Instantiate(cardPrefab, cardParent);
            RectTransform cardRect = obj.GetComponent<RectTransform>();

            cardRect.sizeDelta = new Vector2(cardSize, cardSize);
            cardRect.anchoredPosition = new Vector2(
                startX + x * (cardSize + spacing),
                startY - y * (cardSize + spacing)
            );

            CardState state = saveData.cardStates[i];
            Card card = obj.GetComponent<Card>();
            card.cardId = state.cardId;
            card.cardButton.onClick.AddListener(() => OnCardClick(card));
            card.cardIndex = i;
            if (cardSprites != null && cardSprites.Length > state.cardId)
            {
                card.cardFront = cardSprites[state.cardId];
            }
            allCards.Add(card);
            if (state.isFlipped)
            {
                card.FlipCard();
            }
            if(state.isMatched)
            {
                card.SetMatched();
            }
        }
    }

    public List<CardState> GetCardStates()
    {
        var states = new List<CardState>();
        foreach (var card in allCards)
        {
            states.Add(new CardState
            {
                cardId = card.cardId,
                isFlipped = card.IsFlipped(),
                isMatched = card.IsMatched(),
            });
        }
        return states;
    }
}
