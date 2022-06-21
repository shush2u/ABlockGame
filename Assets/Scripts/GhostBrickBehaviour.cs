using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBrickBehaviour : MonoBehaviour
{
    public float transparency = 0.1f;

    private GameObject currentGhostTetrimino;

    private Transform[,] grid = new Transform[10, 20];

    public void updatePosition(GameObject tetrimino)
    {
        if(currentGhostTetrimino != null)
        {
            Destroy(currentGhostTetrimino);
        }
        currentGhostTetrimino = (GameObject)Instantiate(tetrimino, tetrimino.transform.position, tetrimino.transform.rotation);
        Destroy(currentGhostTetrimino.GetComponent<controls>());
        MakeGhostTransparent();
        bool lowestValidPointReached = false;
        do
        {
            currentGhostTetrimino.transform.position += Vector3.down;
            if(!IsValidMove())
            {
                currentGhostTetrimino.transform.position += Vector3.up;
                lowestValidPointReached = true;
            }
        } while (lowestValidPointReached == false);
    }

    private void MakeGhostTransparent()
    {
        foreach (Transform children in currentGhostTetrimino.transform)
        {
            Material mat = children.GetComponent<Renderer>().material;
            Color currentColor = mat.color;
            Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, transparency);
            mat.SetColor("_Color", newColor);
        }
        
    }   

    private bool IsValidMove()  
    {
        foreach (Transform children in currentGhostTetrimino.transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);
            if (roundedX < 0 || roundedX > 9 || roundedY < 0 || roundedY > 20)
            {
                return false;
            }
            if (grid[roundedX, roundedY] != null)
            {
                return false;
            }
        }
        return true;
    }

    public void UpdateGrid(Transform[,] newGrid)
    {
        grid = newGrid;
    }
}
