using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    [Header("Control Parameters")]
    public float horizontalMoveRate = 0.2f;
    public float placeTetriminoDelay = 0.25f;
    public float fallTime = 2f;
    public float fastFallTimeMultiplier = 15f;

    [Header("Point Display Parameters")]
    public float points = 0;
    [SerializeField] private TextMeshProUGUI pointsText;

    [Header("Reward System")]
    public int pointsPerLine = 100;
    public float tetrisMultiplier = 1.5f;
    public float consecutiveClearMultiplier = 1.25f;
    private int consecutiveClears = 0;

    [Header("Speed up System")]
    public float speedUpPointRequirement = 1000f;
    public float speedUpMultiplier = 0.9f;
    private float speedUpCounter = 0;

    // Controls params

    public float GetHorizontalMoveRate()
    {
        return horizontalMoveRate;
    }

    public float GetFallTime()
    {
        return fallTime;
    }

    public float GetFastFallTimeMultiplier()
    {
        return fastFallTimeMultiplier;
    }

    public float GetPlaceTetriminoDelay()
    {
        return placeTetriminoDelay;
    }

    // Pausing

    [Header("Pause Parameters")]
    
    public GameObject pauseMenuPanel;
    public GameObject inGamePauseButton;

    private GameObject currentControlledTetrimino;
    private bool paused = false;

    // NEW INPUT SYSTEM STUFF
    public TetriminoControls tetriminoControls;
    private InputAction pause;

    private void Awake()
    {
        tetriminoControls = new TetriminoControls();
    }
    private void OnEnable()
    {
        pause = tetriminoControls.Player.Pause;
        pause.Enable();
        pause.performed += PauseButton;
    }

    private void OnDisable()
    {
        pause.Disable();
    }
    private void PauseButton(InputAction.CallbackContext context)
    {
        PauseGame();
    }
    // END

    public void PauseGame()
    {
        TogglePauseMenu();
        FindObjectOfType<AudioManager>().toggleAudioFocus();
    }
    
    public void SetNewCCTetrimino(GameObject tetrimino)
    {
        currentControlledTetrimino = tetrimino;
    }

    private void TogglePauseMenu()
    {
        if (paused == false)
        {
            paused = true;
            currentControlledTetrimino.GetComponent<controls>().IsDummy(true);
            inGamePauseButton.SetActive(false);
        }
        else
        {
            paused = false;
            currentControlledTetrimino.GetComponent<controls>().IsDummy(false);
            inGamePauseButton.SetActive(true);
        }
        pauseMenuPanel.SetActive(paused);
    }

    // Point System

    public void AddPoints(int linesCleared)
    {
        int scoreToAdd = 0;
        if(linesCleared <= 3)
        {
            scoreToAdd += (int)(pointsPerLine * linesCleared);
        }
        else
        {
            scoreToAdd += (int)(pointsPerLine * linesCleared * tetrisMultiplier);
        }
        if(consecutiveClears > 0)
        {
            for(int i = 0; i < consecutiveClears; i++)
            {
                scoreToAdd = (int)(scoreToAdd * consecutiveClearMultiplier);
            }
        }
        points += scoreToAdd;
        speedUpCounter += scoreToAdd;
        consecutiveClears++;
        SpeedUp();
        UpdatePointsText();
    }

    private void SpeedUp()
    {
        if(speedUpCounter >= speedUpPointRequirement)
        {
            speedUpCounter -= speedUpPointRequirement;
            fallTime *= speedUpMultiplier;
        }
    }

    private void UpdatePointsText()
    {
        pointsText.text = "Points:\n" + points;
    }

    public void ResetConsecutiveClearCount()
    {
        consecutiveClears = 0;
    }

    // Swap Behaviour

    private bool swapped = false;

    public bool SwapCheck()
    {
        return swapped;
    }

    public void ToggleHasSwapped(bool toggle)
    {
        swapped = toggle;
    }

    // Game Over Stuff

    [Header("Game Over Parameters")]
    public GameObject gameOverPanel;
    public string MainMenuName;

    public void GameOver()
    {
        Debug.Log("GAME OVER!");
        gameOverPanel.SetActive(true);
        StopMusic();
    }

    private void StopMusic()
    {
        FindObjectOfType<AudioManager>().StopMusic();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(MainMenuName);
    }
}
