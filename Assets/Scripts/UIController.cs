using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("UI References")]
    public Text scoreText;
    public Text movesText;
    public Text comboText;
    public Button menuButton;
    public MenuManager menuManager;
    public AudioManager audioManager;

    [Header("Score Settings")]
    public int baseMatchPoints = 100;
    public int comboIncrement = 50;

    public int Score { get; private set; }
    public int Moves { get; private set; }
    public int MatchedPairs { get; private set; }
    public int ComboMultiplier { get; private set; } = 0;

    private int totalPairs;

    private void Start()
    {
        menuButton.onClick.AddListener(() =>
        {
            menuManager.ShowMainMenu();
            audioManager.PlayButtonClickSound();
        });
    }

    public void ShowGameCompletePanel()
    {
        menuManager.ShowGameOver(Score, Moves);
    }

    public void Init(int gridWidth, int gridHeight)
    {
        Score = 0;
        Moves = 0;
        MatchedPairs = 0;
        ComboMultiplier = 0;
        totalPairs = gridWidth * gridHeight / 2;

        UpdateUI();
    }

    public void LoadUI(GameSaveData data)
    {
        Score = data.score;
        Moves = data.moves;
        MatchedPairs = data.matchedPairs;
        ComboMultiplier = data.comboMultiplier;
        totalPairs = data.gridWidth * data.gridHeight / 2;

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

    public bool IsGameComplete()
    {
        return MatchedPairs == totalPairs;
    }

    private void UpdateUI()
    {
        scoreText.text = $"{Score}";
        movesText.text = $"{Moves}";
        comboText.text = $"x{ComboMultiplier}";
    }
}