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
    public Button resumeGameButton;
    public Slider gridWidthSlider;
    public Slider gridHeightSlider;
    public Text gridWidthText;
    public Text gridHeightText;

    [Header("Game Over UI")]
    public Text finalScoreText;
    public Text finalMovesText;
    public Button showMainMenuButton;

    [Header("Manager References")]
    public AudioManager audioManager;
    public GameDataManager gameDataManager;
    public GameSetupManager gameSetupManager;
    
    [Header("Grid Settings")]
    private GridSize gridSize = new GridSize(4, 3); // Default grid size
    
    private GameSaveData currentSaveData;

    private void Start()
    {
        gameDataManager = new GameDataManager();
        SetupUI();
        ShowMainMenu();
    }

    private void SetupUI()
    {
        // Main Menu
        playNewGameButton.onClick.AddListener(() => { PlayButtonSound(); StartGame(); });
        playLoadGameButton.onClick.AddListener(() => { PlayButtonSound(); LoadGame(); });
        saveGameButton.onClick.AddListener(() => { PlayButtonSound(); SaveGame(); });
        resumeGameButton.onClick.AddListener(() => { PlayButtonSound(); ResumeGame(); });

        // Game Over
        showMainMenuButton.onClick.AddListener(() => { PlayButtonSound(); ShowMainMenu(); });

        gridWidthSlider.onValueChanged.AddListener(value =>
        {
            gridSize.width = Mathf.RoundToInt(value);
            gridWidthText.text = $"GridWidth : {gridSize.width}";

            playNewGameButton.interactable = gridSize.width > 0 && gridSize.height > 0 && gridSize.width * gridSize.height % 2 == 0;
        });
        gridHeightSlider.onValueChanged.AddListener(value =>
        {
            gridSize.height = Mathf.RoundToInt(value);
            gridHeightText.text = $"GridHeight : {gridSize.height}";

            playNewGameButton.interactable = gridSize.width > 0 && gridSize.height > 0 && gridSize.width * gridSize.height % 2 == 0;
        });
        
        gridWidthSlider.value = gridSize.width;
        gridHeightSlider.value = gridSize.height;
    }

    private void ResumeGame()
    {
        HideAllPanels();
    }

    public void UpdateUI()
    {
        gameDataManager.TryLoad(out currentSaveData);
        playLoadGameButton.interactable = currentSaveData != null;
        var currentGameState = gameSetupManager.GetCurrentGameState();
        bool gameInProgress = currentGameState != null && currentGameState.gridSize.width > 0 &&  currentGameState.gridSize.height > 0;
        saveGameButton.interactable = gameInProgress;
        resumeGameButton.interactable = gameInProgress;
        backgroundImage.enabled = true;
    }

    private void StartGame()
    {
        HideAllPanels();
        gameSetupManager.StartNewGame(gridSize);
    }

    private void LoadGame()
    {
        HideAllPanels();
        gameSetupManager.LoadGame(currentSaveData);
    }

    private void SaveGame()
    {
        HideAllPanels();
        gameDataManager.Save(gameSetupManager.GetCurrentGameState());
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

    private void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        backgroundImage.enabled = false;
    }
}