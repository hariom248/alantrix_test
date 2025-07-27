using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GridSize
{
    public int width;
    public int height;

    public GridSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
}

public class CardLayoutManager : MonoBehaviour
{
    [Header("Layout Settings")]
    public RectTransform cardParent;
    public float spacing = 20f;
    public float margin = 20f;

    [Header("Manager References")]
    public CardGenerator cardGenerator;

    private int availableWidth;
    private int availableHeight;

    private List<Card> cards = new List<Card>();

    private void Start()
    {
        InitializeCardParent();
    }

    private void InitializeCardParent()
    {
        RectTransform parentRect = cardParent.GetComponent<RectTransform>();

        availableWidth = (int)parentRect.rect.width;
        availableHeight = (int)parentRect.rect.height;

        parentRect.anchorMin = new Vector2(0.5f, 0.5f);
        parentRect.anchorMax = new Vector2(0.5f, 0.5f);
        parentRect.pivot = new Vector2(0.5f, 0.5f);

        parentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, availableWidth);
        parentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, availableHeight);
    }

    public void GenerateBoard(GridSize gridSize, Action<Card> OnCardClick)
    {
        GenerateBoardInternal(gridSize, availableWidth, availableHeight, OnCardClick);
    }

    public void LoadFromSave(GameSaveData saveData, Action<Card> OnCardClick)
    {
        RectTransform parentRect = cardParent.GetComponent<RectTransform>();
        GenerateBoardInternal(
            saveData.gridSize,
            parentRect.rect.width,
            parentRect.rect.height,
            OnCardClick,
            saveData.cardStates
        );
    }

    private void GenerateBoardInternal(
        GridSize gridSize,
        float availableWidth,
        float availableHeight,
        Action<Card> OnCardClick,
        List<CardState> savedStates = null
    )
    {
        ClearBoard();

        int totalCards = gridSize.width * gridSize.height;

        RectTransform parentRect = cardParent.GetComponent<RectTransform>();

        float cardSize = Mathf.Min(
            (availableWidth - spacing * (gridSize.width - 1)) / gridSize.width,
            (availableHeight - spacing * (gridSize.height - 1)) / gridSize.height
        );

        float margin = 40f;
        float targetWidth = cardSize * gridSize.width + spacing * (gridSize.width - 1) + margin;
        float targetHeight = cardSize * gridSize.height + spacing * (gridSize.height - 1) + margin;
        parentRect.sizeDelta = new Vector2(targetWidth, targetHeight);

        float startX = -targetWidth / 2f + margin / 2f + cardSize / 2f;
        float startY = targetHeight / 2f - margin / 2f - cardSize / 2f;

        // Generate cards using CardGenerator
        cards = cardGenerator.GenerateCards(totalCards, OnCardClick, savedStates);

        // Position cards in grid
        for (int i = 0; i < totalCards; i++)
        {
            int x = i % gridSize.width;
            int y = i / gridSize.width;

            RectTransform cardRect = cards[i].GetComponent<RectTransform>();
            cardRect.SetParent(cardParent, false);
            cardRect.sizeDelta = new Vector2(cardSize, cardSize);
            cardRect.anchoredPosition = new Vector2(
                startX + x * (cardSize + spacing),
                startY - y * (cardSize + spacing)
            );
        }
    }

    private void ClearBoard()
    {
        foreach (Transform child in cardParent)
            Destroy(child.gameObject);

        cards.Clear();
    }

    public List<CardState> GetCardStates()
    {
        var states = new List<CardState>();
        foreach (var card in cards)
        {
            states.Add(new CardState
            {
                cardId = card.CardId,
                cardVisualState = card.VisualState
            });
        }
        return states;
    }
}
