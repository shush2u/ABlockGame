using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Control Parameters")]
    public float horizontalMoveRate = 0.2f;
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

    // Point System

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
        speedUpCounter += scoreToAdd;
        consecutiveClears++;
        SpeedUp();
        updatePointsText();
    }

    private void SpeedUp()
    {
        if(speedUpCounter >= speedUpPointRequirement)
        {
            speedUpCounter -= speedUpPointRequirement;
            fallTime *= speedUpMultiplier;
        }
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
