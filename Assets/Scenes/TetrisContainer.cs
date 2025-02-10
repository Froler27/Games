using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisContainer : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 21;
    [SerializeField] private float interval = 0.8f;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private float moveDelta = 0.1f;

    // runtime variables
    private float scale = 0.9f;
    private int[,] container;
    private float timer = 0.0f;
    private TetrisBlock currentBlock;
    private GameObject[,] blocks;
    private float halfHeight;
    private float toWorldFactor = 1.0f;
    private bool isGameOver = false;
    private int fullRow = -1;
    private int noEmptyRow = -1;


    void Awake()
    {
        container = new int [height, width];
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        Camera mainCamera = Camera.main;
        float fov = mainCamera.fieldOfView;
        float aspect = mainCamera.aspect;
        if (mainCamera.orthographic)
        {
            halfHeight = mainCamera.orthographicSize;
        }
        else
        {
            halfHeight = Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad) * Mathf.Abs(mainCamera.transform.position.z);
        }
        float halfWidth = halfHeight * aspect;
        Debug.Log("halfHeight: " + halfHeight + " halfWidth: " + halfWidth);
        toWorldFactor = halfHeight * 2 / height;



        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                container[i, j] = 0;
            }
        }

        timer = interval;
        currentBlock = new TetrisBlock();

        if (blockPrefab == null)
        {
            Debug.LogError("预制体引用为空，请在编辑器中赋值。");
        }

        blocks = new GameObject[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                blocks[i, j] = Instantiate(blockPrefab, 
                    new Vector3(j - width*0.5f, -i + height*0.5f - 0.5f, 0) * toWorldFactor, 
                    Quaternion.identity);
                blocks[i, j].transform.SetParent(this.transform);
                blocks[i, j].transform.localScale = new Vector3(scale, scale, scale) * toWorldFactor;
            }
        }
    }

    private float timerLeft = 0.0f;
    private float timerRight = 0.0f;
    private float timerDown = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            return;
        }
        HandleInput();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("space key was pressed");
        }
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (container[i, j] == 0)
                {
                    blocks[i, j].SetActive(false);
                }
                else
                {
                    blocks[i, j].SetActive(true);
                }
            }
        }
    }

    void HandleInput() 
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            timerLeft = moveDelta;
            MoveLeft();
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            timerLeft -= Time.deltaTime;
            if (timerLeft <= 0.0f)
            {
                timerLeft += moveDelta;
                MoveLeft();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            timerRight = moveDelta;
            MoveRight();
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            timerRight -= Time.deltaTime;
            if (timerRight <= 0.0f)
            {
                timerRight += moveDelta;
                MoveRight();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            timerDown = moveDelta;
            MoveDown();
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            timerDown -= Time.deltaTime;
            if (timerDown <= 0.0f)
            {
                timerDown += moveDelta;
                MoveDown();
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentBlock.RotateClockwise();
            Vector3 pos = currentBlock.GetPosition();
            if (!IsValid(pos))
            {
                currentBlock.RotateCounterClockwise();
            }
            else
            {
                currentBlock.RotateCounterClockwise();
                hideBlock();
                currentBlock.RotateClockwise();
                ShowBlock(-1);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hideBlock();
            while (true)
            {
                currentBlock.MoveDown();
                Vector3 pos = currentBlock.GetPosition();
                if (!IsValid(pos))
                {
                    currentBlock.MoveUp();
                    ShowBlock(1);
                    if (IsGameOver())
                    {
                        isGameOver = true;
                        Debug.Log("Game Over");
                        return;
                    }
                    currentBlock = new TetrisBlock();
                    fullRow = GetFullRow();
                    if (fullRow >= 0)
                    {
                        HandleFullRow(fullRow);
                    }
                    break;
                }
            }
        }
    }

    void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            timer += interval;
            MoveDown();
        }
    }

    int GetFirstNoEmptyRow()
    {
        for (int i = 0; i < height; i++)
        {
            if (!IsEmptyRow(i))
            {
                return i;
            }
        }
        return height;
    }

    bool IsEmptyRow(int row)
    {
        for (int j = 0; j < width; j++)
        {
            if (container[row, j] > 0)
            {
                return false;
            }
        }
        return true;
    }

    int GetFullRow()
    {
        noEmptyRow = GetFirstNoEmptyRow();
        for (int i = noEmptyRow; i < height; i++)
        {
            if (IsFullRow(i))
            {
                return i;
            }
        }
        return -1;
    }

    bool IsFullRow(int row)
    {
        if (row < 0 || row >= height)
        {
            return false;
        }
        for (int j = 0; j < width; j++)
        {
            if (container[row, j] == 0)
            {
                return false;
            }
        }
        return true;
    }

    void MoveLeft()
    {
        currentBlock.MoveLeft();
        Vector3 pos = currentBlock.GetPosition();
        if (IsValid(pos))
        {
            currentBlock.MoveRight();
            hideBlock();
            currentBlock.MoveLeft();
            ShowBlock(-1);
        }
        else
        {
            currentBlock.MoveRight();
        }
    }

    void MoveRight()
    {
        currentBlock.MoveRight();
        Vector3 pos = currentBlock.GetPosition();
        if (IsValid(pos))
        {
            currentBlock.MoveLeft();
            hideBlock();
            currentBlock.MoveRight();
            ShowBlock(-1);
        }
        else
        {
            currentBlock.MoveLeft();
        }
    }

    void MoveDown()
    {
        currentBlock.MoveDown();
        Vector3 pos = currentBlock.GetPosition();
        if (IsValid(pos)) // 如果下移合法
        {
            currentBlock.MoveUp();
            hideBlock();
            currentBlock.MoveDown();
            ShowBlock(-1);
        }
        else
        {
            currentBlock.MoveUp();
            ShowBlock(1);
            if (IsGameOver())
            {
                isGameOver = true;
                Debug.Log("Game Over");
                return;
            }
            currentBlock = new TetrisBlock();
            fullRow = GetFullRow();
            if (fullRow >= 0)
            {
                HandleFullRow(fullRow);
            }
        }
    }

    void HandleFullRow(int row)
    {
        Debug.Log("Full row");
        int fullRowCounter = 1;
        for (int i = fullRow + 1; i < height; i++)
        {
            if (IsFullRow(i))
            {
                fullRowCounter++;
            }
        }
        ClearRow(fullRow, fullRowCounter);
    }

    void ClearRow(int fullRow, int count)
    {
        Debug.Log("Clear row fullRow: " + fullRow + " count: " + count);
        int moveCount = fullRow - noEmptyRow + count;
        Debug.Log("moveCount: " + moveCount);
        for (int i = 0; i < moveCount; i++)
        {
            int temp = fullRow + count - 1 - i;
            Debug.Log("temp: " + temp);
            for (int j = 0; j < width; j++)
            {
                container[temp, j] = temp - count >= 0 ? container[temp - count, j] : 0;
            }
        }
    }

    bool IsValid(Vector3 pos)
    {
        Vector2[] block = currentBlock.GetBlock();
        for (int i = 0; i < block.Length; i++)
        {
            int y = (int)(-pos.y + block[i].y);
            int x = (int)(pos.x + block[i].x + (int)(width*0.5f+0.5f));
            if (y < 0)
            {
                continue;
            }
            if (x < 0 || x >= width || y >= height || container[y, x] > 0)
            {
                return false;
            }
        }
        return true;
    }

    bool IsGameOver()
    {
        Vector3 pos = currentBlock.GetPosition();
        Vector2[] block = currentBlock.GetBlock();
        for (int i = 0; i < block.Length; i++)
        {
            int y = (int)(-pos.y + block[i].y);
            int x = (int)(pos.x + block[i].x + (int)(width*0.5f+0.5f));
            if (y < 0)
            {
                continue;
            }
            return false;
        }
        return true;
    }

    void ShowBlock(int value)
    {
        Vector3 pos = currentBlock.GetPosition();
        Vector2[] block = currentBlock.GetBlock();
        for (int i = 0; i < block.Length; i++)
        {
            int y = (int)(-pos.y + block[i].y);
            int x = (int)(pos.x + block[i].x + (int)(width*0.5f+0.5f));
            container[y, x] = value;
        }
    }

    void hideBlock()
    {
        Vector3 pos = currentBlock.GetPosition();
        Vector2[] block = currentBlock.GetBlock();
        for (int i = 0; i < block.Length; i++)
        {
            int y = (int)(-pos.y + block[i].y);
            if (y < 0)
            {
                continue;
            }
            int x = (int)(pos.x + block[i].x + (int)(width*0.5f+0.5f));
            container[y, x] = 0;
        }
    }
}
