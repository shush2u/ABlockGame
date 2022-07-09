using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Control Parameters")]
    [SerializeField] private float horizontalMoveRate = 0.2f;
    [SerializeField] private float placeTetriminoDelay = 0.25f;
    [SerializeField] private float fallTime = 2f;
    [SerializeField] private float fastFallTimeMultiplier = 15f;
    [SerializeField] private GameObject spawnPoint;

    [Header("Point Display Parameters")]
    [SerializeField] private float points = 0;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI lineClearText;
    [SerializeField] private GameObject lineClearObject;
    private bool lineClearTextActive = false;

    [Header("Reward System")]
    [SerializeField] private int pointsPerLine = 100;
    [SerializeField] private float tetrisMultiplier = 1.5f;
    [SerializeField] private float consecutiveClearMultiplier = 1.25f;
    private int consecutiveClearCount = 0;

    [Header("Speed up System")]
    [SerializeField] private float speedUpPointRequirement = 1000f;
    [SerializeField] private float speedUpMultiplier = 0.9f;
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

    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject inGamePauseButton;

    private GameObject currentControlledPiece;
    private bool paused = false;

    // NEW INPUT SYSTEM STUFF
    public TetriminoControls tetriminoControls;
    private InputAction pause;
    //

    [SerializeField] private GameObject staticOptionsObject;
    private StaticOptions staticOptions;

    private void Start()
    {
        staticOptions = staticOptionsObject.GetComponent<StaticOptions>();
        countdownEnabled = staticOptions.IsCountdownEnabled();
        audioManager = audioManagerObject.GetComponent<AudioManager>();

        BeginCountdown();
        countdownToggleButton.isOn = countdownEnabled;
    }


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

    [Header("Countdown Params")]

    [SerializeField] private GameObject CountdownAnimator;
    [SerializeField] private GameObject CountdownCanvas;

    [SerializeField] private Toggle countdownToggleButton;

    private bool countdownEnabled;

    private void FixedUpdate()
    {
        if(CountdownCanvas.activeSelf == true && countdownEnabled)
        {
            if(CountdownAnimator.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                EndOfCountdown();
            }
        }
        if (lineClearTextActive == true)
        {
            if (lineClearObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                lineClearTextActive = false;
                RemoveLineClearText();
            }
        }
    }

    private void EndOfCountdown()
    {
        if (currentControlledPiece != null)
        {
            currentControlledPiece.GetComponent<Controls>().IsDummy(false);
        }
        else
        {
            spawnPoint.GetComponent<SpawnTetrimino>().Unpause();
        }
        CountdownCanvas.SetActive(false);
    }

    private void BeginCountdown()
    {
        if(countdownEnabled)
        {
            if(currentControlledPiece != null)
            {
                currentControlledPiece.GetComponent<Controls>().IsDummy(true);
            }
            else
            {
                spawnPoint.GetComponent<SpawnTetrimino>().Pause();
            }
        
            CountdownCanvas.SetActive(true);
        }
        else
        {
            spawnPoint.GetComponent<SpawnTetrimino>().Unpause();
            if (currentControlledPiece != null)
                currentControlledPiece.GetComponent<Controls>().IsDummy(false);
        }
    }

    public void ToggleCountdown(bool toggle)
    {
        countdownEnabled = toggle;
        staticOptions.ToggleCountdown(toggle);
    }

    private void PauseButton(InputAction.CallbackContext context)
    {
        PauseGame();
    }

    public void PauseGame()
    {
        TogglePauseMenu();
        audioManager.toggleAudioFocus();
    }
    
    public void SetNewCCTetrimino(GameObject tetrimino)
    {
        currentControlledPiece = tetrimino;
    }

    private void TogglePauseMenu()
    {
        CountdownCanvas.SetActive(false);
        if (paused == false)
        {
            paused = true;
            if(currentControlledPiece != null)
            {
                currentControlledPiece.GetComponent<Controls>().IsDummy(true);
            }
            inGamePauseButton.SetActive(false);
        }
        else
        {
            paused = false;
            BeginCountdown();
            inGamePauseButton.SetActive(true);
        }
        pauseMenuPanel.SetActive(paused);
    }

    // Point System

    public void AddPoints(int linesCleared)
    {
        int scoreToAdd = 0;
        if (linesCleared <= 3)
        {
            scoreToAdd += (int)(pointsPerLine * linesCleared);
        }
        else
        {
            scoreToAdd += (int)(pointsPerLine * linesCleared * tetrisMultiplier);
        }
        if(consecutiveClearCount > 0)
        {
            for(int i = 0; i < consecutiveClearCount; i++)
            {
                scoreToAdd = (int)(scoreToAdd * consecutiveClearMultiplier);
            }
        }
        points += scoreToAdd;
        speedUpCounter += scoreToAdd;
        consecutiveClearCount++;
        SpeedUp();
        UpdatePointsText();
        DisplayLineClearText(linesCleared, consecutiveClearCount, scoreToAdd);
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

    private void DisplayLineClearText(int linesCleared, int consecutiveClears, int scoreToAdd)
    {
        lineClearTextActive = true;
        lineClearText.text = "";
        if (consecutiveClears > 1)
        {
            lineClearText.text += consecutiveClears + " In a Row!\n";
        }
        if (linesCleared == 1)
        {
            lineClearText.text += linesCleared + " line cleared!\n";
        }
        else
        {
            lineClearText.text += linesCleared + " lines cleared!\n";
        }
        lineClearText.text += "+" + scoreToAdd + " points";
        lineClearObject.SetActive(false);
        lineClearObject.SetActive(true);
    }
    private void RemoveLineClearText()
    {
        lineClearObject.SetActive(false);
    }

    public void ResetConsecutiveClearCount()
    {
        consecutiveClearCount = 0;
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
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private string MainMenuName;

    [Header("Audio Manager")]
    [SerializeField] private GameObject audioManagerObject;
    private AudioManager audioManager;

    public void GameOver()
    {
        Debug.Log("GAME OVER!");
        gameOverPanel.SetActive(true);
        highScoreText.text = "Total Score:\n" + points;
        StopMusic();
    }

    private void StopMusic()
    {
        audioManager.StopMusic();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMainMenu()
    {
        audioManager.ResetAudioFocus();
        SceneManager.LoadScene(MainMenuName);
    }
}
