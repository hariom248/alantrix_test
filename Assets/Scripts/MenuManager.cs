using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameOverPanel;
    public Image backgroundImage;

    [Header("Main Menu UI")]
    public Button playNewGameButton;
    public Button playLoadGameButton;
    public Button saveGameButton;
    public Slider gridWidthSlider;
    public Slider gridHeightSlider;
    public Text gridWidthText;
    public Text gridHeightText;


    [Header("Game Over UI")]
    public Text finalScoreText;
    public Text finalMovesText;
    public Button showMainMenuButton;

    public AudioManager audioManager;
    public GameDataManager gameDataManager;
    public GameManager gameManager;
    private GameSaveData currentSaveData;

    private int gridWidth = 4;
    private int gridHeight = 3;

    void Start()
    {
        gameDataManager = new GameDataManager();
        SetupUI();
        ShowMainMenu();
    }

    void SetupUI()
    {
        // Main Menu
        playNewGameButton.onClick.AddListener(() => { PlayButtonSound(); StartGame(); });
        playLoadGameButton.onClick.AddListener(() => { PlayButtonSound(); LoadGame(); });
        saveGameButton.onClick.AddListener(() => { PlayButtonSound(); SaveGame(); });

        // Game Over
        showMainMenuButton.onClick.AddListener(() => { PlayButtonSound(); ShowMainMenu(); });

        gridWidthSlider.onValueChanged.AddListener(value =>
        {
            gridWidth = Mathf.RoundToInt(value);
            gridWidthText.text = $"GridWidth : {gridWidth}";

            playNewGameButton.interactable = gridWidth > 0 && gridHeight > 0 && gridWidth * gridHeight % 2 == 0;
        });
        gridHeightSlider.onValueChanged.AddListener(value =>
        {
            gridHeight = Mathf.RoundToInt(value);
            gridHeightText.text = $"GridHeight : {gridHeight}";

            playNewGameButton.interactable = gridWidth > 0 && gridHeight > 0 && gridWidth * gridHeight % 2 == 0;
        });
        
        gridWidthSlider.value = gridWidth;
        gridHeightSlider.value = gridHeight;
    }

    public void UpdateUI()
    {
        gameDataManager.TryLoad(out currentSaveData);
        playLoadGameButton.interactable = currentSaveData != null;
        var currentGameState = gameManager.GetCurrentGameState();
        saveGameButton.interactable = currentGameState != null && currentGameState.gridWidth > 0 && currentGameState.gridHeight > 0 && !gameManager.uiController.IsGameComplete();
        backgroundImage.enabled = true;
    }

    private void StartGame()
    {
        HideAllPanels();
        gameManager.StartNewGame(gridWidth, gridHeight);
    }

    private void LoadGame()
    {
        HideAllPanels();
        gameManager.LoadGame(currentSaveData);
    }

    private void SaveGame()
    {
        HideAllPanels();
        gameDataManager.Save(gameManager.GetCurrentGameState());
    }

    private void PlayButtonSound()
    {
        audioManager.PlayButtonClickSound();
    }

    public void ShowMainMenu()
    {
        HideAllPanels();
        UpdateUI();
        mainMenuPanel.SetActive(true);
    }

    public void ShowGameOver(int finalScore, int finalMoves)
    {
        HideAllPanels();
        UpdateUI();
        gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + finalScore;
        if (finalMovesText != null)
            finalMovesText.text = "Moves: " + finalMoves;
    }

    void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        backgroundImage.enabled = false;
    }
}