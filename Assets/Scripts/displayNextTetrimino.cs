using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayNextTetrimino : MonoBehaviour
{
    private GameObject instantiatedTetrimino;
    public void UpdateNextTetriminoDisplay(GameObject nextTetrimino)
    {
        NewDisplayTetrimino(nextTetrimino);
    }

    private void NewDisplayTetrimino(GameObject nextTetrimino)
    {
        Destroy(instantiatedTetrimino);
        instantiatedTetrimino = (GameObject) Instantiate(nextTetrimino, transform.position, Quaternion.identity);
        instantiatedTetrimino.GetComponent<Controls>().IsDummy(true);
    }
}
