using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class controls : MonoBehaviour
{

    public Vector3 rotationPoint;

    // Level Manager Script
    private LevelManager levelManager;


    private AudioManager audioManager;
    

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        audioManager = FindObjectOfType<AudioManager>();
        if (SpawnIsFree())
        {
            brickHolding = FindObjectOfType<BrickHolding>();
            ghostBrickBehaviour = FindObjectOfType<GhostBrickBehaviour>();
            settleSound = audioManager.settleSound;

            horizontalMoveRate = levelManager.GetHorizontalMoveRate();
            fallTime = levelManager.GetFallTime();
            fastFallTimeMultiplier = levelManager.GetFastFallTimeMultiplier();
            placeTetriminoDelay = levelManager.GetPlaceTetriminoDelay();

            previousTime = Time.time; // Sets start time to current time, otherwise first tick of the tetrimino fall will happen instantly. (Except for the very first piece)
            horizontalMoveTime = Time.time;

            UpdateGridForGhost();
            UpdateGhost();
        }
        else
        {
            levelManager.GameOver();
            this.enabled = false;
        }
        
    }

    /// <summary> Enables/disables this script on the GameObject </summary>
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

    // Basic controls

    public TetriminoControls tetriminoControls;
    private InputAction spin;
    private InputAction slam;
    private InputAction swap;
    //private InputAction move;
 

    float horizontalInput = 0;

    private float horizontalMoveRate = 0.2f;
    private float horizontalMoveTime;

    bool firstFrameRight = true;
    bool firstFrameLeft = true;

    private void Awake()
    {
        tetriminoControls = new TetriminoControls();
    }
    private void OnEnable()
    {
        spin = tetriminoControls.Player.Spin;
        slam = tetriminoControls.Player.Slam;
        swap = tetriminoControls.Player.Swap;
        //move = tetriminoControls.Player.Move;
        spin.Enable();
        slam.Enable();
        swap.Enable();
        //move.Enable();
        spin.performed += Spin;
        slam.performed += Slam;
        swap.performed += Swap;
    }

    private void OnDisable()
    {
        spin.Disable();
        slam.Disable();
        swap.Disable();
    }
    
    private void Spin(InputAction.CallbackContext context)
    {
        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
        if (!ValidMove())
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
        }
        UpdateGhost();
    }

    private void Slam(InputAction.CallbackContext context)
    {
        SlamTetriminoDown();
    }
    private void Swap(InputAction.CallbackContext context)
    {
        SwapBrick();
    }

    float testHozMovement;

    // Update is called once per frame
    void Update()   
    {
        if(this.enabled)
        {
            levelManager.SetNewCCTetrimino(this.gameObject);
        }
        if(!IsPaused())
        {
            //horizontalInput = move.ReadValue<float>();
            //Debug.Log(horizontalInput);
            horizontalInput = Input.GetAxisRaw("Horizontal");
            if (horizontalInput == 0)
            {
                firstFrameLeft = true;
                firstFrameRight = true;
                horizontalMoveTime = Time.time;
            }

            //Debug.Log(horizontalInput);

            /*if (Input.GetKeyDown(KeyCode.Z))
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
            }*/
        }
    }

    private bool IsPaused()
    {
        if(Time.timeScale != 0)
        {
            return false;
        }
        return true;
    }

    private void FixedUpdate()
    {
        if (Time.time - previousTime > (Input.GetKey(KeyCode.S) ? fallTime / fastFallTimeMultiplier : fallTime))
        {
            Descend();
        }

        if (horizontalInput > 0) // right
        {
            if (firstFrameLeft == false)
            {
                firstFrameLeft = true;
                horizontalMoveTime = Time.time;
            }

            if (firstFrameRight == true || Time.time - horizontalMoveTime > horizontalMoveRate)
            {
                horizontalMoveTime = Time.time;
                firstFrameRight = false;
                transform.position += Vector3.right;
                if (!ValidMove())
                {
                    transform.position += Vector3.left;
                }
                UpdateGhost();
            }
        }
        if (horizontalInput < 0) // left
        {
            if (firstFrameRight == false)
            {
                firstFrameRight = true;
                horizontalMoveTime = Time.time;
            }

            if (firstFrameLeft == true || Time.time - horizontalMoveTime > horizontalMoveRate)
            {
                horizontalMoveTime = Time.time;
                firstFrameLeft = false;
                transform.position += Vector3.left;
                if (!ValidMove())
                {
                    transform.position += Vector3.right;
                }
                UpdateGhost();
            }
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

    private float fallTime = 2f;
    private float fastFallTimeMultiplier = 15f;
    private float previousTime = 0f;

    private float placeTetriminoDelay = 0.2f;

    bool settlingTetrimino = false;

    private void Descend()
    {
        transform.position += Vector3.down;
        if (!ValidMove())
        {
            transform.position += Vector3.up;
            if(settlingTetrimino == false)
            {
                settlingTetrimino = true;
                Invoke("SettleTetrimino", placeTetriminoDelay);

            }
        }
        previousTime = Time.time;
    }

    bool slammed = false;

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
                slammed = true;
                lowestValidPointReached = true;
            }
        } while (lowestValidPointReached == false);
    }

    //

    // SFX

    private AudioSource settleSound;

    //

    // Grid Logic

    private static Transform[,] grid = new Transform[10, 20];

    private void SettleTetrimino()
    {
        if(!slammed)
        {
            transform.position += Vector3.down; // checks if player hasnt moved the piece since the invoke
            if (!ValidMove())
            {
                transform.position += Vector3.up;
                settleSound.Play();
                AddToGrid();
                CheckForLines();
                UpdateGridForGhost();
                levelManager.ToggleHasSwapped(false);
                this.enabled = false;
                FindObjectOfType<SpawnTetrimino>().spawnNewTetrimino();
            }
            else
            {
                previousTime = Time.time;
            }
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
            levelManager.AddPoints(linesCleared);
        }
        else
        {
            levelManager.ResetConsecutiveClearCount();
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
            brickHolding.SwapBricks(this.gameObject);
            Destroy(this.gameObject);
            levelManager.ToggleHasSwapped(true);
            UpdateGhost();
        }
    }

    private bool HasSwapped()
    {
        return levelManager.SwapCheck();
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
