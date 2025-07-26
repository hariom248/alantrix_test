using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int gridWidth = 4;
    public int gridHeight = 4;

    private Card lastFlippedCard = null;

    private GameDataManager dataManager;
    public CardSpawner cardSpawner;
    public UIController uiController;

    void Awake()
    {
         dataManager = new GameDataManager();
    }

    void Start()
    {
        if (dataManager.TryLoad(out GameSaveData data))
            LoadGame(data);
        else
            StartNewGame();
    }

    [ContextMenu("Initialize Game")]
    public void StartNewGame()
    {
        cardSpawner.GenerateBoard(gridWidth, gridHeight, OnCardClicked);
        uiController.Init(gridWidth, gridHeight);
    }

    [ContextMenu("Reset Data")]
    public void ResetData()
    {
        dataManager.ClearSave();
    }

    [ContextMenu("Load Game")]
    public void LoadGame(GameSaveData data)
    {
        cardSpawner.LoadFromSave(data, OnCardClicked);
        uiController.LoadUI(data);
    }

    [ContextMenu("Save Game")]
    public void SaveGame()
    {
        var saveData = new GameSaveData
        {
            gridWidth = cardSpawner.GridWidth,
            gridHeight = cardSpawner.GridHeight,
            cardStates = cardSpawner.GetCardStates(),
            score = uiController.Score,
            moves = uiController.Moves,
            matchedPairs = uiController.MatchedPairs,
            comboMultiplier = uiController.ComboMultiplier
        };

        dataManager.Save(saveData);
    }

    public void OnCardClicked(Card card)
    {
        if (!card.CanClick()) return;

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
        yield return new WaitForSeconds(1f);

        if (card1.GetCardId() == card2.GetCardId())
        {
            card1.SetMatched();
            card2.SetMatched();
            uiController.RegisterMatch();

            if (uiController.IsGameComplete())
            {
                yield return new WaitForSeconds(1f);
                Debug.Log("Game Complete");
            }
        }
        else
        {
            uiController.RegisterMismatch();

            card1.FlipCard();
            card2.FlipCard();
        }
    }
}