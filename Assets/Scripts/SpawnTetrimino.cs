using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTetrimino : MonoBehaviour
{
    public GameObject[] tetriminoTypes;
    public GameObject NextTetrimino;

    private GameObject previousTetrimino;
    private int tetriminoRepeats = 0;

    private displayNextTetrimino nextTetriminoDisplay;

    // Start is called before the first frame update
    void Start()
    {
        nextTetriminoDisplay = FindObjectOfType<displayNextTetrimino>();
        pickNextTetrimino();
        previousTetrimino = NextTetrimino;
        spawnNewTetrimino();
    }

    public void spawnNewTetrimino() // public function to call when a new tetrimino is needed
    {
        spawnNextTetrimino(NextTetrimino);
    }

    public void spawnHeldTetrimino(GameObject tetrimino)
    {
        Instantiate(tetrimino, transform.position, Quaternion.identity);
    }

    private void updateNextTetriminoDisplay(GameObject NextTetrimino) // Updates UI to show next tetrimino
    {
        nextTetriminoDisplay.updateNextTetriminoDisplay(NextTetrimino);
    }

    private void pickNextTetrimino() // Picks new random tetrimino
    {
        NextTetrimino = tetriminoTypes[Random.Range(0, tetriminoTypes.Length)];

        if(NextTetrimino == previousTetrimino)
        {
            tetriminoRepeats++;
        }
        else
        {
            tetriminoRepeats = 0;
        }

        if(tetriminoRepeats >= 2)
        {
            NextTetrimino = tetriminoTypes[Random.Range(0, tetriminoTypes.Length)];
        }

        updateNextTetriminoDisplay(NextTetrimino); 
    }

    private void spawnNextTetrimino(GameObject tetrimino) // Spawns next tetrimino ingame, picks new next tetrimino.
    {
        Instantiate(tetrimino, transform.position, Quaternion.identity);
        pickNextTetrimino();
    }
}
