using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardLayoutManager : MonoBehaviour
{
    [Header("Card Settings")]
    public GameObject cardPrefab;
    public Sprite[] cardSprites;
    
    [Header("Layout Settings")]
    public RectTransform cardParent;
    public float spacing = 20f;
    public float margin = 20f;
    
    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;
    
    private int gridWidth;
    private int gridHeight;
    private int availableWidth;
    private int availableHeight;
    
    public List<Card> AllCards { get; private set; } = new List<Card>();

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

    public void GenerateBoard(int width, int height, Action<Card> OnCardClick)
    {
        GenerateBoardInternal(width, height, availableWidth, availableHeight, OnCardClick);
    }

    public void LoadFromSave(GameSaveData saveData, Action<Card> OnCardClick)
    {
        RectTransform parentRect = cardParent.GetComponent<RectTransform>();
        GenerateBoardInternal(
            saveData.gridWidth,
            saveData.gridHeight,
            parentRect.rect.width,
            parentRect.rect.height,
            OnCardClick,
            saveData.cardStates
        );
    }

    private void GenerateBoardInternal(
        int width,
        int height,
        float availableWidth,
        float availableHeight,
        Action<Card> OnCardClick,
        List<CardState> savedStates = null
    )
    {
        ClearBoard();

        gridWidth = width;
        gridHeight = height;
        int totalCards = width * height;

        RectTransform parentRect = cardParent.GetComponent<RectTransform>();

        float cardSize = Mathf.Min(
            (availableWidth - spacing * (width - 1)) / width,
            (availableHeight - spacing * (height - 1)) / height
        );

        float margin = 40f;
        float targetWidth = cardSize * width + spacing * (width - 1) + margin;
        float targetHeight = cardSize * height + spacing * (height - 1) + margin;
        parentRect.sizeDelta = new Vector2(targetWidth, targetHeight);

        float startX = -targetWidth / 2f + margin / 2f + cardSize / 2f;
        float startY = targetHeight / 2f - margin / 2f - cardSize / 2f;

        List<int> ids = savedStates == null
            ? GenerateShuffledIds(totalCards)
            : Enumerable.Range(0, totalCards).ToList();

        for (int i = 0; i < totalCards; i++)
        {
            int x = i % width;
            int y = i / width;

            GameObject obj = Instantiate(cardPrefab, cardParent);
            RectTransform cardRect = obj.GetComponent<RectTransform>();

            cardRect.sizeDelta = new Vector2(cardSize, cardSize);
            cardRect.anchoredPosition = new Vector2(
                startX + x * (cardSize + spacing),
                startY - y * (cardSize + spacing)
            );

            Card card = obj.GetComponent<Card>();

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

            card.cardButton.onClick.AddListener(() => OnCardClick(card));
            AllCards.Add(card);
        }
    }

    private void ClearBoard()
    {
        foreach (Transform child in cardParent)
            Destroy(child.gameObject);

        AllCards.Clear();
    }

    private List<int> GenerateShuffledIds(int total)
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < total / 2; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }
        for (int i = 0; i < ids.Count; i++)
        {
            int j = UnityEngine.Random.Range(0, ids.Count);
            (ids[i], ids[j]) = (ids[j], ids[i]);
        }
        return ids;
    }

    public List<CardState> GetCardStates()
    {
        var states = new List<CardState>();
        foreach (var card in AllCards)
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
