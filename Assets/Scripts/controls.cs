using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controls : MonoBehaviour
{

    public Vector3 rotationPoint;

    // Level Manager Script
    private LevelManager levelManager;
    

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        if (SpawnIsFree())
        {
            brickHolding = FindObjectOfType<BrickHolding>();
            ghostBrickBehaviour = FindObjectOfType<GhostBrickBehaviour>();

            previousTime = Time.time; // Sets start time to current time, otherwise first tick of the tetrimino fall will happen instantly. (Except for the very first piece)

            UpdateGridForGhost();
            UpdateGhost();
        }
        else
        {
            levelManager.GameOver();
            this.enabled = false;
        }
        
    }

    /// <summary>
    /// If enables/disables this script on a GameObject
    /// </summary>
    /// <param name="trueOrFalse"></param>
    public void IsDummy(bool trueOrFalse)
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
        float horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position += Vector3.right;
            if(!ValidMove())
            {
                transform.position += Vector3.left;
            }
            UpdateGhost();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position += Vector3.left;
            if (!ValidMove())
            {
                transform.position += Vector3.right;
            }
            UpdateGhost();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
            if (!ValidMove())
            {
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90); 
            }
            UpdateGhost();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            SwapBrick();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SlamTetriminoDown();
        }
    }


    private void FixedUpdate()
    {
        if (Time.time - previousTime > (Input.GetKey(KeyCode.S) ? fallTime / fastFallTimeMultiplier : fallTime))
        {
            Descend();
        }
    }

    bool ValidMove()
    {
        foreach (Transform children in transform)
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

    // Fall Logic

    public float fallTime = 2f;
    public float fastFallTimeMultiplier = 15f;
    private float previousTime = 0f;

    private void Descend()
    {
        transform.position += Vector3.down;
        if (!ValidMove())
        {
            transform.position += Vector3.up;
            SettleTetrimino();
        }
        previousTime = Time.time;
    }

    private void SlamTetriminoDown()
    {
        bool lowestValidPointReached = false;
        do
        {
            transform.position += Vector3.down;
            if (!ValidMove())
            {
                transform.position += Vector3.up;
                SettleTetrimino();
                lowestValidPointReached = true;
            }
        } while (lowestValidPointReached == false);
    }

    //

    // Grid Logic

    private static Transform[,] grid = new Transform[10, 20];

    private void SettleTetrimino()
    {
        AddToGrid();
        CheckForLines();
        UpdateGridForGhost();
        levelManager.ToggleHasSwapped(false);
        this.enabled = false;
        FindObjectOfType<SpawnTetrimino>().spawnNewTetrimino();
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

    private void CheckForLines()
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

    private bool SpawnIsFree()
    {
        foreach(Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);
            if (grid[roundedX, roundedY] != null)
            {
                return false;
            }
        }
        return true;
    }

    //

    // Brick Holding

    private BrickHolding brickHolding;

    private void SwapBrick()
    {
        if(HasSwapped() == false)
        {
            brickHolding.swapBricks(this.gameObject);
            Destroy(this.gameObject);
            levelManager.ToggleHasSwapped(true);
            UpdateGhost();
        }
    }

    private bool HasSwapped()
    {
        return levelManager.swapCheck();
    }

    //

    // Ghost (Prediction) Brick

    private GhostBrickBehaviour ghostBrickBehaviour;

    private void UpdateGhost()
    {
        ghostBrickBehaviour.updatePosition(this.gameObject);
    }

    private void UpdateGridForGhost()
    {
        ghostBrickBehaviour.UpdateGrid(grid);
    }

    //

}
