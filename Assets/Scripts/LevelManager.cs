using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
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
        pointsText.text = "Points: " + points;
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

    public void GameOver()
    {
        Debug.Log("GAME OVER!");
    }
}
