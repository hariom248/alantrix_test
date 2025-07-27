using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float matchDelay = 1f;

    [Header("Manager References")]
    public CardLayoutManager cardLayoutManager;
    public GameUIManager gameUIManager;
    public AudioManager audioManager;

    private GridSize gridSize;
    private Card lastFlippedCard = null;


    public void StartNewGame(GridSize gridSize)
    {
        this.gridSize = gridSize;
        cardLayoutManager.GenerateBoard(gridSize, OnCardClicked);
        gameUIManager.Initialize(GetTotalPairs());
    }

    public void LoadGame(GameSaveData data)
    {
        this.gridSize = data.gridSize;
        cardLayoutManager.LoadFromSave(data, OnCardClicked);
        gameUIManager.LoadUI(data, GetTotalPairs());
    }

    private int GetTotalPairs()
    {
        return gridSize.width * gridSize.height / 2;
    }

    public GameSaveData GetCurrentGameState()
    {
        return new GameSaveData
        {
            gridSize = gridSize,
            cardStates = cardLayoutManager.GetCardStates(),
            score = gameUIManager.Score,
            moves = gameUIManager.Moves,
            matchedPairs = gameUIManager.MatchedPairs,
            comboMultiplier = gameUIManager.ComboMultiplier
        };
    }

    public void OnCardClicked(Card card)
    {
        if (!card.CanClick()) return;

        audioManager.PlayCardFlipSound();

        card.FlipCard();

        if (lastFlippedCard == null)
        {
            lastFlippedCard = card;
        }
        else
        {
            gameUIManager.RegisterMove();

            // Copy to local vars to avoid race conditions
            Card first = lastFlippedCard;
            Card second = card;
            lastFlippedCard = null;

            StartCoroutine(CheckPairMatch(first, second));
        }
    }

    private IEnumerator CheckPairMatch(Card card1, Card card2)
    {
        yield return new WaitForSeconds(card2.flipDuration + matchDelay);

        if (card1.CardId == card2.CardId)
        {
            card1.SetMatched();
            card2.SetMatched();
            gameUIManager.RegisterMatch();
            audioManager.PlayCardMatchSound();

            if (gameUIManager.IsGameComplete())
            {
                yield return new WaitForSeconds(1f);
                audioManager.PlayGameCompleteSound();
                gameUIManager.ShowGameCompletePanel();
            }
        }
        else
        {
            gameUIManager.RegisterMismatch();
            audioManager.PlayCardMismatchSound();

            card1.FlipCard();
            card2.FlipCard();
        }
    }
}