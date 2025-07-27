
using UnityEngine;

public class GameSetupManager : MonoBehaviour
{
    [Header("Manager References")]
    public CardLayoutManager cardLayoutManager;
    public GameProgressManager gameProgressManager;
    public GameLogicManager gameLogicManager;
    private GridSize gridSize;

    public void StartNewGame(GridSize gridSize)
    {
        this.gridSize = gridSize;
        cardLayoutManager.GenerateBoard(gridSize, gameLogicManager.OnCardClicked);
        gameProgressManager.Initialize(GetTotalPairs());
    }

    public void LoadGame(GameSaveData data)
    {
        this.gridSize = data.gridSize;
        cardLayoutManager.LoadFromSave(data, gameLogicManager.OnCardClicked);
        gameProgressManager.LoadUI(data, GetTotalPairs());
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
            score = gameProgressManager.Score,
            moves = gameProgressManager.Moves,
            matchedPairs = gameProgressManager.MatchedPairs,
            comboMultiplier = gameProgressManager.ComboMultiplier
        };
    }
}