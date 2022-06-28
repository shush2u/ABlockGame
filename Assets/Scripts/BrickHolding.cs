using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickHolding : MonoBehaviour
{
    public GameObject currentHeldBrick;

    private SpawnTetrimino spawnTetrimino;

    private void Start()
    {
        spawnTetrimino = FindObjectOfType<SpawnTetrimino>();
    }

    private bool SlotEmpty()
    {
        if(currentHeldBrick == null)
        {
            return true;
        }
        return false;
    }

    public void SwapBricks(GameObject controlledBrick)
    {
        if(SlotEmpty())
        {
            spawnTetrimino.spawnNewTetrimino();
        }
        NewHeldTetrimino(controlledBrick);
    }

    private void NewHeldTetrimino(GameObject newHeldTetrimino)
    {
        if(!SlotEmpty())
        {
            currentHeldBrick.GetComponent<controls>().IsDummy(false);
            spawnTetrimino.spawnHeldTetrimino(currentHeldBrick);
            Destroy(currentHeldBrick);
        }
        currentHeldBrick = (GameObject)Instantiate(newHeldTetrimino, transform.position, Quaternion.identity);
        currentHeldBrick.GetComponent<controls>().IsDummy(true);
    }
}
