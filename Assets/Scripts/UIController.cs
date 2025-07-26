using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("UI References")]
    public Text scoreText;
    public Text movesText;
    public Text comboText;

    [Header("Score Settings")]
    public int baseMatchPoints = 100;
    public int comboIncrement = 50;

    public int Score { get; private set; }
    public int Moves { get; private set; }
    public int MatchedPairs { get; private set; }
    public int ComboMultiplier { get; private set; } = 0;


    public void ResetUI()
    {
        Score = 0;
        Moves = 0;
        MatchedPairs = 0;
        ComboMultiplier = 0;

        UpdateUI();
    }

    public void LoadUI(GameSaveData data)
    {
        Score = data.score;
        Moves = data.moves;
        MatchedPairs = data.matchedPairs;
        ComboMultiplier = data.comboMultiplier;

        UpdateUI();
    }

    public void RegisterMove()
    {
        Moves++;
        UpdateUI();
    }

    public void RegisterMatch()
    {
        MatchedPairs++;
        ComboMultiplier = Mathf.Max(1, ComboMultiplier + 1);

        int pointsGained = baseMatchPoints + (comboIncrement * (ComboMultiplier - 1));
        Score += pointsGained;

        UpdateUI();
    }

    public void RegisterMismatch()
    {
        ComboMultiplier = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = $"{Score}";
        movesText.text = $"{Moves}";
        comboText.text = $"x{ComboMultiplier}";
    }
}