using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Card lastFlippedCard = null;
    public CardSpawner cardSpawner;
    public UIController uiController;
    public AudioManager audioManager;
    public float matchDelay = 1f;


    [ContextMenu("Initialize Game")]
    public void StartNewGame(int width, int height)
    {
        cardSpawner.GenerateBoard(width, height, OnCardClicked);
        uiController.Init(width, height);
    }

    [ContextMenu("Load Game")]
    public void LoadGame(GameSaveData data)
    {
        cardSpawner.LoadFromSave(data, OnCardClicked);
        uiController.LoadUI(data);
    }

    public GameSaveData GetCurrentGameState()
    {
        return new GameSaveData
        {
            gridWidth = cardSpawner.GridWidth,
            gridHeight = cardSpawner.GridHeight,
            cardStates = cardSpawner.GetCardStates(),
            score = uiController.Score,
            moves = uiController.Moves,
            matchedPairs = uiController.MatchedPairs,
            comboMultiplier = uiController.ComboMultiplier
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
            uiController.RegisterMove();

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
            uiController.RegisterMatch();
            audioManager.PlayCardMatchSound();

            if (uiController.IsGameComplete())
            {
                yield return new WaitForSeconds(1f);
                audioManager.PlayGameCompleteSound();
                uiController.ShowGameCompletePanel();
            }
        }
        else
        {
            uiController.RegisterMismatch();
            audioManager.PlayCardMismatchSound();

            card1.FlipCard();
            card2.FlipCard();
        }
    }
}