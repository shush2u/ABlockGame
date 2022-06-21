using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controls : MonoBehaviour
{

    public Vector3 rotationPoint;

    public float fallTime = 2f;
    private float previousTime = 0f;

    private static Transform[,] grid = new Transform[10, 20];

    // Level Manager Script
    private LevelManager levelManager;
    

    // Start is called before the first frame update
    void Start()
    {
        brickHolding = FindObjectOfType<BrickHolding>();
        levelManager = FindObjectOfType<LevelManager>();
        ghostBrickBehaviour = FindObjectOfType<GhostBrickBehaviour>();
        UpdateGridForGhost();
        updateGhost();
    }

    public void isDummy(bool trueOrFalse)
    {
        if(trueOrFalse == true)
        {
            this.enabled = false;
        }
        else
        {
            this.enabled = true;
        }   
    }

    // Update is called once per frame
    void Update()   
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position += Vector3.right;
            if(!validMove())
            {
                transform.position += Vector3.left;
            }
            updateGhost();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position += Vector3.left;
            if (!validMove())
            {
                transform.position += Vector3.right;
            }
            updateGhost();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
            if (!validMove())
            {
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90); 
            }
            updateGhost();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            swapBrick();
        }
    }


    private void FixedUpdate()
    {
        if (Time.time - previousTime > (Input.GetKey(KeyCode.Space) ? fallTime / 10 : fallTime))
        {
            transform.position += Vector3.down;
            if (!validMove())
            {
                transform.position += Vector3.up;
                AddToGrid();
                checkForLines();
                UpdateGridForGhost();
                levelManager.ToggleHasSwapped(false);
                this.enabled = false;
                if(spawnIsFree())
                {
                    FindObjectOfType<SpawnTetrimino>().spawnNewTetrimino();
                }
            }
            previousTime = Time.time;
        }
    }

    private bool spawnIsFree()
    {
        if (grid[4, 18] != null)
        {
            levelManager.GameOver();
            return false;
        }
        else
        {
            return true;
        }
    }

    private void AddToGrid()
    {
        foreach(Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);
            grid[roundedX, roundedY] = children;
        }
    }

    private void checkForLines()
    {
        int linesCleared = 0;
        for(int i = 19; i >= 0; i--)
        {
            bool lineClear = true;
            for(int j = 0; j < 10; j++)
            {
                if (grid[j, i] == null)
                {
                    lineClear = false;
                }
            }        
            if(lineClear == true)
            {
                DestroyLine(i);
                MoveUpperLinesDown(i);
                linesCleared++;
            }
        }
        if(linesCleared > 0)
        {
            levelManager.addPoints(linesCleared);
        }
        else
        {
            levelManager.resetConsecutiveClearCount();
        }
    }

    private void DestroyLine(int line)
    {
        for(int i = 0; i < 10; i++)
        {
            Destroy(grid[i, line].gameObject);
            grid[i, line] = null;
        }
    }

    private void MoveUpperLinesDown(int line)
    {
        for(int i = line; i < 20; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                if (grid[j, i] != null)
                {
                    grid[j, i - 1] = grid[j, i];
                    grid[j, i] = null;
                    grid[j, i - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }

    bool validMove()
    {
        foreach(Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);
            if(roundedX < 0 || roundedX > 9 || roundedY < 0 || roundedY > 20)
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

    // Brick Holding

    private BrickHolding brickHolding;

    private void swapBrick()
    {
        if(hasSwapped() == false)
        {
            brickHolding.swapBricks(this.gameObject);
            Destroy(this.gameObject);
            levelManager.ToggleHasSwapped(true);
            updateGhost();
        }
    }

    private bool hasSwapped()
    {
        return levelManager.swapCheck();
    }

    // Ghost (Prediction) Brick

    private GhostBrickBehaviour ghostBrickBehaviour;

    private void updateGhost()
    {
        ghostBrickBehaviour.updatePosition(this.gameObject);
    }

    private void UpdateGridForGhost()
    {
        ghostBrickBehaviour.UpdateGrid(grid);
    }

}
