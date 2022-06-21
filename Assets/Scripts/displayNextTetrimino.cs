using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class displayNextTetrimino : MonoBehaviour
{
    private GameObject instantiatedTetrimino;
    public void updateNextTetriminoDisplay(GameObject nextTetrimino)
    {
        newDisplayTetrimino(nextTetrimino);
    }

    private void newDisplayTetrimino(GameObject nextTetrimino)
    {
        Destroy(instantiatedTetrimino);
        instantiatedTetrimino = (GameObject) Instantiate(nextTetrimino, transform.position, Quaternion.identity);
        instantiatedTetrimino.GetComponent<controls>().IsDummy(true);
    }
}
