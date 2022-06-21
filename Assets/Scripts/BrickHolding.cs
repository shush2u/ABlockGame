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

    public void swapBricks(GameObject controlledBrick)
    {
        if(SlotEmpty())
        {
            spawnTetrimino.spawnNewTetrimino();
        }
        newHeldTetrimino(controlledBrick);
    }

    private void newHeldTetrimino(GameObject newHeldTetrimino)
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
