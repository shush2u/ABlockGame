using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTetrimino : MonoBehaviour
{
    public GameObject[] tetriminoTypes;
    public GameObject NextTetrimino;

    private GameObject previousTetrimino;
    private int tetriminoRepeats = 0;

    private DisplayNextTetrimino nextTetriminoDisplay;

    // Start is called before the first frame update
    void Start()
    {
        nextTetriminoDisplay = FindObjectOfType<DisplayNextTetrimino>();
        PickNextTetrimino();
        previousTetrimino = NextTetrimino;
        SpawnNewTetrimino();
    }

    public void Pause()
    {
        this.enabled = false;
    }
    public void Unpause()
    {
        this.enabled = true; 
    }

    public void SpawnNewTetrimino() // public function to call when a new tetrimino is needed
    {
        SpawnNextTetrimino(NextTetrimino);
    }

    public void SpawnHeldTetrimino(GameObject tetrimino)
    {
        Instantiate(tetrimino, transform.position, Quaternion.identity);
    }

    private void UpdateNextTetriminoDisplay(GameObject NextTetrimino) // Updates UI to show next tetrimino
    {
        nextTetriminoDisplay.UpdateNextTetriminoDisplay(NextTetrimino);
    }

    private void PickNextTetrimino() // Picks new random tetrimino
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

        UpdateNextTetriminoDisplay(NextTetrimino); 
    }

    private void SpawnNextTetrimino(GameObject tetrimino) // Spawns next tetrimino ingame, picks new next tetrimino.
    {
        Instantiate(tetrimino, transform.position, Quaternion.identity);
        PickNextTetrimino();
    }
}
