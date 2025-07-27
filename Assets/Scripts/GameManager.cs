using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Card lastFlippedCard = null;
    public CardLayoutManager cardLayoutManager;
    public GameUIManager gameUIManager;
    public AudioManager audioManager;
    public float matchDelay = 1f;


    public void StartNewGame(int width, int height)
    {
        cardLayoutManager.GenerateBoard(width, height, OnCardClicked);
        gameUIManager.Init(width, height);
    }

    public void LoadGame(GameSaveData data)
    {
        cardLayoutManager.LoadFromSave(data, OnCardClicked);
        gameUIManager.LoadUI(data);
    }

    public GameSaveData GetCurrentGameState()
    {
        return new GameSaveData
        {
            gridWidth = cardLayoutManager.GridWidth,
            gridHeight = cardLayoutManager.GridHeight,
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