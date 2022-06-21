using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Point Display Parameters")]
    public float points = 0;
    [SerializeField] private TextMeshProUGUI pointsText;

    [Header("Reward System")]
    public int pointsPerLine = 100;
    public float tetrisMultiplier = 1.5f;
    public float consecutiveClearMultiplier = 1.25f;

    private int consecutiveClears = 0;

    public void addPoints(int linesCleared)
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
        consecutiveClears++;
        updatePointsText();
    }

    private void updatePointsText()
    {
        pointsText.text = "Points:\n" + points;
    }

    public void resetConsecutiveClearCount()
    {
        consecutiveClears = 0;
    }

    // Swap Behaviour

    private bool swapped = false;

    public bool swapCheck()
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
