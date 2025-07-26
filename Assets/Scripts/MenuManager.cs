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


    [Header("Game Over UI")]
    public Text finalScoreText;
    public Text finalMovesText;
    public Button playAgainButton;
    public Button playAgainLoadGameButton;

    public AudioManager audioManager;
    public GameDataManager gameDataManager;
    public GameManager gameManager;
    private GameSaveData currentSaveData;

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
        playAgainButton.onClick.AddListener(() => { PlayButtonSound(); StartGame(); });
        playAgainLoadGameButton.onClick.AddListener(() => { PlayButtonSound(); LoadGame(); });

    }

    public void UpdateUI()
    {
        gameDataManager.TryLoad(out currentSaveData);
        playLoadGameButton.interactable = currentSaveData != null;
        playAgainLoadGameButton.interactable = currentSaveData != null;
        backgroundImage.enabled = true;
    }

    void StartGame()
    {
        HideAllPanels();
        gameManager.StartNewGame();
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