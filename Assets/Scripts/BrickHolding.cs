using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickHolding : MonoBehaviour
{
    private GameObject currentHeldBrick;

    [SerializeField] private GameObject spawnTetriminoObject;
    private SpawnTetrimino spawnTetrimino;

    private void Start()
    {
        spawnTetrimino = spawnTetriminoObject.GetComponent<SpawnTetrimino>();
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
            spawnTetrimino.SpawnNewTetrimino();
        }
        NewHeldTetrimino(controlledBrick);
    }

    private void NewHeldTetrimino(GameObject newHeldTetrimino)
    {
        if(!SlotEmpty())
        {
            currentHeldBrick.GetComponent<Controls>().IsDummy(false);
            spawnTetrimino.SpawnHeldTetrimino(currentHeldBrick);
            Destroy(currentHeldBrick);
        }
        currentHeldBrick = (GameObject)Instantiate(newHeldTetrimino, transform.position, Quaternion.identity);
        currentHeldBrick.GetComponent<Controls>().IsDummy(true);
    }
}
