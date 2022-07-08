using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Start Game Params")]
    public string startLevelName;

    public void StartGame()
    {
        SceneManager.LoadScene(startLevelName);
    }


    [Header("Menu Tabs")]
    public GameObject mainScreen;
    public GameObject settingsScreen;
    public GameObject creditsScreen;
    public GameObject quitConfirm;

    private GameObject currentScreen;

    [Header("Settings Sync")]
    public Toggle countdownToggle;
    public StaticOptions staticOptions;

    private void Start()
    {
        staticOptions = FindObjectOfType<StaticOptions>();
        countdownToggle.isOn = staticOptions.IsCountdownEnabled();
    }

    public void OpenSettings()
    {
        mainScreen.SetActive(false);
        settingsScreen.SetActive(true);
        currentScreen = settingsScreen;
    }

    public void OpenCredits()
    {
        mainScreen.SetActive(false);
        creditsScreen.SetActive(true);
        currentScreen = creditsScreen;
    }

    public void Back()
    {
        currentScreen.SetActive(false);
        mainScreen.SetActive(true);
        currentScreen = mainScreen;
    }

    public void AttemptQuit()
    {
        quitConfirm.SetActive(true);
    }

    public void ConfirmQuit()
    {
        Application.Quit();
    }

    public void CancelQuit()
    {
        quitConfirm.SetActive(false);
    }

    [Header("Display Tetrimino Params")]
    public GameObject displayPosition;
    private GameObject currentDisplayTetrimino;
    public float timeToFade = 1.0f;

    bool fadeIn = true;
    bool fadeOut = false;
    bool interval = false;

    public void Update()
    {
        if(currentDisplayTetrimino != null)
        {
            if (fadeIn == true)
            {
                FadeIn();
            }
            else
            {
                if(interval == true)
                {
                    Invoke("Interval", 8);
                    interval = false;
                }
                if(fadeOut == true)
                {
                    FadeOut();
                }
            }
            
        }
        else
        {
            CreateNewDisplayTetrimino();
        }
    }

    public GameObject[] displayTetriminos;
    private GameObject previousTetrimino;

    private void CreateNewDisplayTetrimino()
    {
        GameObject nextDisplayTetrimino = pickNewDisplayTetrimino();
        currentDisplayTetrimino = (GameObject)Instantiate(nextDisplayTetrimino, displayPosition.transform.position, displayPosition.transform.rotation);
        currentDisplayTetrimino.GetComponent<controls>().IsDummy(true);

        foreach (Transform children in currentDisplayTetrimino.transform)
        {
            Color oldColor = children.GetComponent<Renderer>().material.color;
            Color color = new Color(oldColor.r, oldColor.g, oldColor.b, 0f);
            children.GetComponent<Renderer>().material.color = color;
        }
    }

    private GameObject pickNewDisplayTetrimino()
    {
        GameObject nextTetrimino = displayTetriminos[Random.Range(0, displayTetriminos.Length)];
        while(previousTetrimino == nextTetrimino)
        {
            nextTetrimino = displayTetriminos[Random.Range(0, displayTetriminos.Length)];
        }
        return nextTetrimino;
    }

    private void FadeIn()
    {
        foreach (Transform children in currentDisplayTetrimino.transform)
        {
            Color newColor = children.GetComponent<Renderer>().material.color;
            if (newColor.a < 1)
            {
                newColor.a += Time.deltaTime / timeToFade;
                children.GetComponent<Renderer>().material.color = newColor;
            }
            else
            {
                newColor.a = 1;
                fadeIn = false;
                interval = true;
            }
        }
    }

    private void Interval()
    {
        if(fadeOut == false)
        {
            fadeOut = true;
        }
    }

    private void FadeOut()
    {
        foreach (Transform children in currentDisplayTetrimino.transform)
        {
            Color newColor = children.GetComponent<Renderer>().material.color;
            if (newColor.a > 0)
            {
                newColor.a -= Time.deltaTime / timeToFade;
                children.GetComponent<Renderer>().material.color = newColor;
            }
            else
            {
                newColor.a = 0;
                fadeIn = true;
                fadeOut = false;
                Destroy(currentDisplayTetrimino);
            }
        }
    }
}
